/*
 * Overview
 * ********
 * 
 * This class provides the base foundation logic for all strategy classes that implementing sharding. 
 * 
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
using System.Data;
using System.Data.SqlClient;

namespace PYN.EnzoAzureLib
{
    public class ShardCore
    {
        /// <summary>
        /// Signature of a delegate that pre-processes a connection object.
        /// </summary>
        /// <param name="connection">The connection object</param>
        /// <param name="options">The shard execution options</param>
        /// <returns></returns>
        public delegate SqlConnection PreProcessConnectionDelegate(SqlConnection connection, ShardExecutionOptions options);
        
        /// <summary>
        /// Event raised when a connection needs to be pre-processed by the shard strategy class
        /// </summary>
        public event PreProcessConnectionDelegate OnPreProcessConnection;

        /// <summary>
        /// Enum indicating the mode of execution of a statement. Most shard strategy classes will use
        /// at least the FAN_OUT, ROOT and possily FILTERING_OFF values. FILTERING_ON is specific to
        /// SQL Azure Data Federation since this is a filtering feature by that platform.
        /// </summary>
        public enum ShardOperationMode
        {
            UNKNOWN = 0,            // Not defined
            FILTERING_ON = 1,       // Filtering to a specific key within a specific shard instance (federation member/database)
            FILTERING_OFF = 2,      // Unfiltered request within a federation member (this option is only used by the Federation sharding model)
            ROOT = 4,               // Execution against the root database
            FAN_OUT = 8,            // Execution against one or more shard instances (federation members within a federation, multiple databases)
        }

        /// <summary>
        /// Execution options of a statement.
        /// </summary>
        /// <remarks>
        /// This class is used by strategy sharding libraries to indicate which shard instance(s) should be used when
        /// executing a SQL statement. 
        /// </remarks>
        public class ShardExecutionOptions
        {
            public string ShardName = "";
            public string ShardKey = "";
            public object MemberValue = "";
            public string IndexName = "";           // The index name to use when executing a query
            public ShardOperationMode Mode = ShardOperationMode.UNKNOWN;
            public bool AddTenantKey = false;       // When set to true, a column is added dynamically that contains the Tenant Key 
        }

        public ShardCore() { }
        public ShardCore(bool addConnectionGuid) { AddConnectionGuid = addConnectionGuid; }

        /// <summary>
        /// Establishes a connection to the database and uses the execution options provided. 
        /// Returns a SqlConnection object to use for the actual command
        /// which points to the correct database within a shard.
        /// </summary>
        /// <remarks>This method calls the OnPreProcessConnection event to let the shard strategy
        /// class perform any preliminary tasks, such as running a T-SQL statement, before the
        /// connection is passed to the execution code that runs the command.</remarks>
        /// <param name="plainConnectionString">The connection string of the database on which a command is to be executed.</param>
        /// <param name="fd_options">The execution options to use determining on which database(s) the command should run.</param>
        /// <returns></returns>
        internal SqlConnection ProcessConnectionString(string plainConnectionString, ShardExecutionOptions fd_options)
        {

            SqlConnection connectionToUse = new SqlConnection(plainConnectionString); // create a new connection object to avoid collisions in multi-threaded apps
            
            if (OnPreProcessConnection != null)
                OnPreProcessConnection(connectionToUse, fd_options);

            if (connectionToUse.State == ConnectionState.Closed)
                connectionToUse.TryOpen();

            return connectionToUse;

        }
        
        /// <summary>
        /// Executes a command against a specific connection
        /// </summary>
        /// <param name="command">The command to execute against the database, or federation member.</param>
        /// <param name="connectionString">The connection string to use. </param>
        /// <param name="exceptions">An exception collection to which any error will be appended to.</param>
        /// <remarks>This option will not leverage SQL Azure Data Federation.</remarks>
        /// <returns></returns>
        public DataTable ExecuteSingleQuery(
            SqlCommand cmd,
            string connectionString,
            System.Collections.Concurrent.ConcurrentQueue<Exception> exceptions)
        {
            return ExecuteSingleQuery(cmd, connectionString, new ShardExecutionOptions(), exceptions);
        }

        /// <summary>
        /// Executes a command against a specific connection
        /// </summary>
        /// <param name="command">The command to execute against the database, or federation member.</param>
        /// <param name="connectionString">The connection string to use. </param>
        /// <param name="exceptions">An exception collection to which any error will be appended to.</param>
        /// <remarks>When connecting against a SQL Azure Data Federation, the ShardExecutionOptions parameter is used to set the context of the database connection.
        /// </remarks>
        /// <returns></returns>
        public DataTable ExecuteSingleQuery(
            SqlCommand cmd,
            string connectionString,
            ShardExecutionOptions fd_options,
            System.Collections.Concurrent.ConcurrentQueue<Exception> exceptions)
        {

            DataTable dt = new DataTable();

            try
            {
                SqlCommand command = cmd.Clone();
                SqlConnection connectionToUse = ProcessConnectionString(connectionString, fd_options); 

                // Set the command connection 
                SqlDataAdapter da = new SqlDataAdapter(command);
                connectionToUse.TryOpen();
                command.Connection = connectionToUse;

                // Should a TenantKey column be added to the output?
                if (AddConnectionGuid || (fd_options != null && fd_options.AddTenantKey))
                {
                    // Add the connection GUID to this set of records
                    // This helps us identify which row came from which connection
                    DataColumn col = dt.Columns.Add(SharedSettings._TENANTKEY_, typeof(string));
                    
                    // fd_options could be null for sharding models that do not use the shard execution options, such as the Expanded Shard Strategy
                    // If null, return the connection Guid to serve as an identifier for the tenant database
                    if (AddConnectionGuid)
                        col.DefaultValue = connectionToUse.ConnectionGuid();
                    else if (fd_options != null && fd_options.AddTenantKey)
                        col.DefaultValue = fd_options.MemberValue;
                }

                // Get the data
                ShardExtensions.TryFill(da, dt);
                connectionToUse.Close();

            }
            catch (Exception ex)
            {
                // Should we silence this exception and add it to the list of exceptions?
                if (exceptions != null)
                    exceptions.Enqueue(ex);
                else
                    throw;
            }

            return dt;
        }


        /// <summary>
        /// Executes a NonQuery against a single database
        /// </summary>
        /// <param name="command">The NonQuery command</param>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="fd_options">The execution options to use when running this command</param>
        /// <param name="exceptions">The exceptions collection</param>
        /// <remarks>This procedure is meant for writes (insert, update and delete). The execution options
        /// provide information to the shard strategy object allowing it to change the context of the database
        /// based on the options provided. 
        /// </remarks>
        /// <returns></returns>
        public long ExecuteSingleNonQuery(
            SqlCommand command, 
            string connectionString,
            ShardExecutionOptions options,
            System.Collections.Concurrent.ConcurrentQueue<Exception> exceptions)
        {

            long res = 0;

            try
            {
                
                SqlConnection connectionToUse = ProcessConnectionString(connectionString, options); 

                // Set the command connection 
                connectionToUse.TryOpen();
                command.Connection = connectionToUse;

                // Get the data
                res = command.TryExecuteNonQuery();
                connectionToUse.Close();

            }
            catch (Exception ex)
            {
                // Should we silence this exception and add it to the list of exceptions?
                if (exceptions != null)
                    exceptions.Enqueue(ex);
                else
                    throw;
            }

            return res;
        }

        /// <summary>
        /// Executes a NonQuery against a single database
        /// </summary>
        /// <param name="command">The NonQuery command</param>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="exceptions">The exceptions collection</param>
        /// <remarks>This procedure is meant for writes (insert, update and delete). </remarks>
        /// <returns></returns>
        public long ExecuteSingleNonQuery(
            SqlCommand command, 
            string connectionString,
            System.Collections.Concurrent.ConcurrentQueue<Exception> exceptions)
        {

            return ExecuteSingleNonQuery(command, connectionString, new ShardExecutionOptions(), exceptions);
        }

        /// <summary>
        /// True if a GUID or Tenant Key representing the tenant database will be returned as part of the result set.
        /// </summary>
        /// <remarks>
        /// This method is typically used by shard libraries that do not have no way of knowing which records
        /// are located in which database in a shard. 
        /// This property should not be used by newer sharding libraries. Use the AddTenantKey from the ShardExecutionOptions class instead.
        /// </remarks>
        public bool AddConnectionGuid { get; private set; }

        

    }
}
