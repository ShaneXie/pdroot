using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

using PYN.EnzoAzureLib.CompressedShard;
using PYN.EnzoAzureLib;
using System.Data;

namespace payDevnew
{
    public partial class About : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            
        }

        public DataTable quertResult(string query)
        {
            string connStr = "Server=localhost;Database=theRoot;User Id=sa;Password=Thisway1;";
            RootDatabase root_db = null;
            DataTable data = null;

            root_db = new RootDatabase(connStr);

            root_db.Tenants.DefaultExecutionContext.Mode = ShardCore.ShardOperationMode.FAN_OUT;

            data = root_db.Tenants.ExecuteDataTable(query);

            Response.Write("<table Class=\".table table-striped table-bordered\"><thead><tr>");

            foreach (DataColumn col in data.Columns)
            {
                Response.Write("<th>"+col.ColumnName+"</th>");
            }

            Response.Write("</tr></thead><tbody>");

            foreach (DataRow row in data.Rows)
            {
                Response.Write("<tr class=\".warning\">");
                foreach (var cell in row.ItemArray)
                {
                    Response.Write("<td>"+cell.ToString()+"</td>");
                }
                Response.Write("</tr>");
            }

            Response.Write("</tbody>");

            return data;
        }
    }
}