/*
 * Overview
 * ********
 * 
 * This class provides a simple implementation of a Distributed Command Parse. 
 * It is not meant to be used for complex parsing logic at the moment; this class
 * will evolve to provide more robust parsing logic in the future.
 * 
 * 
 * Author:              Herve Roggero
 * Publishing Company:  Blue Syntax Consulting LLC
 * First Created On:    June 01 2011
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
using System.Text.RegularExpressions;
using System.Data;

#if false

    Supported distributed statement structure:
    -----------------------------------------

    SELECT fields USING ( sql1 ) FEDERATED ON {ROOT, (fed_name [, member_key = member_value [, FILTERED]]) }
    [WHERE field [NOT] IN ( sql2 ) FEDERATED ON {ROOT, (fed_name [, member_key = member_value [, FILTERED]]) }] 
    [ORDER BY fields3]
    [CACHED FOR n seconds]

    In-Memory Structure
    -------------------

    DistributedQuery
        string TopN = 100%          [integer or percentage value]
        string[] Fields = { * }     [could include MIN, MAX, AVG, COUNT...]
        string[] Fields2 = { }      [list of fields grouped on]
        bool IsAggregate = false    [true if Fields contain MIN, MAX...]
        bool IsFiltered = false     [true if WHERE clause included]
        string FilterType = ""      [IN, NOT_IN]
        string SQL1 = ""
        string SQL2 = ""
        string OriginalSQL = ""
        Tuple Federation1 = { Type = [root, default, named]; Name = "fed_key"; Value = "key" }
        Tuple Federation2 = { Type = [root, default, named]; Name = "fed_key"; Value = "key" }
        

    Notes
    -----
        - Aggregates, if present, must be applied to all fields (except those found in fields2)
        - Aggregates must operate on a specific field; the use of * is not supported
        - The use of DISTINCT is not supported

#endif


namespace PYN.EnzoAzureLib
{
    /// <summary>
    /// Class that contains a T-SQL statement for a distributed query.
    /// </summary>
    /// <remarks>
    /// This class parses a distributed T-SQL statement entirely and builds multiple
    /// properties that make up the parts of the distributed call. 
    /// </remarks>
    public class DistributedQuery
    {
        internal string _sql = "";
        internal static CachedTables _cache = new CachedTables();

        public DistributedQuery(string sql) { _sql = sql; Parse(); }

        public DistributedStatement DistributedStatement1 { get; private set; }
        public DistributedStatement DistributedStatement2 { get; private set; }
        private ClientProcessingWHERE WHEREOperation { get; set; }
        private ClientProcessingORDERBY ORDEROperation { get; set; }
        private ClientProcessingCACHE CACHEOperation { get; set; }

        public bool IsDistributed { get; set; }
        public bool IsAggregated { get { return (DistributedStatement1 != null && DistributedStatement1.ClientSQL != null && DistributedStatement1.ClientSQL != ""); } }
        public bool IsCacheRequested { get { return CACHEOperation.HasCache; } }

        /// <summary>
        /// Returns a hash of the SQL statement used as the cache key
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private string GetKeyForSQL(string sql)
        {
            // remove the last few characters if caching is part of that SQL
            string rxCache = @".+\s+(cached\s+for\s+.*)";
            Match match = Regex.Match(sql, rxCache, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (match.Success)
                sql = sql.Substring(0, match.Groups[1].Index);

            return sql.Trim().GetHash();
        }

        /// <summary>
        /// Returns a DataTable from memory/cache if it is found, or null if no cache is found
        /// </summary>
        /// <returns></returns>
        public DataTable GetDataTableFromCache()
        {
            string key = GetKeyForSQL(_sql);
            if (_cache.ContainsKey(key))
                return _cache[key].Table;
            return null;
        }

        public ShardCore.ShardExecutionOptions Shard1 { get; internal set; }
        public ShardCore.ShardExecutionOptions Shard2 { get; internal set; }

        /// <summary>
        /// Parses the T-SQL statement
        /// </summary>
        private void Parse()
        {
            // Analyze the first part of the SQL statement for any client-side processing
            DistributedStatement1 = new DistributedStatement();

            #region Extract part 1 of the client-side statement, if any
            DistributedStatement1.Parse(_sql);
            IsDistributed = DistributedStatement1.IsMatch(_sql);

            string sqlRemaining = _sql;
            if (IsDistributed) sqlRemaining = DistributedStatement1.RemainingSQL;
            #endregion

            if (IsDistributed)
            {
                #region Extract Shard 1 information, if any
                Shard1 = null;
                ClientProcessingShardExtraction se = new ClientProcessingShardExtraction();
                se.Parse(sqlRemaining);

                if (se.Mode != ShardCore.ShardOperationMode.UNKNOWN)
                {
                    Shard1 = new ShardCore.ShardExecutionOptions();
                    Shard1.MemberValue = se.MemberValue;
                    Shard1.ShardName = se.ShardName;
                    Shard1.Mode = se.Mode;
                    Shard1.ShardKey = se.MemberKey;
                }
                #endregion

                sqlRemaining = se.RemainingSQL;

                if (sqlRemaining != null && sqlRemaining != "")
                {
                    ClientProcessingWHERE cpwhere = new ClientProcessingWHERE();
                    cpwhere.Parse(sqlRemaining);
                    WHEREOperation = cpwhere;
                    
                    if (cpwhere.HasWhere)
                    {
                        sqlRemaining = cpwhere.RemainingSQL;

                        DistributedStatement2 = new DistributedStatement();
                        DistributedStatement2.DistributedSQL = cpwhere.DistributedSQL;

                        ClientProcessingShardExtraction se2 = new ClientProcessingShardExtraction();
                        se2.Parse(sqlRemaining);

                        sqlRemaining = se2.RemainingSQL;

                        if (se2.Mode != ShardCore.ShardOperationMode.UNKNOWN)
                        {
                            Shard2 = new ShardCore.ShardExecutionOptions();
                            Shard2.MemberValue = se2.MemberValue;
                            Shard2.ShardName = se2.ShardName;
                            Shard2.Mode = se2.Mode;
                            Shard2.ShardKey = se2.MemberKey;
                        }
                    }

                    // Extract ORDER BY operation, if any
                    ClientProcessingORDERBY orderby = new ClientProcessingORDERBY();
                    //orderby.Parse((cpwhere.HasWhere) ? cpwhere.RemainingSQL : se.RemainingSQL);
                    orderby.Parse(sqlRemaining);
                    ORDEROperation = orderby;
                    sqlRemaining = orderby.RemainingSQL;

                    // Extract CACHE hint if any
                    ClientProcessingCACHE cache = new ClientProcessingCACHE();
                    cache.Parse(sqlRemaining);
                    CACHEOperation = cache;

                }
            
            }

        }

        /// <summary>
        /// Evaluates records in the first table from records in the second table, applying the desired WHERE condition
        /// </summary>
        /// <param name="res1">The table with the primary list of records</param>
        /// <param name="res2">The table containing the data to include or exclude</param>
        /// <param name="where">The operation to perform</param>
        /// <returns></returns>
        private DataTable ReduceTables(DataTable res1, DataTable res2, ClientProcessingWHERE where)
        {
            if (res2 != null && where != null && where.HasWhere && where.WhereType != ClientWHEREClauseType.UNKNOWN)
            {

                bool inClause = (where.WhereType == ClientWHEREClauseType.IN);

                if (inClause)
                    res1 = res1.In(where.Field, res2);
                else
                    res1 = res1.NotIn(where.Field, res2);

                res1.AcceptChanges();
            }

            return res1;
        }

        /// <summary>
        /// Processes the result of the distributed operation.
        /// </summary>
        /// <param name="table">The data table for DistributedStatement1</param>
        /// <param name="table2">The data table for DistributedStatement2</param>
        /// <returns></returns>
        public DataTable ProcessResults(DataTable table, DataTable table2)
        {
            // If we have res2!=null we are dealing with a WHERE [NOT] IN condition... process
            if (table2 != null)
                table = ReduceTables(table, table2, WHEREOperation);

            DataTable res = new DataTable();

            if (DistributedStatement1.AggregateColumns.Where(p => p.AggregateOperator != AggregateOperators.NONE).Count() > 0)
            #region create an aggregated resultset
            {
                foreach (AggregateInfo col in DistributedStatement1.AggregateColumns)
                {
                    string aggr = "";

                    switch (col.AggregateOperator)
                    {
                        case AggregateOperators.COUNT: aggr = "count("; break;
                        case AggregateOperators.MIN: aggr = "min("; break;
                        case AggregateOperators.MAX: aggr = "max("; break;
                        case AggregateOperators.AVG: aggr = "avg("; break;
                        case AggregateOperators.NONE: aggr = ""; continue;
                        default: throw new NotImplementedException("Aggregate implementation incomplete");
                    }

                    aggr += col.ColumnName + ")";

                    object aggrValue = table.Compute(aggr, "");

                    string colNameToUse = col.ColumnName;
                    int attempts = 0;
                    while (res.Columns[colNameToUse] != null)
                    {
                        colNameToUse = col.ColumnName + (attempts++).ToString();
                    }

                    Type typeToUse = aggrValue.GetType();

                    res.Columns.Add(new DataColumn(colNameToUse, typeToUse));
                    res.Columns[colNameToUse].DefaultValue = aggrValue;

                }

                // Add a row; this will load a new row with the default values
                res.Rows.Add(new object[] { });
            }
            #endregion
            else
            {
                // Return everything?
                if (!(DistributedStatement1.AggregateColumns.Count == 1 && DistributedStatement1.AggregateColumns[0].ColumnName.Trim() == "*"))
                {
                    // Specific columns then...
                    for (int i = table.Columns.Count - 1; i >= 0; i--)
                    {
                        string colName = table.Columns[i].ColumnName.ToLower().Trim();
                        if (DistributedStatement1.AggregateColumns.Where(p => p.ColumnName.ToLower().Trim() == colName).Count() == 0)
                            table.Columns.Remove(colName);
                    }
                }
                
                res = table;

            }

            // Order the data table?
            if (ORDEROperation != null)
                res.DefaultView.Sort = ORDEROperation.OrderBy;

            if (CACHEOperation != null && CACHEOperation.HasCache)
                _cache.AddOrUpdate(GetKeyForSQL(_sql), res, CACHEOperation.CacheTimeout);

            return res;
        }

    }

    /// <summary>
    /// Internal cache of a Data Table that was previously saved as a result of using the CACHED FOR clause.
    /// </summary>
    internal class CachedTable
    {
        internal CachedTable(DataTable table, int timeout) { Table = table; Timeout = DateTime.Now.AddSeconds(timeout); }
        internal DateTime Timeout { get; set; }
        internal DataTable Table { get; set; }
        internal bool IsExpired { get { return (DateTime.Now > Timeout); } }
    }

    /// <summary>
    /// Class that implements a collection of Cached Data Tables. A timer is used to destroy older items.
    /// </summary>
    internal class CachedTables : Dictionary<string, CachedTable>
    {

        private System.Timers.Timer _expiredTimer = new System.Timers.Timer(1000);

        internal CachedTables() { _expiredTimer.Elapsed += new System.Timers.ElapsedEventHandler(TimeExpired); _expiredTimer.Start(); }

        void TimeExpired(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (this.Count > 0)
                {
                    List<KeyValuePair<string, CachedTable>> cacheItems = this.Where(p => p.Value.IsExpired).ToList();
                    foreach (KeyValuePair<string, CachedTable> item in cacheItems)
                        this.Remove(item.Key);
                }
            }
            catch { }
        }

        internal void AddOrUpdate(string key, DataTable table, int timeout)
        {
            if (this.ContainsKey(key))
                this[key].Timeout = DateTime.Now.AddSeconds(timeout);
            else
                this.Add(key, new CachedTable(table, timeout));
        }

    }

    /// <summary>
    /// List of filter clauses allowed by the distributed query
    /// </summary>
    public enum ClientWHEREClauseType
    {
        UNKNOWN = 0,
        IN = 1,
        NOT_IN = 2,
    }

    /// <summary>
    /// Class used to manage the CACHED FOR clause
    /// </summary>
    public class ClientProcessingCACHE : IDistributedStatement
    {
        private string rxCache = @"\s*cached\s+for\s+(.*)";

        public void Parse(string sql)
        {
            OriginalSQL = sql;
            RemainingSQL = "";
            DistributedSQL = "";

            Match match = Regex.Match(sql, rxCache, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            HasCache = match.Success;

            if (HasCache)
            {
                string fieldTmp = match.Groups[1].Value.Trim();
                fieldTmp = fieldTmp.Replace(";", "");
                int val = 0;
                int.TryParse(fieldTmp, out val);
                CacheTimeout = val;
            }

        }

        public string OriginalSQL { get; private set; }
        public string RemainingSQL { get; private set; }
        public string DistributedSQL { get; private set; }
        public bool HasCache { get; private set; }
        public int CacheTimeout { get; private set; }
    }

    /// <summary>
    /// Class that processes the ORDER BY clause at the tail end of the distributed statement.
    /// </summary>
    public class ClientProcessingORDERBY : IDistributedStatement
    {

        private string rxOrderByWithCache = @"\s*order\s+by\s+(.*)\s+(cached\s+for\s+.+)";
        private string rxOrderBy = @"\s*order\s+by\s+(.*)";

        public void Parse(string sql)
        {
            OriginalSQL = sql;
            RemainingSQL = "";
            DistributedSQL = "";
            bool endOfSql = false;  // is there more after the ORDER BY clause?

            Match matchOrderBy = Regex.Match(sql, rxOrderByWithCache, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            
            if (!matchOrderBy.Success)
            {
                endOfSql = true;
                matchOrderBy = Regex.Match(sql, rxOrderBy, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            }

            HasOrderBy = matchOrderBy.Success;

            if (HasOrderBy)
            {
                string fieldTmp = matchOrderBy.Groups[1].Value.Trim();
                fieldTmp = fieldTmp.Replace(";", "");
                OrderBy = fieldTmp;

                if (!endOfSql)
                    RemainingSQL = matchOrderBy.Groups[2].Value.Trim();

            }
            else
                RemainingSQL = sql;

        }

        public string OriginalSQL { get; private set; }
        public string RemainingSQL { get; private set; }
        public string DistributedSQL { get; private set; }
        public bool HasOrderBy { get; private set; }
        public string OrderBy { get; private set; }

    }

    /// <summary>
    /// Class processing the WHERE clause of a distributed query
    /// </summary>
    public class ClientProcessingWHERE : IDistributedStatement
    {
        private string rxWhere = @"\s*where\s+(.*)\s+(not\s+)?in\s*\((.+?)\)(.*)";
        private string exField = @"\s*(\w+)(\s+not)?\s*";

        public string RemainingSQL { get; private set; }
        public string OriginalSQL { get; private set; }
        internal bool HasWhere { get; private set; }
        public string DistributedSQL { get; private set; }
        internal string Field { get; private set; }
        internal ClientWHEREClauseType WhereType { get; private set; }

        public void Parse(string sql)
        {
            OriginalSQL = sql;
            DistributedSQL = "";
            WhereType = ClientWHEREClauseType.UNKNOWN;

            Match matchWhere = Regex.Match(sql, rxWhere, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            HasWhere = matchWhere.Success;

            if (HasWhere)
            {
                string fieldTmp = matchWhere.Groups[1].Value.Trim();
                Match matchField = Regex.Match(fieldTmp, exField, RegexOptions.IgnoreCase | RegexOptions.Singleline);

                Field = matchField.Groups[1].Value.Trim();
                WhereType = ClientWHEREClauseType.IN;
                if (matchField.Groups[2].Value.Trim().ToLower() == "not") WhereType = ClientWHEREClauseType.NOT_IN;

                DistributedSQL = matchWhere.Groups[3].Value.Trim();
                RemainingSQL = matchWhere.Groups[4].Value.Trim();
            }
            else
                RemainingSQL = sql;
        }

    }

    /// <summary>
    /// Represents a federated section of an SQL statement. This class provides enough
    /// information to build a ShardExecutionOption based on a T-SQL.
    /// </summary>
    public class ClientProcessingShardExtraction : IDistributedStatement
    {
        // Match:  [FEDERATED ON {ROOT, (fed_name [, member_key = member_value [, FILTERED]]) }]

        public string rxRoot = @"\s*federated\s*on\s*root.*";
        public string rxShard = @"federated\s*on\s*\((.+?)\)(.*)";
        public string rxShardInner = @"(.+),(.+)";
        public string rxMemberKey = @"(.+)=(.+)";
        public string rxRemaining = @"federated\s+on\s+\(.+?\)(.*)";

        internal ShardCore.ShardOperationMode Mode { get; private set; }
        internal string ShardName { get; private set; }
        internal string MemberValue { get; private set; }
        internal string MemberKey { get; set; }

        /// <summary>
        /// Remaining SQL statement to process, if any
        /// </summary>
        public string RemainingSQL { get; private set; }
        public string OriginalSQL { get; private set; }
        public string DistributedSQL { get { return ""; } }

        /// <summary>
        /// Parses a portion of a client-side SQL statement that starts with FEDERATED ON...
        /// </summary>
        /// <param name="sql">Portion of SQL statement to analyze.</param>
        public void Parse(string sql)
        {
            OriginalSQL = sql;
            Mode = ShardCore.ShardOperationMode.UNKNOWN;

            if (Regex.IsMatch(sql, rxRoot, RegexOptions.IgnoreCase | RegexOptions.Singleline))
                Mode = ShardCore.ShardOperationMode.ROOT;

            else if (Regex.IsMatch(sql, rxShard, RegexOptions.IgnoreCase | RegexOptions.Singleline))
            {
                // Extract the SQL fragment within the parenthesis
                Match match = Regex.Match(sql, rxShard, RegexOptions.IgnoreCase | RegexOptions.Singleline);

                // Find out if there is a least one comma
                Match matchInner = Regex.Match(match.Groups[1].Value, rxShardInner, RegexOptions.IgnoreCase | RegexOptions.Singleline);

                if (matchInner.Success)
                {

                    Mode = ShardCore.ShardOperationMode.FILTERING_OFF;

                    if (matchInner.Groups[2].Value.ToLower().Trim().StartsWith("filtered"))
                        Mode = ShardCore.ShardOperationMode.FILTERING_ON;

                    Match matchInner2 = Regex.Match(matchInner.Groups[1].Value, rxShardInner, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    string memberKey = "";
                    
                    if (matchInner2.Success)
                    {
                        ShardName = matchInner2.Groups[1].Value.Trim();
                        memberKey = matchInner2.Groups[2].Value.Trim();
                    }
                    else
                    {
                        int lastParenthesis = matchInner.Groups[2].Value.Trim().IndexOf(")");
                        ShardName = matchInner.Groups[1].Value;
                        if (lastParenthesis > 0)
                            memberKey = matchInner.Groups[2].Value.Trim().Substring(0, lastParenthesis);
                        else
                            memberKey = matchInner.Groups[2].Value.Trim();
                    }

                    Match matchKey = Regex.Match(memberKey, rxMemberKey, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    MemberValue = matchKey.Groups[2].Value.Trim();
                    MemberKey = matchKey.Groups[1].Value.Trim();
                }
                else
                {
                    int lastParenthesis = match.Groups[1].Value.Trim().IndexOf(")");
                    Mode = ShardCore.ShardOperationMode.FAN_OUT;
                    if (lastParenthesis > 0)
                        ShardName = match.Groups[1].Value.Trim().Substring(0, lastParenthesis);
                    else
                        ShardName = match.Groups[1].Value.Trim();

                }

                RemainingSQL = "";
                Match matchRemaining = Regex.Match(sql, rxRemaining, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (matchRemaining.Success)
                    RemainingSQL = matchRemaining.Groups[1].Value.Trim();

            }
            else
                RemainingSQL = sql;

        }
    }

    /// <summary>
    /// List of supported aggregate operators
    /// </summary>
    public enum AggregateOperators
    {
        NONE = 0,
        MIN = 1,
        MAX = 2,
        COUNT = 3,
        AVG = 4,
    }

    /// <summary>
    /// Class that stores a specific aggregator for a specific column
    /// </summary>
    public class AggregateInfo
    {
        public string ColumnName { get; set; }
        public AggregateOperators AggregateOperator { get; set; }
    }

    /// <summary>
    /// Class representing a distributed SQL statement. The instance of this class
    /// can represent a subset of a distributed SQL statement if it is part of
    /// a multi-part distributed SQL statement.
    /// </summary>
    public class DistributedStatement : IDistributedStatement
    {
        public readonly string Indicator = "USING";
        public string RegExMatch = @"\s*select\s*.+\busing\b\s*\(.+\).*";
        public string RegExMatchHasWHERE = @"\s*select\s*.+\busing\b\s*\(.+\)\s*\bwhere\b.*";
        public string RegExMatchHasNOT_IN = @"\s*select\s*.+\busing\b\s*\(.+\)\s*\bwhere\b\s*\w\s*not\s*in\s*\(.*\).*";
        public string RegExSplit = @"(\s*.+\busing\b\s*\(\s*)";
        public string RegExSplitSpace = @"(\b)";

        public void Parse(string sql)
        {
            OriginalSQL = sql;

            if (IsMatch(sql))
            {
                Regex rg = new Regex(RegExSplit, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                string[] splitString = rg.Split(sql, 2);

                foreach (string part in splitString)
                {
                    if (part.Trim() == "") continue;

                    if (ClientSQL == null || ClientSQL == "")
                    {
                        ClientSQL = part;
                        // remove the OVER clause
                        ClientSQL = ClientSQL.Substring(0, ClientSQL.LastIndexOf(Indicator, StringComparison.CurrentCultureIgnoreCase));

                        BuildAggregateColumns();

                    }
                    else
                    {
                        DistributedSQL = part;
                        // remove the last ")"

                        int pos = 0;
                        int depth = 1;
                        while (depth != 0)
                        {
                            pos++;
                            if (part[pos] == '(') depth++;
                            else if (part[pos] == ')') depth--;
                        }

                        DistributedSQL = DistributedSQL.Substring(0, pos);

                        if (part.Trim().Length > pos + 1)
                            RemainingSQL = part.Substring(pos + 1);
                        else
                            RemainingSQL = "";
                        DistributedSQL = DistributedSQL.Trim();

                    }
                }
            }
            else
            {
                DistributedSQL = sql;
                ClientSQL = "";
            }
        }

        /// <summary>
        /// Evaluates the aggregate SQL statement and builds the internal memory structure
        /// representing the aggregate calculations.
        /// </summary>
        private void BuildAggregateColumns()
        {
            AggregateColumns = new List<AggregateInfo>();

            if (ClientSQL.Trim() != "")
            {
                // This is how the statement looks like: SELECT [TOP N] fields
                if (ClientSQL.Trim().ToLower().StartsWith("select"))
                {
                    
                    AggregateOperators op = AggregateOperators.AVG;
                    string field = "";
                    bool readingField = false;

                    string[] parts = Regex.Split(ClientSQL, RegExSplitSpace);

                    foreach (string part in parts)
                    {
                        if (part == null || part.Trim().Length == 0) continue;
                        if (part.Trim().ToLower() == "select") continue;
                        if (part.Trim().ToLower().StartsWith("top") && !readingField) continue;
                        decimal topN = 0;
                        if (decimal.TryParse(part.Trim(), out topN))
                        {
                            continue;
                        }

                        // We are dealing with a column...
                        // Format is:  AGGR(fieldname)

                        if (part.Trim().ToLower() == "(") { readingField = true; continue; };
                        if (part.Trim().ToLower().StartsWith(")")) {readingField = false; continue;}

                        if (!readingField)
                        {
                            if (part.ToLower().Equals("min")) { op = AggregateOperators.MIN; continue; }
                            if (part.ToLower().Equals("max")) { op = AggregateOperators.MAX; continue; }
                            if (part.ToLower().Equals("avg")) { op = AggregateOperators.AVG; continue; }
                            if (part.ToLower().Equals("count")) { op = AggregateOperators.COUNT; continue; }
                            if (part.Trim().ToLower() == ",") continue;

                            // the field we are reading is then not an aggregate... 
                            // this means there are no aggregates in this statement, or that
                            // a field does not have an aggregate...

                            if (AggregateColumns.Where(p => p.AggregateOperator != AggregateOperators.NONE).Count() > 0)
                                throw new Exception("If used, aggregates must be defined on all fields");

                            AggregateInfo info = new AggregateInfo();
                            info.AggregateOperator = AggregateOperators.NONE;
                            info.ColumnName = part.Trim();
                            AggregateColumns.Add(info);

                            continue;

                        }

                        
                        if (!readingField)
                            throw new Exception("Unsupported aggregate: " + part);

                        // Now extract the field

                        field = part.Trim();

                        AggregateInfo info2 = new AggregateInfo();
                        info2.AggregateOperator = op;
                        info2.ColumnName = field;
                        AggregateColumns.Add(info2);

                    }
                }
            }
        }

        public List<AggregateInfo> AggregateColumns { get; private set; }

        public bool IsMatch(string sql)
        {
            // MUST MATCH:
            //  SELECT .... USING ( SELECT ... ) 
            Regex rg = new Regex(RegExMatch, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return rg.IsMatch(sql);
        }

        /// <summary>
        /// The client-side statement
        /// </summary>
        public string ClientSQL { get; set; }

        /// <summary>
        /// The SQL statement that the database engine should execute
        /// </summary>
        public string DistributedSQL { get; internal set; }
        public string OriginalSQL { get; internal set; }
        public string RemainingSQL { get; internal set; }

        private DataTable ProcessTable(DataTable table)
        {
            DataTable res = new DataTable();

            if (AggregateColumns.Where(p => p.AggregateOperator != AggregateOperators.NONE).Count() > 0)
            #region create an aggregated resultset
            {
                foreach (AggregateInfo col in AggregateColumns)
                {
                    string aggr = "";

                    switch (col.AggregateOperator)
                    {
                        case AggregateOperators.COUNT: aggr = "count("; break;
                        case AggregateOperators.MIN: aggr = "min("; break;
                        case AggregateOperators.MAX: aggr = "max("; break;
                        case AggregateOperators.AVG: aggr = "avg("; break;
                        case AggregateOperators.NONE: aggr = ""; continue;
                        default: throw new NotImplementedException("Aggregate implementation incomplete");
                    }

                    aggr += col.ColumnName + ")";

                    object aggrValue = table.Compute(aggr, "");

                    string colNameToUse = col.ColumnName;
                    int attempts = 0;
                    while (res.Columns[colNameToUse] != null)
                    {
                        colNameToUse = col.ColumnName + (attempts++).ToString();
                    }

                    Type typeToUse = aggrValue.GetType();

                    res.Columns.Add(new DataColumn(colNameToUse, typeToUse));
                    res.Columns[colNameToUse].DefaultValue = aggrValue;

                }

                // Add a row; this will load a new row with the default values
                res.Rows.Add(new object[] { });
            }
            #endregion
            else
            {
                // Return everything?
                if (AggregateColumns.Count == 1 && AggregateColumns[0].ColumnName.Trim() == "*")
                    return table;

                // Specific columns?
                for (int i = table.Columns.Count - 1; i >= 0 ; i--)
                {
                    string colName = table.Columns[i].ColumnName.ToLower().Trim();
                    if (AggregateColumns.Where(p => p.ColumnName.ToLower().Trim() == colName).Count() == 0)
                        table.Columns.Remove(colName);
                }

                return table;

            }

            return res;
        }


    }

    /// <summary>
    /// Common distributed SQL Interface
    /// </summary>
    public interface IDistributedStatement
    {
        void Parse(string sql);
        string OriginalSQL { get; }
        string RemainingSQL { get; }
        string DistributedSQL { get; }
    }

}

