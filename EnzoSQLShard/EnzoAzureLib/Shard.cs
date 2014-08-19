
/*
 * *******************************************************************************************************************************************
 * 
 * THIS IS THE ORIGINAL SHARD LIBRARY AVAILABLE FOR REFERENCE. PLEASE USE THE SHARDSTRATEGYEXPANDED CLASS INSTEAD, OR SHARDSTARTEGYFEDERATION
 * 
 * *******************************************************************************************************************************************
 * 
 * Name: Enzo Shard Library
 * 
 * 
 * 
 * The purpose of this library is to provide an open-source project of a possible
 * implementation of a Shard: a Horizontal Partition Shard (HPS). A HPS allows your code to query and modify data against
 * multiple databases (SQL Server and SQL Azure) without knowing precisely which database
 * contains the actual data. 
 * 
 * With this library you may be able to achieve the following objectives:
 * 
 *   -  Increase system performance by adding another database to the shard
 *   -  Increase scalability by adding multiple databases to the shard
 *   -  Spread the load of queries over multiple databases
 *   -  Reduce the impact of running a database in the cloud
 * 
 * You should note however that your schema design may impact the ability to use this library.
 * Since the shard is a HPS, a table is split horizontally and as such the primary of each
 * table is not unique throughout the shard. As a result, an additional key, the database GUID,
 * is required to fully resolve the location of a record in the shard. the database GUID is also
 * called a breadcrumb, and it is calculated on the fly and added automatically to the resultsets
 * when calling on the shard methods.
 * 
 * ExecuteShardQuery should be used when reading data from the shard. It returns the breadcrumb (GUID).
 * ExecuteShardNonQuery should be used when updating data in the shard. It expects the breadcrumb back.
 * ExecuteParallelRoundRobinLoad performs a massive insert of records in the shard, in a round robing manner.
 * UseParallel is true by default and spreads the shard execute methods over multiple threads using the Parallel Task Library (PTL)
 * UseCache is true by default and loads a cache (Enterprise Library Cache) of results to avoid further roundtrips
 * UseSlidingWindow controls the behavior of the cache in a way that slides the expiration of its content as the data is being read
 * 
 * This library is limited in a few ways:
 * 
 *   -  It requires the use of SqlCommand objects to function
 *   -  It does not detect automatically if a commmand is a Read or an Update/Delete/Insert
 *   -  Its cache is local only and does not leverage the Windows Server AppFabric
 *   -  Due to its use of the PTL and the newer cache, it requires .NET 4.0
 * 
 * Author: Herve Roggero
 * Copyright: Roggero Consulting LLC
 * Publishing Company: Blue Syntax Consulting
 * Date: June 24 2011
 * Version: 2.0.0
 * 
 * Disclaimer:
 * This library is provided as-is with no guarantees or warantees of any kind, either implied or inferred. 
 * This library is provided for evaluation purposes only and is not meant for production systems. This  code 
 * was never tested in a production system. 
 * 
 * 
 * WARNING
 * *************************************************************
 * 
 *      The Shard class below is provided as a backward compatibility class for projects that use the initial implementation of the shard library.
 *      However it is recommended to use the ShardExpanded class, or other strategy classes directly for new projects. 
 * 
 * *************************************************************
 * 
 * */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Runtime.Caching;
using System.Threading;

namespace PYN.EnzoAzureLib
{
    /// <summary>
    /// This class provides extension methods and uses the ShardExpanded strategy class, which offers an expanded shard implementation. 
    /// </summary>
    public static class Shard
    {
        #region Variables Section


        private static ShardExpanded _shard = null;
        
        private static TimeSpan defaultTTL = new TimeSpan(0, 1, 0);     // default time to live: 1 minute
        
        public static bool UseSlidingWindow = true;                    // by default use a sliding expiration 
        public static bool UseCache = true;

        #endregion

        
        /// <summary>
        /// Clears the cache
        /// </summary>
        public static void ResetCache()
        {
            for (int i = (int)MemoryCache.Default.GetCount() - 1; i >= 0; i--)
                MemoryCache.Default.Remove(MemoryCache.Default.ElementAt(i).Key);
        }

        /// <summary>
        /// The list of Sql Connections forming the shard
        /// </summary>
        /// <returns></returns>
        public static List<SqlConnection> ShardConnections {get;set;}
        
        /// <summary>
        /// Executes a command that returns data against the connections specified in ShardConnections
        /// </summary>
        public static DataTable ExecuteShardQuery(this SqlCommand command)
        {
            return ShardInstance.ExecuteShardQuery(command, new QueryOptions(0, ""), ShardConnections);
        }

        /// <summary>
        /// Executes a command that returns data against the connections specified in ShardConnections; does not use the cache
        /// </summary>
        public static DataTable ExecuteShardQuery(this SqlCommand command, QueryOptions queryOptions)
        {
            return ShardInstance.ExecuteShardQuery(command, queryOptions, ShardConnections);
        }

        ///// <summary>
        ///// Executes a command that returns data against one or more connections
        ///// </summary>
        ///// <remarks>This method is meant for Read operations. You can specify the _GUID_ command parameter to read from a single
        ///// database backend, or not specify it at all to read from the entire shard. This method returns a _GUID_ column that 
        ///// is used when making a call to ExecuteShardNonQuery. 
        ///// </remarks>
        //public static DataTable ExecuteShardQuery(this SqlCommand command, QueryOptions queryOptions, List<SqlConnection> connections)
        //{
        //    DataTable data = new DataTable();

        //    #region Executes the command against the connections collection and fill the data object
        //    if (UseParallel)
        //    {
        //        try
        //        {
        //            var exceptions = new System.Collections.Concurrent.ConcurrentQueue<Exception>();

        //            // REF #003
        //            Parallel.ForEach(connections, delegate(SqlConnection c)
        //            //foreach (SqlConnection c in connections)
        //            {
        //                DataTable dt = ExecuteSingleQuery(command, c, exceptions);

        //                // Add the data to the aggregated data table
        //                lock (data)
        //                    data.Merge(dt, true, MissingSchemaAction.Add);

        //            }
        //            );

        //            if (!exceptions.IsEmpty)
        //                throw new AggregateException(exceptions);

        //        }
        //        catch (AggregateException ex)
        //        {
        //            throw ex;
        //        }
        //        catch (Exception generalEx)
        //        {
        //            throw generalEx;
        //        }
        //    }
        //    else
        //    {
        //        foreach (SqlConnection c in connections)
        //        {
        //            DataTable dt = ExecuteSingleQuery(command, c, null);

        //            // Add the data to the aggregated data table
        //            lock (data)
        //                data.Merge(dt, true, MissingSchemaAction.Add);

        //        }
        //    }
        //    #endregion

        //    #region Apply Order By and Top conditions
        //    // At this point, we have retrieved the data from all underlying connections
        //    // We need to apply the OrderBy and the Top conditions 

        //    DataView dv = data.DefaultView;
        //    dv.Sort = queryOptions.OrderBy;

        //    DataTable res = new DataTable();

        //    // Add a new column that represents a unique identifier for the aggregated result set
        //    DataColumn uid = new DataColumn(SharedSettings._GUIDROW_);
        //    uid.DataType = typeof(int);
        //    uid.AutoIncrement = true;
        //    uid.AutoIncrementSeed = 1;

        //    // If a TOP command is expected, load the rows one at a time
        //    if (queryOptions.Top > 0)
        //    {
        //        res = dv.Table.Clone();
        //        res.Columns.Add(uid);
        //        int i = 0;
        //        foreach (DataRow dr in dv.Table.Rows)
        //            if (i++ < queryOptions.Top)
        //                res.ImportRow(dr);
        //            else
        //                break;
        //    }
        //    else
        //        res.Merge(dv.Table);
        //    #endregion

        //    // Reapply the sort on the final datatable
        //    res.DefaultView.Sort = dv.Sort;

        //    return res;

        //}

        /// <summary>
        /// Executes a NonQuery call to the Shard
        /// </summary>
        /// <remarks>This procedure executes a NonQuery against the Shard. The _GUID_ parameter controls 
        /// which databases receive this command. Not adding this column to the list of parameters will run the statement 
        /// on all databases; a NULL value will use a round-robin method (a NULL value should only be used for INSERT commands), 
        /// and a value will execute the command on the desired database only. </remarks>
        /// <returns></returns>
        public static long ExecuteShardNonQuery(this SqlCommand command)
        {
            return ExecuteShardNonQuery(command, ShardConnections);
        }

        /// <summary>
        /// Executes a parallel execution of commands using a round-robin mechanism. Mostly used 
        /// for insert commands that need to load lots of data.
        /// </summary>
        /// <param name="commands">Collection of commands to execute in the shard</param>
        /// <returns></returns>
        public static long ExecuteParallelRoundRobinLoad(this List<SqlCommand> commands)
        {
            return ExecuteParallelRoundRobinLoad (commands, ShardConnections);
        }

        /// <summary>
        /// Runs a command that behaves like an INSERT by selecting the next database in a round robin manner.
        /// </summary>
        /// <param name="commands">Collection of commands to execute in the shard</param>
        /// <returns></returns>
        public static long ExecuteNextRoundRobin(this SqlCommand command)
        {
            List<SqlCommand> commands = new List<SqlCommand>();
            commands.Add(command);
            return ExecuteParallelRoundRobinLoad(commands, ShardConnections);
        }

        /// <summary>
        /// Executes a parallel execution of commands using a round-robin mechanism. Mostly used 
        /// for insert commands that need to load lots of data.
        /// </summary>
        /// <param name="commands">Collection of commands to execute in the shard</param>
        /// <param name="connections">List of connections to use for this call</param>
        /// <returns></returns>
        public static long ExecuteParallelRoundRobinLoad(this List<SqlCommand> commands, List<SqlConnection> connections)
        {
            return ShardInstance.ExecuteParallelRoundRobinLoad(commands, connections);
        }

        /// <summary>
        /// Creates a single instance of an Expanded Shard. This ensures that round-robin calls will work as expected over multiple calls.
        /// </summary>
        public static ShardExpanded ShardInstance
        {
            get
            {
                if (_shard == null)
                    _shard = new ShardExpanded(true);
                return _shard;
            }
        }

        ///// <summary>
        ///// Executes a NonQuery call to the Shard
        ///// </summary>
        ///// <param name="command">The command object containing the SQL call</param>
        ///// <param name="connections">The collection of connections that make up the shard</param>
        ///// <remarks>This procedure executes a NonQuery against the Shard. The _GUID_ parameter controls 
        ///// which databases receive this command. Not adding this column to the list of parameters will run the statement 
        ///// on all databases; a NULL value will use a round-robin method (a NULL value should only be used for INSERT commands), 
        ///// and a value will execute the command on the desired database only. </remarks>
        ///// <returns></returns>
        public static long ExecuteShardNonQuery(this SqlCommand command, List<SqlConnection> connections)
        {
            return ShardInstance.ExecuteShardNonQuery(command, connections);
        }

        /// <summary>
        /// Executes a single command againt a single database 
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="connection">The connection object</param>
        /// <param name="exceptions">The exceptions collection</param>
        /// <remarks>This command can be called for reads and writes, although it is primarily meant for read. 
        /// This procedure tries to find paramaters that provide a hint as to which database should be called. 
        /// The _GUID_ variable holds the field name that contains this information. </remarks>
        /// <returns></returns>
        //private static DataTable ExecuteSingleQuery(
        //    SqlCommand command, 
        //    SqlConnection connectionToUse, 
        //    System.Collections.Concurrent.ConcurrentQueue<Exception> exceptions)
        //{

        //    DataTable dt = new DataTable();

        //    try
        //    {
        //        // Create a copy of the command object
        //        SqlCommand cmd = command.Clone();
        //        SqlDataAdapter da = new SqlDataAdapter(cmd);
                
        //        SqlConnection connection = new SqlConnection(connectionToUse.ConnectionString); // create a new connection object to avoid collisions in multi-threaded apps
                
        //        string cacheKey = "";       // the key to retrieve from the cache, or to save into the cache

        //        System.Diagnostics.Debug.WriteLine("Fetching records from GUID: " + connection.ConnectionGuid());

        //        // Build the cache key now - this key is used to retrieve a cached resultset 
        //        // and/or to insert a cache item. Note that the _GUID_ value will be added
        //        // if present, allowing the cache to be granular enough
        //        if (UseCache)
        //        {
        //            string tmpKey = "";
        //            foreach (SqlParameter p in cmd.Parameters)
        //                tmpKey += p.ParameterName + ":" + (p.Value ?? "<null>").ToString() + "|";   // warning: order of keys important...
        //            tmpKey += cmd.CommandText;
        //            tmpKey += "GUID" + "|" + connection.ConnectionGuid();
        //            cacheKey = tmpKey.GetHash();
        //            //System.Diagnostics.Debug.WriteLine("cache key calculated: " + cacheKey);
        //        }

        //        // Is the _GUID_ parameter provided? And if so, is the parameter's value = to the connection's GUID?
        //        if (cmd.Parameters.Contains(SharedSettings._GUID_) && cmd.Parameters[SharedSettings._GUID_].Value != null && cmd.Parameters[SharedSettings._GUID_].Value.ToString() != connection.ConnectionGuid())
        //            // REF #001
        //            return dt;  // ... no ... so proceed to the next connection - return an empty datatable
        //        else if (cmd.Parameters.Contains(SharedSettings._GUID_))
        //            // REF #002
        //            cmd.Parameters.Remove(cmd.Parameters[SharedSettings._GUID_]);  // ... yes ... remove this extra field - the database doesn't know what that is

        //        // Attempt to fetch from cache...
        //        if (UseCache && cacheKey != "")
        //        {
        //            // try to fetch from the cache, if the key is found
        //            object val = MemoryCache.Default[cacheKey];
        //            if (val != null)
        //            {
        //                //System.Diagnostics.Debug.WriteLine("rows fetched from cache: " + cacheKey + " -> " + ((DataTable)val).Rows.Count.ToString() + " rows");
        //                return (DataTable)val;
        //            }
        //        }

        //        // Set the command connection 
        //        connection.TryOpen();
        //        cmd.Connection = connection;

        //        // Add the connection GUID to this set of records
        //        // This helps us identify which row came from which connection
        //        DataColumn col = dt.Columns.Add(SharedSettings._GUID_, typeof(string));
        //        col.DefaultValue = connection.ConnectionGuid();

        //        // Get the data
        //        ShardExtensions.TryFill(da, dt);
        //        connection.Close();

        //        // Add to the cache if requested...
        //        // The cache item will expire automatically at the set timespan using a sliding window
        //        // Do not add to the cache if the item is already added and has not yet expired
        //        if (UseCache && cacheKey != "" && !MemoryCache.Default.Contains(cacheKey))
        //        {
        //            CacheItemPolicy cip = new CacheItemPolicy();
        //            if (UseSlidingWindow)
        //                cip.SlidingExpiration = defaultTTL;
        //            else
        //                cip.AbsoluteExpiration = new DateTimeOffset(System.DateTime.Now.Add(defaultTTL));
        //            MemoryCache.Default.Add(cacheKey, dt, cip);
        //            //System.Diagnostics.Debug.WriteLine("data added to cache: " + cacheKey + " -> " + dt.Rows.Count.ToString() + " rows");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        // Should we silence this exception and add it to the list of exceptions?
        //        if (exceptions != null)
        //            exceptions.Enqueue(ex);
        //        else
        //            throw;
        //    }

        //    return dt;
        //}

        /// <summary>
        /// Executes a NonQuery against a single database
        /// </summary>
        /// <param name="command">The NonQuery command</param>
        /// <param name="connection">The connection object</param>
        /// <param name="exceptions">The exceptions collection</param>
        /// <remarks>This procedure is meant for writes (insert, update and delete). The _GUID_ column dictates
        /// which databases will receive this statement.</remarks>
        /// <returns></returns>
        //private static long ExecuteSingleNonQuery(SqlCommand command, SqlConnection connectionToUse, System.Collections.Concurrent.ConcurrentQueue<Exception> exceptions)
        //{

        //    long res = 0;

        //    try
        //    {
        //        // Create a copy of the command object
        //        SqlCommand cmd = command.Clone();
        //        SqlConnection connection = new SqlConnection(connectionToUse.ConnectionString); // create a new connection object to avoid collisions in multi-threaded apps

        //        // Is the _GUID_ parameter provided? And if so, is the parameter's value = to the connection's GUID?
        //        if (cmd.Parameters.Contains(SharedSettings._GUID_) && cmd.Parameters[SharedSettings._GUID_].Value != null && cmd.Parameters[SharedSettings._GUID_].Value.ToString() != connection.ConnectionGuid())
        //            // REF #001
        //            return 0;  // ... no ... so proceed to the next connection - return an empty datatable
        //        else if (cmd.Parameters.Contains(SharedSettings._GUID_))
        //            // REF #002
        //            cmd.Parameters.Remove(cmd.Parameters[SharedSettings._GUID_]);  // ... yes ... remove this extra field - the database doesn't know what that is
                
        //        // Set the command connection 
        //        connection.TryOpen();
        //        cmd.Connection = connection;

        //        // Get the data
        //        int val = cmd.TryExecuteNonQuery();
        //        connection.Close();

        //        res += val;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Should we silence this exception and add it to the list of exceptions?
        //        if (exceptions != null)
        //            exceptions.Enqueue(ex);
        //        else
        //            throw;
        //    }

        //    return res;
        //}

        /// <summary>
        /// Calculates a GUID for a connection string, based on its connection string
        /// </summary>
        //public static string ConnectionGuid(this SqlConnection connection)
        //{
        //    // REF #005
        //    SqlConnectionStringBuilder cb = new SqlConnectionStringBuilder(connection.ConnectionString);
        //    string connUID =
        //        ((cb.UserID != null) ? cb.UserID : "SSPI") + "#" +
        //        cb.DataSource + "#" +
        //        ((cb.InitialCatalog != null) ? cb.InitialCatalog : "master");
        //    string connHash = connUID.GetHash().ToString();
        //    return connHash;
        //}


    }
}
