/*
 * Overview
 * ********
 * 
 * This class offers a simple bitmap indexing mechanism. Client code builds a bitmap index
 * by returning a BIT field in the first column of a statement. The bit field indicates if the
 * shard database should be flagged or not (1 or 0). Later, when a statement is executed against
 * shard databases, specifying the index will limit the execution of a statement to the 
 * databases that are flagged in the index.
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
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace PYN.EnzoAzureLib
{

    /// <summary>
    /// Delegate allowing the CacheHitMap to call the shard strategy to execute a well-formed SQL statement to build 
    /// an in-memory index of databases/members containing the expected data.
    /// </summary>
    /// <param name="cmd">A command to execute against a shard.</param>
    /// <param name="options">The shard options to use.</param>
    /// <param name="includeShardId">True to include the shard id as part of the resultset.</param>
    /// <returns></returns>
    public delegate DataTable ShardExecuteDataDelegate(SqlCommand cmd, ShardCore.ShardExecutionOptions options);

    /// <summary>
    /// A collection of indexes used to classify in memory which shard members contain specific records.
    /// </summary>
    public class ShardIndexes : Dictionary<string, ShardIndex>
    {

        public string ShardName { get; internal set; }
        public string ShardKey { get; internal set; }
        internal ShardExecuteDataDelegate ShardExecuteData { get; set; }

        /// <summary>
        /// Constructor of shard indexes.
        /// </summary>
        public ShardIndexes(
            string shardKey, 
            string shardName, 
            ShardExecuteDataDelegate executeDataTableDelegate) 
        { 
            ShardKey = shardKey; 
            ShardName = shardName;
            ShardExecuteData = executeDataTableDelegate;
        }

        /// <summary>
        /// Indicates that all underlying indexes should be marked invalid. They will be rebuilt automatically when needed.
        /// </summary>
        public void InvalidateAll()
        {
            foreach (ShardIndex item in base.Values)
                item.IsInvalid = false;
        }

        /// <summary>
        /// Rebuilds all indexes in parallel.
        /// </summary>
        /// <param name="fastRebuild">True to reduce the map only. Databases/Members that previously did not match will not be reconsidered. Send False to rebuild the entire map.</param>
        public void RebuildAll()
        {
            Parallel.ForEach(Values, delegate(ShardIndex item)  {
                item.Build();
            });
        }

        /// <summary>
        /// Rebuild a specific shard index by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fastRebuild"></param>
        public void Build(string name) { this[name].Build(); }

        /// <summary>
        /// Rebuild a specific shard index by its name, if it is found and if it is currently invalid.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fastRebuild"></param>
        public void BuildIfInvalid(string name) { if (this[name] != null && (this[name].IsInvalid || this[name].HitMap == null)) this[name].Build(); }

        /// <summary>
        /// Adds an index to the list.
        /// </summary>
        /// <param name="key">The unique index name to use</param>
        /// <param name="index">The actual bitmap index </param>
        public new void Add(string key, ShardIndex index)
        {
            index.ShardKey = ShardKey;
            index.ShardName = ShardName;
            index.ShardExecuteData = ShardExecuteData;
            base.Add(key, index);
            this[key].Invalidate();
        }

        /// <summary>
        /// Adds an index definition to the list.
        /// </summary>
        /// <param name="key">The unique index name to use</param>
        /// <param name="indexSQLFilter">The statement to execute against all databases in a shard to build the bitmap index</param>
        public void Add(string key, string indexSQLFilter)
        {
            ShardIndex map = new ShardIndex();
            map.Filter = indexSQLFilter;
            map.ShardKey = ShardKey;
            map.ShardName = ShardName;
            map.ShardExecuteData = ShardExecuteData;
            map.Invalidate();

            if (Keys.Contains(key))
                Remove(key);

            base.Add(key, map);
        }

    }

    /// <summary>
    /// This class represents a specific shard index, which holds a list of shard members' ID and a boolean
    /// indicating whether the shard contains the needed records or not. 
    /// </summary>
    public class ShardIndex
    {
        /// <summary>
        /// The SQL statement used to build the index.
        /// </summary>
        public string Filter { get; set; }
        
        public bool IsCaseSensitive { get; set; }
        public DateTime LastUpdatedOn { get; internal set; }
        public bool IsInvalid { get; internal set; }
        public string ShardName { get; internal set; }
        public string ShardKey { get; internal set; }

        /// <summary>
        /// A function pointer to the method to call when it becomes necessary to build/rebuild an index.
        /// </summary>
        internal ShardExecuteDataDelegate ShardExecuteData { get; set; }

        public ShardIndex() { Invalidate(); }

        /// <summary>
        /// Invalidates the shard index so that it gets rebuilt the next time
        /// </summary>
        public void Invalidate() { IsInvalid = true; }


        /// <summary>
        /// Rebuilds the cache hit map
        /// </summary>
        /// <param name="fastRebuild">When true, only verify the nodes that did not have a positive match previously.</param>
        public void Build() 
        {
            HitMap = new Dictionary<int, bool>();
            BuildInternal();
        }

        /// <summary>
        /// Builds this index based on the information provided in this index.
        /// </summary>
        private void BuildInternal()
        {
            ShardCore.ShardExecutionOptions fedOptions = new ShardCore.ShardExecutionOptions();
            fedOptions.Mode = ShardCore.ShardOperationMode.FAN_OUT;
            fedOptions.ShardName = this.ShardName;
            fedOptions.ShardKey = this.ShardKey;
            fedOptions.MemberValue = null;
            fedOptions.AddTenantKey = true;

            SqlCommand cmdCache = new SqlCommand();
            cmdCache.CommandText = Filter;

            if (ShardExecuteData != null)
            {
                DataTable dtCacheOut = ShardExecuteData.Invoke(cmdCache, fedOptions);

                IsInvalid = false;
                LastUpdatedOn = DateTime.UtcNow;
                HitMap = new Dictionary<int, bool>();

                foreach (DataRow row in dtCacheOut.Rows)
                {
                    bool isTrue = (bool)row[0];
                    int memberId = int.Parse(row[dtCacheOut.Columns.Count - 1].ToString());
                    HitMap.Add(memberId, isTrue);
                }
            }
        }

        /// <summary>
        /// Returns a list of databases by index and a boolean indicating whether a hit was recorded
        /// </summary>
        public Dictionary<int, bool> HitMap { get; internal set; }

    }
}
