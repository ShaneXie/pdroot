/*
 * Overview
 * ********
 * 
 * Original shard strategy kept for backward compatibility. This class represents an Expanded shard logic.
 * An expanded shard treats each database in the shard as a simple data store; records go in a database
 * in no specific order, other than round-robin. As a result, in an expanded shard records can be stored
 * in any database without deterministic logic.
 * 
 * This class was modified from the original library to inherit from the ShardCore class. However newer
 * capabilities, such as Distributed Queries and ShardIndex have not been added to this class yet.
 * 
 * A convention used by this class is that a SqlCommand object contains a database GUID as a SqlParameter
 * in order to execute a statement against a specific database. Since each database string making up the
 * shard has a unique GUID, the library can direct a statement to a specific database using this technique.
 * 
 * Author:              Herve Roggero
 * Publishing Company:  Blue Syntax Consulting LLC
 * First Created On:    May 31 2010
 * Last Updated On:     Sept 03 2011
 * 
 * License Information
 * *******************
 * 
 * This project is open-source and covered under the Microsoft Public License (Ms-PL)
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;

namespace PYN.EnzoAzureLib
{

    public delegate DataTable CacheRequestDelegate(SqlCommand cmd, string cacheKey);
    public delegate DataTable CacheSaveDelegate(SqlCommand cmd, string cacheKey, DataTable table);

    /// <summary>
    /// Class that represents an expanded shard strategy.
    /// </summary>
    public class ShardExpanded : ShardCore
    {
        private int lastConnectionIndex = -1;
        private object lockForRoundRobin = new object();

        public event CacheRequestDelegate OnCacheRequest;
        public event CacheSaveDelegate OnCacheSave;

        public ShardExpanded(bool addConnectionGuid)
            : base(addConnectionGuid)
        {
        }

        /// <summary>
        /// Returns the next available connection string
        /// </summary>
        /// <param name="allConnections"></param>
        /// <returns></returns>
        public int GetNextConnectionIndex(List<SqlConnection> allConnections)
        {
            int connectionIndex = -1;

            lock (lockForRoundRobin)
            {
                if (lastConnectionIndex < 0 || lastConnectionIndex == allConnections.Count - 1)
                    lastConnectionIndex = 0;
                else
                    lastConnectionIndex++;

                connectionIndex = lastConnectionIndex;
            }

            return connectionIndex;
        }

        /// <summary>
        /// Returns true if a command object has no specific information about a database GUID, in which case it must be a round-robin execution.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="guidColName"></param>
        /// <returns></returns>
        public bool IsRoundRobinCall(SqlCommand command, string guidColName)
        {
            return (command.Parameters.Contains(guidColName) && command.Parameters[guidColName].Value == null);
        }

        /// <summary>
        /// Returns the list of databases to execute a statement against.
        /// </summary>
        /// <param name="allConnections"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public List<SqlConnection> GetDatabasesToOperateOn(List<SqlConnection> allConnections, SqlCommand command)
        {
            List<SqlConnection> res = new List<SqlConnection>();

            if (IsRoundRobinCall(command, SharedSettings._TENANTKEY_))
            {
                // ROUND-ROBIN CALL: REF #004
                // First determine if this should be treated as a round-robin execution method
                // If so, execute the statement on only 1 database; increment the counter that
                // keeps track of the last database used and execute the single call
                res.Add(allConnections[GetNextConnectionIndex(allConnections)]);
                return res; 
            }
            else
               return allConnections;

        }

        /// <summary>
        /// Executes a parallel execution of commands using a round-robin mechanism. Mostly used 
        /// for insert commands that need to load lots of data.
        /// </summary>
        /// <param name="commands">Collection of commands to execute in the shard</param>
        /// <param name="connections">List of connections to use for this call</param>
        /// <returns></returns>
        public long ExecuteParallelRoundRobinLoad(List<SqlCommand> commands, List<SqlConnection> connections)
        {
            //
            // This is a special method that performs a load on all the underlying database connections
            // in the shard using a round-robin method 
            //
            object alock = new object();
            long res = 0;

            var exceptions = new System.Collections.Concurrent.ConcurrentQueue<Exception>();

            // Set a limit to the number of parallel queries this method can execute.
            // This is optional and is considered a safeguard. You can experiment by replacing this
            // value by a constant, such as 10 or 20. 
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = connections.Count; 

            Parallel.ForEach(commands, po, delegate(SqlCommand cmd)
            {
                int connectionIndex = GetNextConnectionIndex(connections);

                long rowsAffected = ExecuteSingleNonQuery(cmd, connections[connectionIndex].ConnectionString, exceptions);

                lock (alock)
                    res += rowsAffected;
            }
            );

            if (!exceptions.IsEmpty)
                throw new AggregateException(exceptions);

            return res;

        }

        /// <summary>
        /// Executes a NonQuery call to the Shard
        /// </summary>
        /// <param name="command">The command object containing the SQL call</param>
        /// <param name="connections">The collection of connections that make up the shard</param>
        /// <remarks>This procedure executes a NonQuery against the Shard. The _GUID_ parameter controls 
        /// which databases receive this command. Not adding this column to the list of parameters will run the statement 
        /// on all databases; a NULL value will use a round-robin method (a NULL value should only be used for INSERT commands), 
        /// and a value will execute the command on the desired database only. </remarks>
        /// <returns></returns>
        public long ExecuteShardNonQuery(SqlCommand command, List<SqlConnection> connections)
        {
            object alock = new object();
            long res = 0;

            try
            {
                var exceptions = new System.Collections.Concurrent.ConcurrentQueue<Exception>();

                // If we have a connection parameter, ensure we run the statement on the correct connection
                if (command.Parameters.Contains(SharedSettings._TENANTKEY_))
                {
                    connections = connections.Where( p => p.ConnectionGuid() ==  ((string)command.Parameters[SharedSettings._TENANTKEY_].Value)).ToList();
                    command.Parameters.Remove(command.Parameters[SharedSettings._TENANTKEY_]);
                }   

                // REF #003
                Parallel.ForEach(connections, delegate(SqlConnection c)
                //foreach (SqlConnection c in connections)
                {
                    
                    long rowsAffected = ExecuteSingleNonQuery(command, c.ConnectionString, exceptions);

                    // Add the data to the aggregated data table
                    lock (alock)
                        res += rowsAffected;

                }
                );

                if (!exceptions.IsEmpty)
                    throw new AggregateException(exceptions);
            }
            catch (AggregateException ex)
            {
                throw ex;
            }
            catch (Exception generalEx)
            {
                throw generalEx;
            }


            return res;

        }

        /// <summary>
        /// Executes a command that returns data against one or more connections
        /// </summary>
        /// <remarks>This method is meant for Read operations. You can specify the _GUID_ command parameter to read from a single
        /// database backend, or not specify it at all to read from the entire shard. This method returns a _GUID_ column that 
        /// is used when making a call to ExecuteShardNonQuery. 
        /// </remarks>
        public DataTable ExecuteShardQuery(SqlCommand command, QueryOptions queryOptions, List<SqlConnection> connections)
        {
            DataTable data = new DataTable();

            #region Executes the command against the connections collection and fill the data object
            try
            {
                var exceptions = new System.Collections.Concurrent.ConcurrentQueue<Exception>();

                // REF #003
                Parallel.ForEach(connections, delegate(SqlConnection c)
                //foreach (SqlConnection c in connections)
                {

                    string cacheKey = "";       // the key to retrieve from the cache, or to save into the cache
                    DataTable dt = null;

                    if (OnCacheRequest != null)
                    {
                        string tmpKey = "";
                        foreach (SqlParameter p in command.Parameters)
                            tmpKey += p.ParameterName + ":" + (p.Value ?? "<null>").ToString() + "|";   // warning: order of keys important...
                        tmpKey += command.CommandText;
                        tmpKey += "GUID" + "|" + c.ConnectionGuid();
                        cacheKey = tmpKey.GetHash();

                        dt = OnCacheRequest(command.Clone(), cacheKey);

                    }

                    if (dt == null)
                        dt = ExecuteSingleQuery(command, c.ConnectionString, exceptions);

                    if (OnCacheSave != null)
                        OnCacheSave(command, cacheKey, dt);

                    // Add the data to the aggregated data table
                    lock (data)
                        data.Merge(dt, true, MissingSchemaAction.Add);


                }
                );

                if (!exceptions.IsEmpty)
                    throw new AggregateException(exceptions);

            }
            catch (AggregateException ex)
            {
                throw ex;
            }
            catch (Exception generalEx)
            {
                throw generalEx;
            }
            
            #endregion

            #region Apply Order By and Top conditions
            // At this point, we have retrieved the data from all underlying connections
            // We need to apply the OrderBy and the Top conditions 

            DataView dv = data.DefaultView;
            dv.Sort = queryOptions.OrderBy;

            DataTable res = new DataTable();

            // Add a new column that represents a unique identifier for the aggregated result set
            DataColumn uid = new DataColumn(SharedSettings._GUIDROW_);
            uid.DataType = typeof(int);
            uid.AutoIncrement = true;
            uid.AutoIncrementSeed = 1;

            // If a TOP command is expected, load the rows one at a time
            if (queryOptions.Top > 0)
            {
                res = dv.Table.Clone();
                res.Columns.Add(uid);
                int i = 0;
                foreach (DataRow dr in dv.Table.Rows)
                    if (i++ < queryOptions.Top)
                        res.ImportRow(dr);
                    else
                        break;
            }
            else
                res.Merge(dv.Table);
            #endregion

            // Reapply the sort on the final datatable
            res.DefaultView.Sort = dv.Sort;

            return res;

        }

    }
}
