/*
 * Overview
 * ********
 * 
 * Utility class providing a few extension methods and shared logic.
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
using System.Threading;
using System.Data.SqlClient;

namespace PYN.EnzoAzureLib
{
    /// <summary>
    /// Basic Extension Methods
    /// </summary>
    public static class PYNExtensions
    {
        /// <summary>
        /// Calculates a SHA256 value of a string
        /// </summary>
        public static string GetHash(this string val)
        {
            byte[] buf = System.Text.UTF8Encoding.UTF8.GetBytes(val);
            byte[] res = System.Security.Cryptography.SHA256.Create().ComputeHash(buf);
            return BitConverter.ToString(res).Replace("-", "");
        }
    }

    public static class ShardExtensions
    {

        private const int MAX_ATTEMPTS = 3;
        private const int SLEEP_TIME = 3000;

        /// <summary>
        /// Marks all the rows that are found in singleColumnTable with status of Deleted. 
        /// Does not change the status of other rows.
        /// Call AcceptChanges to make the selection final.
        /// </summary>
        /// <param name="data">The source table</param>
        /// <param name="singleColumnTable">A single column table (uses row[0])</param>
        /// <param name="fieldName">The field name in the source table to filter with</param>
        /// <returns></returns>
        public static DataTable NotIn(this DataTable data, string fieldName, DataTable singleColumnTable)
        {
            foreach (DataRow row in singleColumnTable.Rows)
            {
                DataRow[] rows = data.Select(fieldName + " = " + row[0]);
                foreach (DataRow r in rows)
                    data.Rows.Remove(r);
            }
            return data;
        }

        /// <summary>
        /// Marks all the rows that are not found in singleColumnTable with status of Deleted. 
        /// Does not change the status of other rows.
        /// Call AcceptChanges to make the selection final.
        /// </summary>
        /// <param name="data">The source table</param>
        /// <param name="singleColumnTable">A single column table (uses row[0])</param>
        /// <param name="fieldName">The field name in the source table to filter with</param>
        /// <returns></returns>
        public static DataTable In(this DataTable data, string fieldName, DataTable singleColumnTable)
        {
            string singleTableColName = singleColumnTable.Columns[0].ColumnName;
            foreach (DataRow row in data.Rows)
            {
                if (singleColumnTable.Select(singleTableColName + " = " + row[fieldName]).Count() == 0)
                    row.Delete();
            }
            return data;
        }


        /// <summary>
        /// Attempts to open a database connection and retries using an exponential backoff approach
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IDbConnection TryOpen(this IDbConnection connection)
        {
            int attempts = 0;

            if (connection.State == ConnectionState.Open) return connection;

            while (attempts < MAX_ATTEMPTS)
            {
                try
                {
                    if (attempts > 0)
                        Thread.Sleep(SLEEP_TIME * attempts);
                    connection.Open();
                    return connection;
                }
                catch (Exception ex)
                {
                    if (!IsTransientError(ex))
                        throw;
                }
                attempts++;
            }
            throw new Exception("Unable to obtain a connection to SQL Server or SQL Azure.");
        }

        /// <summary>
        /// Attempts to execute a statement against SQL Azure or SQL Server and handles automatic retries
        /// </summary>
        /// <param name="command">The SqlCommand to use</param>
        /// <returns></returns>
        public static int TryExecuteNonQuery(this IDbCommand command)
        {
            int attempts = 0;
            string err = "";

            while (attempts < MAX_ATTEMPTS)
            {
                try
                {
                    if (attempts > 0)
                        Thread.Sleep(SLEEP_TIME * attempts);
                    int res = command.ExecuteNonQuery();
                    return res;
                }
                catch (Exception ex)
                {
                    if (!IsTransientError(ex))
                        throw;
                    err = ex.GetBaseException().Message;
                }
                attempts++;
            }
            throw new Exception("Unable to fetch records from SQL Server or SQL Azure. Last Error: " + err);
        }

        /// <summary>
        /// Attempts to fill a Data Table using a DataAdapter and handles automatic retries
        /// </summary>
        /// <param name="da">The data adapter to use</param>
        /// <param name="table">The data table to fill</param>
        /// <returns></returns>
        public static int TryFill(SqlDataAdapter da, DataTable table)
        {

            int attempts = 0;
            string err = "";
            //SqlException lastEx = null;

            while (attempts < MAX_ATTEMPTS)
            {
                try
                {
                    if (attempts > 0)
                        Thread.Sleep(SLEEP_TIME * attempts);

                    if (da.SelectCommand.Connection.State == ConnectionState.Closed)
                        da.SelectCommand.Connection.Open();

                    int res = da.Fill(table);

                    return res;
                }
                catch (Exception ex)
                {
                    if (!IsTransientError(ex))
                        throw;
                    err = ex.GetBaseException().Message;
                }

                attempts++;
            }

            throw new Exception("Unable to fetch records from SQL Server or SQL Azure. Last Error: " + err);

        }

        /// <summary>
        /// Attempts to return a Data Reader on an existing connection and a command object.
        /// </summary>
        /// <param name="conn">The database connection</param>
        /// <param name="cmd">The command object</param>
        /// <returns></returns>
        public static SqlDataReader TryDataReader(SqlConnection conn, SqlCommand cmd)
        {

            int attempts = 0;
            string err = "";

            if (conn.State == ConnectionState.Closed)
                conn = (SqlConnection)conn.TryOpen();
            if (cmd.Connection == null)
                cmd.Connection = conn;

            while (attempts <= MAX_ATTEMPTS)
            {
                try
                {
                    if (attempts > 0)
                        System.Threading.Thread.Sleep(SLEEP_TIME * attempts);

                    SqlDataReader dr = cmd.ExecuteReader();
                    return dr;
                }
                catch (Exception ex)
                {
                    if (!IsTransientError(ex))
                        throw;
                    err = ex.GetBaseException().Message;
                }

                attempts++;
            }

            if (System.Environment.UserInteractive) System.Console.WriteLine("  WAIT:DB:0x05 (failed)");
            throw new Exception("Unable to fetch records from SQL Server or SQL Azure. Last Error: " + err);

        }

        /// <summary>
        /// Returns true if an exception is transient
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsTransientError(Exception ex)
        {
            if (ex is SqlException)
                return IsTransientError((SqlException)ex);
            if (ex.InnerException is SqlException)
                return IsTransientError((SqlException)ex.InnerException);
            else if (ex is InvalidOperationException && ex.InnerException != null && ex.InnerException.Message.ToLower().StartsWith("timeout expired"))
                return true;
            else if (ex is System.Net.Sockets.SocketException)
            {
                if (((System.Net.Sockets.SocketException)ex).ErrorCode == 10054)
                    return true;
            }
            else if (ex is TimeoutException)
                return true;
            return false;
        }

        /// <summary>
        /// Returns true if a SqlException is transient
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static bool IsTransientError(SqlException ex)
        {
            if (ex.Number == 40197 ||
                ex.Number == 40501 ||
                ex.Number == 10053 ||
                ex.Number == 10054 ||
                ex.Number == 10060 ||
                ex.Number == 40613 ||
                ex.Number == 40143 ||
                ex.Number == 233 ||
                ex.Number == 64 ||
                ex.Number == 20 ||
                ex.Number == 40553 ||
                ex.Number == 40552 ||
                ex.Number == 40551 ||
                ex.Number == 40550 ||
                ex.Number == 40549 ||
                ex.Number == 40545 ||
                ex.Number == 40544
                )
                return true;

            return false;
        }
    }
}
