/*
 * Overview
 * ********
 * 
 * This class is intended to become a centralized configuration point. For the moment it contains a few constants 
 * and classes used by the original expanded shard strategy implementation.
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
using System.Data.SqlClient;

namespace PYN.EnzoAzureLib
{
    public class SharedSettings
    {
        /// <summary>
        /// The column name for the database guid
        /// </summary>
        public const string _TENANTKEY_ = "__tenantkey__";
        public const string _GUIDROW_ = "__guidROW__";


    }

    /// <summary>
    /// Query options to be applied against the aggegrated result set. Used by the original shard library.
    /// </summary>
    public struct QueryOptions
    {
        public QueryOptions(int top, string orderBy)
        {
            Top = top;
            OrderBy = orderBy;
        }
        public int Top;
        public string OrderBy;
    }


    public static class SharedExtensions
    {

        /// <summary>
        /// Calculates a GUID for a connection string, based on its connection string
        /// </summary>
        public static string ConnectionGuid(this SqlConnection connection)
        {
            // REF #005
            SqlConnectionStringBuilder cb = new SqlConnectionStringBuilder(connection.ConnectionString);
            string connUID =
                ((cb.UserID != null) ? cb.UserID : "SSPI") + "#" +
                cb.DataSource + "#" +
                ((cb.InitialCatalog != null) ? cb.InitialCatalog : "master");
            string connHash = connUID.GetHash().ToString();
            return connHash;
        }
    }

}
