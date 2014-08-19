/*
 * Overview
 * ********
 * 
 * This class provides an implementation of the Enzo Shard library that implemented a Compress Shard model.
 * The Compressed model allows you to build multitenant applications easily by separating the tenants in multiple databases and/or schemas
 * 
 * 
 * Author:              Herve Roggero
 * Publishing Company:  Blue Syntax Consulting LLC
 * First Created On:    May 09 2013
 * 
 * License Information
 * *******************
 * 
 * This project is open-source and covered under the Microsoft Public License (Ms-PL)
 * 
 * Remarks
 * *******
 * 
 * This strategy implements two concepts:
 * 
 *      Tenant:     The database the stores a customer (tenant) objects and records
 *      Tenants:    The list of tenants in a Root database
 *      Root:       The security/root database that controls the list of tenants and global tables
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;

namespace PYN.EnzoAzureLib.CompressedShard
{
    /// <summary>
    /// A Tenant is an actual database (or schema within a database) that contains a tenant records and database objects.
    /// </summary>
    public class Tenant : ShardCore
    {

        #region Public Properties
        public string Name = "";
        public int TenantId = 0;
        //public string DistributionName = "";
        //public string DistributionType = "";
        //public string UserType = "";
        //public int MaxLength = 0;
        //public int Precision = 0;
        public int MemberId = 0;
        //public int Scale = 0;
        //public object RangeLow = null;
        //public object RangeHigh = null;
        public Tenants ParentContainer = null;

        public string TenantKey = "";   // The client key used to find the customer database
        public string uid = "";
        public string pwd = "";
        public string server = "";
        public string database = "";
        public int port = 1433;

        #endregion

        public Tenant()
        {
            OnPreProcessConnection += new PreProcessConnectionDelegate(ShardStrategyCompressed_OnPreProcessConnection);
        }

        /// <summary>
        /// Changes the context of the database based on the customer for which the following statements should execute.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        internal SqlConnection ShardStrategyCompressed_OnPreProcessConnection(SqlConnection connection, ShardCore.ShardExecutionOptions options)
        {

            if (options != null && options.Mode != ShardOperationMode.UNKNOWN)
            {

                if (options.Mode == ShardOperationMode.ROOT)
                {
                    // Connect to the root database
                    connection.ConnectionString = ParentContainer.RootConnectionString;
                }
                else
                {
                    // Connect to the customer database/schema
                    connection.ConnectionString = ConnectionString;
                }

                if (connection.State == ConnectionState.Closed)
                    connection.TryOpen();
            }

            return connection;
        }

        /// <summary>
        /// Return a connection string based on the information provided by other properties. Automatically detect the use of 
        /// Integrated Authentication (SSPI) if the user id is blank. Also use port 1433 unless specified otherwise and specify the 
        /// default database if provided (or use master by default). 
        /// </summary>
        internal string ConnectionString
        {
            get
            {
                SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();
                sb.IntegratedSecurity = (uid == null || uid.Trim() == "");
                if (!sb.IntegratedSecurity)
                {
                    sb.UserID = uid;
                    sb.Password = pwd;
                }
                sb.DataSource = server;
                if (port != 1433) sb.DataSource += "," + port.ToString();
                sb.InitialCatalog = (database == "" || database == null) ? "master" : database;
                return sb.ConnectionString;
            }
        }

        /// <summary>
        /// Returns a ShardExecutionOptions class based on the member distribution value.
        /// </summary>
        /// <param name="distributionValue"></param>
        /// <returns></returns>
        internal ShardExecutionOptions GetMemberOptions(string distributionValue)
        {
            ShardExecutionOptions dfo = new ShardExecutionOptions();
            dfo.ShardName = ParentContainer.Name;
            dfo.ShardKey = "";
            dfo.MemberValue = distributionValue;
            dfo.Mode = ShardOperationMode.FILTERING_OFF;

            if (distributionValue != null) dfo.Mode = ShardOperationMode.FILTERING_ON;

            return dfo;
        }

        #region ExecuteNonQuery and ExecuteDataTable overloads

        /// <summary>
        /// Executes a SQL command against a Federation Member. The Default Data Federation Options provide contextual information about the connection to establish.
        /// </summary>
        /// <param name="command">The SQL command to execute.</param>
        /// <returns></returns>
        public long ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(new SqlCommand(sql), GetMemberOptions(null), null);
        }

        /// <summary>
        /// Executes a SQL command against a Data Federation Member using the Default Data Fedaration Options. 
        /// </summary>
        /// <param name="command">The SQL command to execute.</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sql)
        {
            return ExecuteDataTable(new SqlCommand(sql), GetMemberOptions(null), null);
        }

        /// <summary>
        /// Executes a SQL command against a Federation Member. The Default Data Federation Options provide contextual information about the connection to establish.
        /// </summary>
        /// <param name="sql">The SQL command to execute.</param>
        /// <param name="ex">The exception collection to use if an error occurs.</param>
        /// <returns></returns>
        public long ExecuteNonQuery(string sql, System.Collections.Concurrent.ConcurrentQueue<Exception> ex)
        {
            return ExecuteNonQuery(new SqlCommand(sql), GetMemberOptions(null), ex);
        }

        /// <summary>
        /// Executes a SQL command against a Data Federation Member using the Default Data Fedaration Options. 
        /// </summary>
        /// <param name="sql">The SQL command to execute.</param>
        /// <param name="ex">The exception collection to use if an error occurs.</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sql, System.Collections.Concurrent.ConcurrentQueue<Exception> ex)
        {
            return ExecuteDataTable(new SqlCommand(sql), new ShardExecutionOptions(), ex);
        }

        /// <summary>
        /// Executes a SQL command against a Federation Member. The Data Federation Options provide contextual information about the connection to establish.
        /// </summary>
        /// <param name="sql">The SQL command to execute.</param>
        /// <param name="dfo">The Federation Options to use.</param>
        /// <param name="ex">The exception collection to use if an error occurs.</param>
        /// <returns></returns>
        public long ExecuteNonQuery(string sql, ShardCore.ShardExecutionOptions dfo, System.Collections.Concurrent.ConcurrentQueue<Exception> ex)
        {
            return base.ExecuteSingleNonQuery(new SqlCommand(sql), ParentContainer.RootConnectionString, dfo, ex);
        }

        /// <summary>
        /// Executes a SQL command against a Data Federation Member using the options provided. 
        /// </summary>
        /// <param name="sql">The T-SQL statement to execute.</param>
        /// <param name="dfo">The execution options.</param>
        /// <param name="ex">The exception collection to use if an error occurs.</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sql, ShardCore.ShardExecutionOptions dfo, System.Collections.Concurrent.ConcurrentQueue<Exception> ex)
        {
            return base.ExecuteSingleQuery(new SqlCommand(sql), ParentContainer.RootConnectionString, dfo, ex);
        }

        /// <summary>
        /// Executes a command against a Federation Member. The Default Data Federation Options provide contextual information about the connection to establish.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns></returns>
        public long ExecuteNonQuery(SqlCommand command)
        {
            return ExecuteNonQuery(command, GetMemberOptions(null), null);
        }

        /// <summary>
        /// Executes a command against a Data Federation Member using the Default Data Fedaration Options. 
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(SqlCommand command)
        {
            return ExecuteDataTable(command, GetMemberOptions(null), null);
        }

        /// <summary>
        /// Executes a command against a Federation Member. The Default Data Federation Options provide contextual information about the connection to establish.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="ex">The exception collection to use if an error occurs.</param>
        /// <returns></returns>
        public long ExecuteNonQuery(SqlCommand command, System.Collections.Concurrent.ConcurrentQueue<Exception> ex)
        {
            return ExecuteNonQuery(command, GetMemberOptions(null), ex);
        }

        /// <summary>
        /// Executes a command against a Data Federation Member using the Default Data Fedaration Options. 
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="ex">The exception collection to use if an error occurs.</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(SqlCommand command, System.Collections.Concurrent.ConcurrentQueue<Exception> ex)
        {
            return ExecuteDataTable(command, GetMemberOptions(null), ex);
        }

        /// <summary>
        /// Executes a command against a Federation Member. The Data Federation Options provide contextual information about the connection to establish.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="dfo">The Federation Options to use.</param>
        /// <param name="ex">The exception collection to use if an error occurs.</param>
        /// <returns></returns>
        public long ExecuteNonQuery(SqlCommand command, ShardCore.ShardExecutionOptions dfo, System.Collections.Concurrent.ConcurrentQueue<Exception> ex)
        {
            return base.ExecuteSingleNonQuery(command, ParentContainer.RootConnectionString, dfo, ex);
        }

        /// <summary>
        /// Executes a command against a Data Federation Member using the options provided. 
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="dfo">The execution options.</param>
        /// <param name="ex">The exception collection to use if an error occurs.</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(SqlCommand command, ShardCore.ShardExecutionOptions dfo, System.Collections.Concurrent.ConcurrentQueue<Exception> ex)
        {
            return base.ExecuteSingleQuery(command, ParentContainer.RootConnectionString, dfo, ex);
        }

        #endregion

    }


    /// <summary>
    /// Represents a collection of Tenant Containers, and their databases. Each Root Database has a collection of Tenants,
    /// which has a unique name and ID. Most of the operations against tenants are performed on this class. In most cases
    /// an environment will have a single Tenant Container, which holds all the tenants (individual customers). However
    /// in some instances, more than one Tenant Container is needed. For example, a development environment may have its own
    /// Tenant Container for test customers, while a Production container may hold the production tenants. Another way to use
    /// a Tenant Container is to store multiple archives of customer data.
    /// </summary>
    /// <remarks>Use this class to execute SQL statements against a Tenant Container (across Tenant Databases), or against a single Tenant Database.
    /// The DefaultExecutionContext property provides enough information for the library to determine if the statement provided should be executed
    /// against all Tenant Databases, a specific Tenant Database. When a statement executes against multiple
    /// Tenant Databases, the library uses parallel processing to collect the data, and returns a single Data Table object. 
    ///
    /// The logic of operation modes is as follows:
    /// 
    ///     ROOT: executes a command against the root database that contains the definition of all current tenants and tenant databases
    ///     FILTER_ON: executes a command against a single tenant (in a schema or a dedicated database)
    ///     FILTER_OFF: executes a command against all necessary tenants 
    ///     FAN_OUT: same as FILTER_OFF
    /// 
    /// </remarks>
    public class Tenants : List<Tenant>
    {
        /// <summary>
        /// The default execution context is used when none is provided in the ExecuteNonQuery and ExecuteDataTable methods. 
        /// </summary>
        public ShardCore.ShardExecutionOptions DefaultExecutionContext { get; set; }

        /// <summary>
        /// Bit index kept in memory used to select which federation members are likely to contain the desired data.
        /// </summary>
        /// <remarks>
        /// To use this property you need to build indexes in the client code. Once the indexes are built, you can specify
        /// the name of an index to use in the IndexName property of the execution context.
        /// </remarks>
        public ShardIndexes MemberIndexes { get; set; }

        public Tenants() { }

        public string Name { get; internal set; }
        public string FederationID { get; internal set; }
        public string FederationKey { get; internal set; }
        public RootDatabase RootDatabase { get; internal set; }

        internal string RootConnectionString { get { return RootDatabase._connectionString; } }

        /// <summary>
        /// Returns a collection of federation members against which a statement should be executed. 
        /// </summary>
        /// <param name="dfOptions">The data federation option to use.</param>
        /// <returns></returns>
        internal List<Tenant> GetFederationMembersToOperateOn(ShardCore.ShardExecutionOptions dfOptions)
        {

            List<Tenant> members = new List<Tenant>();

            if (dfOptions.Mode == ShardCore.ShardOperationMode.ROOT) throw new Exception("ShardOperationMode is set to ROOT. Use the RootDatabase class to execute a statement against the root database.");

            if (dfOptions.Mode == ShardCore.ShardOperationMode.FAN_OUT)
            {
                // Is there an index name provided?
                if (dfOptions.IndexName != "")
                {
                    // Find out which federation members need to be returned
                    // This is an optimization technique that assumes the cache hit 
                    // currently loaded in memory correctly helps in determining which
                    // federation members contain the needed records.

                    MemberIndexes.BuildIfInvalid(dfOptions.IndexName);

                    // then return the HitMap
                    Dictionary<int, bool> maps = MemberIndexes[dfOptions.IndexName].HitMap;

                    // None found? exit.
                    if (maps == null) return members;

                    // Find all the hits and add the federation member to the list
                    foreach (KeyValuePair<int, bool> map in maps.Where(p => p.Value == true))
                        members.Add(this.First(p => p.MemberId == map.Key));

                    return members;
                }
                else
                    return this;
            }

            // Return the first federation member
            // It doesn't have to be the actual federation member because
            // the SQL execution code will automatically connect to the correct federation member.
            else if (dfOptions.Mode == ShardCore.ShardOperationMode.FILTERING_OFF || dfOptions.Mode == ShardCore.ShardOperationMode.FILTERING_ON)
            {
                if (dfOptions.Mode == ShardCore.ShardOperationMode.FILTERING_OFF)
                    members.AddRange(base.ToArray());
                else
                    members.AddRange(base.ToArray().Where(p => p.TenantKey.ToLower() == dfOptions.MemberValue.ToString().ToLower()));
                return members;
            }

            throw new Exception("Could not determine which federation member(s) to use.");

        }

        /// <summary>
        /// Executes a SQL command against a federation, using the default data federation options.
        /// </summary>
        /// <param name="sql">The SQL statement to execute.</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sql)
        {
            return ExecuteDataTable(sql, DefaultExecutionContext);
        }

        /// <summary>
        /// Executes a SQL command against a federation, using the FILTER_ON option on the Federation Value specified.
        /// </summary>
        /// <param name="sql">The SQL statement to execute.</param>
        /// <param name="federationValue">The federation value to filter on.</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(object federationValue, string sql)
        {
            return ExecuteDataTable(federationValue, new SqlCommand(sql));
        }

        /// <summary>
        /// Executes a SQL command against a federation, using the specified data federation options.
        /// </summary>
        /// <param name="sql">The SQL statement to execute.</param>
        /// <param name="dfOptions">The data federation options to use.</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sql, ShardCore.ShardExecutionOptions dfOptions)
        {
            return ExecuteDataTable(new SqlCommand(sql), dfOptions);
        }

        /// <summary>
        /// Executes a command against a federation, using the default data federation options.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(SqlCommand command)
        {
            return ExecuteDataTable(command, DefaultExecutionContext);
        }

        /// <summary>
        /// Executes a command against a federation, using the FILTER_ON option on the Federation Value specified.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="federationValue">The federation value to filter on.</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(object federationValue, SqlCommand command)
        {
            // Create a default execution option
            ShardCore.ShardExecutionOptions dfo = new ShardCore.ShardExecutionOptions();
            dfo.ShardName = DefaultExecutionContext.ShardName;
            dfo.ShardKey = DefaultExecutionContext.ShardKey;
            //dfo.Mode = ShardCore.ShardOperationMode.FILTERING_ON;
            dfo.Mode = DefaultExecutionContext.Mode;
            dfo.MemberValue = federationValue;

            return ExecuteDataTable(command, dfo);
        }

        /// <summary>
        /// Executes a command against a federation, using the specified data federation options.
        /// </summary>
        /// <remarks>
        /// This command is the one that executes a statement across multiple federation members in parallel using 
        /// the Task Parallel Library. 
        /// </remarks>
        /// <param name="command">The command to execute.</param>
        /// <param name="dfOptions">The data federation options to use.</param>
        /// <returns></returns>
        internal DataTable ExecuteDataTable(SqlCommand command, ShardCore.ShardExecutionOptions dfOptions)
        {
            SqlConnection connection = new SqlConnection();
            object alock = new object();

            // Get the list of federated members to operate on
            List<Tenant> membersToUse = null;

            membersToUse = GetFederationMembersToOperateOn(dfOptions);

            DataTable data = new DataTable();

            #region Execute command across members
            try
            {
                var exceptions = new System.Collections.Concurrent.ConcurrentQueue<Exception>();

                Parallel.ForEach(membersToUse, delegate(Tenant fm)
                //foreach (SqlConnection c in connections)
                {
                    DataTable dt = new DataTable();

                    // Build a federation option dynamically and set most values
                    // to those provided. However, change the federation value
                    // to reflect the current federation being used so the context can
                    // be changed accordingly if we are executing a FAN OUT operation.
                    ShardCore.ShardExecutionOptions dfOptionsToUse = new ShardCore.ShardExecutionOptions();
                    dfOptionsToUse.ShardKey = dfOptions.ShardKey;
                    dfOptionsToUse.ShardName = dfOptions.ShardName;
                    dfOptionsToUse.Mode = dfOptions.Mode;

                    // TODO: Implement FAN OUT operation in this sharding model
                    if (dfOptionsToUse.Mode == ShardCore.ShardOperationMode.FAN_OUT)
                        dfOptionsToUse.MemberValue = -1;
                    else
                        dfOptionsToUse.MemberValue = dfOptions.MemberValue;

                    dt = fm.ExecuteDataTable(command, dfOptionsToUse, exceptions);

                    if (dfOptions.AddTenantKey)
                    {
                        DataTable dt2 = new DataTable();
                        DataColumn col = dt2.Columns.Add(SharedSettings._TENANTKEY_, typeof(string));
                        col.DefaultValue = fm.MemberId.ToString();
                        dt.Merge(dt2, true, MissingSchemaAction.Add);
                    }

                    // Add the data to the aggregated data table
                    lock (alock)
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

            return data;
        }


    }

    /// <summary>
    /// The Root Database class is the root database in the SQL Azure Data Federation specification. This is the parent class of a SQL Azure federation database
    /// and can be used to query one ore more federations. 
    /// </summary>
    /// <remarks>
    /// The Federations property contains the list of federations that were found in the root database. To query a specific federation by name,
    /// use the following syntax:  root_db["federation_name"].  
    /// Generally speaking you can use this class to query for data when the data is located in the root database, or when executing a Distributed Query.
    /// Distributed Queries can only be executed from this class.
    /// </remarks>
    public class RootDatabase : Tenants
    {
        #region SQL
        private string _sqlShardDiscovery = @"
SELECT 
    SD.id as serverid,
    servername, 
    port, 
    TD.uid, 
    TD.pwd, 
    TD.id as databaseid, 
    TD.databasename,
    TD.TenantKey  
FROM 
    serversdef SD JOIN TenantsDef TD ON SD.id = TD.serverid 
WHERE 
    SD.enabled=1 AND TD.enabled = 1
ORDER BY
    SD.id, TD.id
";

        #endregion

        /// <summary>
        /// The connection string to the root database 
        /// </summary>
        internal string _connectionString = "";

        /// <summary>
        /// List of tenants in this root database
        /// </summary>
        public Tenants Tenants { get; set; }

        /// <summary>
        /// Reloads the list of tenants from the database
        /// </summary>
        public void Refresh()
        {
            Load(_connectionString);
        }

        /// <summary>
        /// Loads all the shards and their tenants given the root database.
        /// </summary>
        /// <param name="rootDBConnectionString">The connection string to the root database.</param>
        internal void Load(string rootDBConnectionString)
        {
            SqlConnection conn = new SqlConnection(rootDBConnectionString);
            SqlCommand cmd = new SqlCommand(_sqlShardDiscovery);
            SqlDataReader dr = ShardExtensions.TryDataReader(conn, cmd);

            _connectionString = rootDBConnectionString;
            base.Clear();

            int lastId = -1;
            Tenants = null;

            while (dr.Read())
            {
                string key = dr["serverid"].ToString();

                // Add a new federation if not already done
                if (lastId != int.Parse(key))
                {
                    // Only load the first Tenant collection from the list 
                    // This strategy only supports one tenant collection in the root database
                    if (Tenants != null) break;
                    Tenants = new Tenants();
                    Tenants.Name = key;
                    Tenants.FederationID = dr["serverid"].ToString();
                    Tenants.FederationKey = "";

                    // By default: operate on the distribution name in a Filtered Mode
                    Tenants.DefaultExecutionContext = new ShardCore.ShardExecutionOptions();
                    Tenants.DefaultExecutionContext.ShardName = "";
                    Tenants.DefaultExecutionContext.ShardKey = "";
                    Tenants.DefaultExecutionContext.MemberValue = "";
                    Tenants.DefaultExecutionContext.Mode = ShardCore.ShardOperationMode.FILTERING_ON;
                    Tenants.RootDatabase = this;

                    Tenants.MemberIndexes = new ShardIndexes(
                        Tenants.DefaultExecutionContext.ShardKey,
                        Tenants.DefaultExecutionContext.ShardName,
                        new ShardExecuteDataDelegate(Tenants.ExecuteDataTable));

                    lastId = int.Parse(key);
                }

                // Add the federation member now
                Tenant fm = new Tenant();

                fm.Name = dr["databaseid"].ToString();
                fm.TenantKey = dr["tenantKey"].ToString();
                fm.database = dr["databasename"].ToString();
                fm.uid = dr["uid"].ToString();
                fm.pwd = System.Text.UTF8Encoding.UTF8.GetString((byte[])dr["pwd"]);
                fm.server = dr["servername"].ToString();
                fm.MemberId = int.Parse(dr["databaseid"].ToString());
                fm.ParentContainer = Tenants;
                Tenants.Add(fm);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public RootDatabase(string rootDBConnectionString)
            : base()
        {
            // Build a list of federated members in all available federations
            //Federations = new List<Federation>();
            Load(rootDBConnectionString);
        }

        /// <summary>
        /// Executes a T-SQL statement against the default context property. The statement is executed against the root database.
        /// </summary>
        /// <param name="sql">The T-SQL statement to execute</param>
        /// <returns></returns>
        public long ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(new SqlCommand(sql));
        }

        /// <summary>
        /// Executes a command against the default context property. The statement is executed against the root database.
        /// </summary>
        /// <param name="command">The command object to execute</param>
        /// <returns></returns>
        public long ExecuteNonQuery(SqlCommand command)
        {
            ShardCore.ShardExecutionOptions dfo = new ShardCore.ShardExecutionOptions();
            dfo.Mode = ShardCore.ShardOperationMode.ROOT;
            ShardCore sc = new ShardCore();
            return sc.ExecuteSingleNonQuery(command, _connectionString, dfo, null);
        }

        /// <summary>
        /// Executes a SQL statement against the database. 
        /// </summary>
        /// <remarks>
        /// If the command is a distributed command, the SQL
        /// statement will be parsed and executed against the correct federation member(s). If the command 
        /// is a simple query, it will be executed against the root database.
        /// </remarks>
        /// <param name="sql">A regular T-SQL statement, or a Distributed T-SQL statement.</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sql)
        {
            return ExecuteDataTable(new SqlCommand(sql));
        }

        /// <summary>
        /// Executes a SQL statement against the database. 
        /// </summary>
        /// <remarks>
        /// If the command is a distributed command, the SQL
        /// statement will be parsed and executed against the correct federation member(s). If the command 
        /// is a simple query, it will be executed against the root database.
        /// </remarks>
        /// <param name="command">A regular T-SQL statement, or a Distributed T-SQL statement.</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(SqlCommand command)
        {
            ShardCore.ShardExecutionOptions dfo = new ShardCore.ShardExecutionOptions();
            dfo.Mode = ShardCore.ShardOperationMode.ROOT;
            ShardCore sc = new ShardCore();

            // Inspect the statement; if it is a distributed command, 
            // parse it and execute the inner command only
            DistributedQuery dq = new DistributedQuery(command.CommandText);
            if (dq.IsDistributed)
            {

                DataTable res1 = null;
                DataTable res2 = null;

                // check if data table is found in the cache first...
                res1 = dq.GetDataTableFromCache();

                if (res1 == null)
                {
                    // Execute distributed queries as needed
                    res1 = ExecuteDistributedSQL(dq.DistributedStatement1.DistributedSQL, dq.Shard1);

                    if (dq.DistributedStatement2 != null)
                        res2 = ExecuteDistributedSQL(dq.DistributedStatement2.DistributedSQL, dq.Shard2);

                    res1 = dq.ProcessResults(res1, res2);
                }

                return res1;

            }
            else
            {
                return sc.ExecuteSingleQuery(command, _connectionString, dfo, null);
            }
        }

        /// <summary>
        /// Executes the portion of a distributed query with specific execution options that were extracted by the calling method.
        /// </summary>
        /// <param name="sql">The statement to execute on a federation member</param>
        /// <param name="dq">The Distributed Query object</param>
        /// <param name="dfo">The execution options</param>
        /// <returns></returns>
        private DataTable ExecuteDistributedSQL(string sql, ShardCore.ShardExecutionOptions dfo)
        {
            ShardCore sc = new ShardCore();

            // Override the execution options if one is provided
            if (dfo != null)
            {
                if (dfo.Mode == ShardCore.ShardOperationMode.ROOT)
                {
                    SqlCommand cmd = new SqlCommand(sql);
                    return sc.ExecuteSingleQuery(cmd, _connectionString, dfo, null);
                }
                else
                {
                    if (dfo.ShardKey == null)
                        dfo.ShardKey = Tenants.FederationKey;

                    return Tenants.ExecuteDataTable(sql, dfo);
                }
            }
            else
            {
                throw new Exception("When executing a distributed query against the Root Database object, you must specify the FEDERATED ON hint.");
            }
        }

    }
}
