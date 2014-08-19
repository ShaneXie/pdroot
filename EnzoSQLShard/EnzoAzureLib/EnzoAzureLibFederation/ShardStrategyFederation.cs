/*
 * Overview
 * ********
 * 
 * This class provides an implementation of the Enzo Shard library that leverages the Data Federation capability of SQL Azure. 
 * 
 * 
 * Limitations
 * ***********
 * 
 *      - This shard implementation requires SQL Azure and Data Federation. It will not function on SQL Server 2008 R2 or earlier.
 * 
 * 
 * Author:              Herve Roggero
 * Publishing Company:  Blue Syntax Consulting LLC
 * First Created On:    June 24 2011
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

namespace PYN.EnzoAzureLib.DataFederation
{
    /// <summary>
    /// Represents a Federation Member in a SQL Azure Data Federated database. A federation member contains records
    /// for a range of federation values. When a statement is executed by this class, the FILTER is set to ON if a 
    /// distribution value is specified, or OFF if it is null. 
    /// </summary>
    /// <remarks>You cannot use this class to query multiple federation members. To perform queries across
    /// federation members, use the Federation class or the Root class. </remarks>
    public class FederationMember : ShardCore
    {

        #region Public Properties
        public string Name = "";
        public int FederationId = 0;
        public string DistributionName = "";
        public string DistributionType = "";
        public string UserType = "";
        public int MaxLength = 0;
        public int Precision = 0;
        public int MemberId = 0;
        public int Scale = 0;
        public object RangeLow = null;
        public object RangeHigh = null;
        public Federation ParentFederation = null;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public FederationMember()
        {
            OnPreProcessConnection += new PreProcessConnectionDelegate(FederationMember_OnPreProcessConnection);
        }

        /// <summary>
        /// Determines if the database connection should point to a specific federation member, and whether the
        /// connection should be filtered.
        /// </summary>
        /// <param name="connection">The original database connection to use.</param>
        /// <param name="options">The execution options providing the context information to evaluate.</param>
        /// <remarks>The ShardExecutionOptions parameter contains a property, called Mode, that dictates how the context will be modified. If UNKNOWN is used, the database 
        /// connection string is used as-is, and the statement will execute against the database specified. If ROOT is used, the context will be set  to the ROOT database.
        /// If FILTERING_ON or FILTERING_OFF is used, the FederationName, FederationKey and FederationValue fields are used to define which federation member to connect to.
        /// </remarks>
        /// <returns></returns>
        internal SqlConnection FederationMember_OnPreProcessConnection(SqlConnection connection, ShardCore.ShardExecutionOptions options)
        {
            string preExecute = "";

            if (options != null && options.Mode != ShardOperationMode.UNKNOWN)
            {

                if (connection.State == ConnectionState.Closed)
                    connection.TryOpen();

                if (options.Mode == ShardOperationMode.ROOT)
                    preExecute = "USE FEDERATION ROOT WITH RESET ";
                else
                {
                    preExecute = "USE FEDERATION {0}({1}={2}) WITH RESET, FILTERING = {3} ";
                    preExecute = string.Format(preExecute, options.ShardName, options.ShardKey, options.MemberValue, ((options.Mode == ShardOperationMode.FILTERING_ON) ? "ON" : "OFF"));
                }

                // Execute the preliminary query now
                SqlCommand cmd_preliminary = new SqlCommand(preExecute, connection);
                cmd_preliminary.ExecuteNonQuery();
            }

            return connection;
        }

        /// <summary>
        /// Returns a ShardExecutionOptions class based on the member distribution value.
        /// </summary>
        /// <param name="distributionValue"></param>
        /// <returns></returns>
        internal ShardExecutionOptions GetMemberOptions(string distributionValue)
        {
            ShardExecutionOptions dfo = new ShardExecutionOptions();
            dfo.ShardName = ParentFederation.Name;
            dfo.ShardKey = DistributionName;
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
            return ExecuteDataTable(new SqlCommand(sql), new ShardExecutionOptions() , ex);
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
            return base.ExecuteSingleNonQuery(new SqlCommand(sql), ParentFederation.RootConnectionString, dfo, ex);
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
            return base.ExecuteSingleQuery(new SqlCommand(sql), ParentFederation.RootConnectionString, dfo, ex);
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
            return base.ExecuteSingleNonQuery(command, ParentFederation.RootConnectionString, dfo, ex);
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
            return base.ExecuteSingleQuery(command, ParentFederation.RootConnectionString, dfo, ex);
        }

        #endregion

    }

    /// <summary>
    /// Represents a collection of federations, and their federated members. Each Root Database has a collection of federations,
    /// which has a unique name and ID. Most of the operations against federations are performed on this class.
    /// </summary>
    /// <remarks>Use this class to execute SQL statements against a federation (across federation members), or against a single federation member.
    /// The DefaultExecutionContext property provides enough information for the library to determine if the statement provided should be executed
    /// against all federation members, a specific federation member with FILTER OFF, or FILTER ON. When a statement executes against multiple
    /// federation members, the library uses parallel processing to collect the data, and returns a single Data Table object. 
    /// </remarks>
    public class Federation : List<FederationMember>
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

        public Federation() { }

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
        internal List<FederationMember> GetFederationMembersToOperateOn(ShardCore.ShardExecutionOptions dfOptions)
        {

            List<FederationMember> members = new List<FederationMember>();

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
                    foreach (KeyValuePair<int,bool> map in maps.Where(p => p.Value == true))
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
                members.Add(this[0]);
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
            List<FederationMember> membersToUse = null;
            
            membersToUse = GetFederationMembersToOperateOn(dfOptions);

            DataTable data = new DataTable();

            #region Execute command across members
            try
            {
                var exceptions = new System.Collections.Concurrent.ConcurrentQueue<Exception>();

                Parallel.ForEach(membersToUse, delegate(FederationMember fm)
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

                    if (dfOptionsToUse.Mode == ShardCore.ShardOperationMode.FAN_OUT)
                        dfOptionsToUse.MemberValue = fm.RangeLow;
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
    public class RootDatabase : List<Federation>
    {
        #region SQL
        private string _sqlFederationDiscovery = @"SELECT 
F.federation_id,
F.name as [FederationName],
FD.distribution_name,
FD.distribution_type,
TYPE_NAME(user_type_id) as user_type,
max_length,
precision,
scale,
member_id,
range_low,
range_high
FROM
sys.federations F JOIN sys.federation_distributions FD ON F.federation_id = FD.federation_id
JOIN sys.federation_member_distributions FMD ON FMD.federation_id = F.federation_id
ORDER BY
F.federation_id, range_low";
        #endregion

        /// <summary>
        /// A list of federations within a root database. 
        /// </summary>
        //public List<Federation> Federations { get; internal set; }
        internal string _connectionString = "";

        /// <summary>
        /// Returns a Federation that belongs to the root database
        /// </summary>
        /// <param name="name">The name of the federation.</param>
        /// <returns></returns>
        public Federation this[string name]
        {
            get { return this.FirstOrDefault(p => p.Name.ToLower() == name.ToLower()); }
        }
        
        /// <summary>
        /// Loads all the federations and their members given the root database.
        /// </summary>
        /// <param name="rootDBConnectionString">The connection string to the root database.</param>
        internal void LoadFederations(string rootDBConnectionString)
        {
            SqlConnection conn = new SqlConnection(rootDBConnectionString);
            SqlCommand cmd = new SqlCommand(_sqlFederationDiscovery);
            SqlDataReader dr = ShardExtensions.TryDataReader(conn, cmd);

            _connectionString = rootDBConnectionString;
            //Federations = new List<Federation>();
            base.Clear();

            int lastFederationId = -1;
            Federation f = null;

            while (dr.Read())
            {
                string key = dr["federation_id"].ToString();

                // Add a new federation if not already done
                if (lastFederationId != int.Parse(key))
                {
                    f = new Federation();
                    f.Name = dr["federationName"].ToString();
                    f.FederationID = dr["federation_id"].ToString();
                    f.FederationKey = dr["distribution_name"].ToString();
                    
                    // By default: operate on the distribution name in a fan-out mode
                    f.DefaultExecutionContext = new ShardCore.ShardExecutionOptions();
                    f.DefaultExecutionContext.ShardName = dr["federationname"].ToString();
                    f.DefaultExecutionContext.ShardKey = dr["distribution_name"].ToString();
                    f.DefaultExecutionContext.MemberValue = dr["range_low"].ToString();
                    f.DefaultExecutionContext.Mode = ShardCore.ShardOperationMode.FAN_OUT;
                    f.RootDatabase = this;

                    f.MemberIndexes = new ShardIndexes(
                        f.DefaultExecutionContext.ShardKey,
                        f.DefaultExecutionContext.ShardName,
                        new ShardExecuteDataDelegate(f.ExecuteDataTable));

                    Add(f);
                    lastFederationId = int.Parse(key);
                }

                // Add the federation member now
                FederationMember fm = new FederationMember();

                fm.Name = dr["federationName"].ToString();
                fm.DistributionName = dr["distribution_name"].ToString();
                fm.DistributionType = dr["distribution_type"].ToString();
                fm.UserType = dr["user_type"].ToString();
                fm.MaxLength = int.Parse(dr["max_length"].ToString());
                fm.Precision = int.Parse(dr["precision"].ToString());
                fm.Scale = int.Parse(dr["scale"].ToString());
                fm.MemberId = int.Parse(dr["member_id"].ToString());
                fm.RangeLow = dr["range_low"];
                fm.RangeHigh = dr["range_high"];
                fm.ParentFederation = f;

                f.Add(fm);

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
            LoadFederations(rootDBConnectionString);
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
                        dfo.ShardKey = this[dfo.ShardName].FederationKey;

                    return this[dfo.ShardName].ExecuteDataTable(sql, dfo);
                }
            }
            else
            {
                throw new Exception("When executing a distributed query against the Root Database object, you must specify the FEDERATED ON hint.");
            }
        }

    }


}
