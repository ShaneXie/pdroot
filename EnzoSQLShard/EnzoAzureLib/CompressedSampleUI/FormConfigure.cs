using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CompressedSampleUI
{
    public partial class FormConfigure : Form
    {
        public FormConfigure()
        {
            InitializeComponent();

            #region Pre-load settings

            // Default settings...
            //textBoxServer.Text = @"localhost\SQLServer2012";
            textBoxServer.Text = @"localhost";
            textBoxUID.Text = "";
            textBoxPWD.Text = "";
            textBoxDB.Text = "rootdb";

            // Now check if there is a rootdb connection string defined...
            if (System.Configuration.ConfigurationManager.ConnectionStrings["rootdb"] != null &&
                System.Configuration.ConfigurationManager.ConnectionStrings["rootdb"].ConnectionString != "")
            {
                System.Data.SqlClient.SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(System.Configuration.ConfigurationManager.ConnectionStrings["rootdb"].ConnectionString);
                textBoxServer.Text = csb.DataSource;
                textBoxDB.Text = csb.InitialCatalog;
                textBoxUID.Text = "";
                textBoxPWD.Text = "";
                if (!csb.IntegratedSecurity)
                {
                    textBoxUID.Text = csb.UserID;
                    textBoxPWD.Text = csb.Password;
                }
            }

            #endregion
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {

                SqlConnection sqlConn = new SqlConnection(RootConnectionString);
                sqlConn.Open();
                sqlConn.Close();
                //System.Windows.Forms.MessageBox.Show(this, "Connection was successful", "Success", MessageBoxButtons.OK);

                groupBox2.Enabled = true;
                buttonCreateConfig.Enabled = true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK);
                groupBox2.Enabled = false;
                buttonCreateConfig.Enabled = false;
            }

            try
            {
                RefreshTenants();
            }
            catch { }

        }

        internal string RootConnectionString
        {
            get
            {
                SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
                if (textBoxUID.Text.Trim() == "")
                    csb.IntegratedSecurity = true;
                else
                {
                    csb.IntegratedSecurity = false;
                    csb.UserID = textBoxUID.Text;
                    csb.Password = textBoxPWD.Text;
                }
                csb.InitialCatalog = textBoxDB.Text;
                csb.DataSource = textBoxServer.Text;
                string conn = csb.ConnectionString;
                return conn;
            }
        }

        internal string TenantConnectionString
        {
            get
            {
                SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
                if (textBoxTenantUID.Text.Trim() == "")
                    csb.IntegratedSecurity = true;
                else
                {
                    csb.IntegratedSecurity = false;
                    csb.UserID = textBoxTenantUID.Text;
                    csb.Password = textBoxTenantPWD.Text;
                }
                csb.InitialCatalog = textBoxTenantDB.Text;
                csb.DataSource = textBoxTenantServer.Text;
                string conn = csb.ConnectionString;
                return conn;
            }
        }

        private void buttonCreateConfig_Click(object sender, EventArgs e)
        {
            #region Root Configuration T-SQL Creation
            string sql = @"
CREATE TABLE serversdef
( id int IDENTITY(1,1) primary key,
  servername nvarchar(255) NOT NULL,
  port int,
  uid nvarchar(50),
  pwd varbinary(50),
  enabled bit)

CREATE TABLE TenantsDef
( id int IDENTITY(1,1) primary key,
  serverId int NOT NULL,
  databasename nvarchar(50) NOT NULL,
  tenantKey nvarchar(50) NOT NULL,
  uid nvarchar(100),
  pwd varbinary(50),
  enabled bit)

CREATE UNIQUE INDEX IDX1 ON TenantsDef (tenantKey)
CREATE UNIQUE INDEX IDX2 ON TenantsDef (serverId, databasename, uid)

";
            #endregion

            try
            {
                if (System.Windows.Forms.MessageBox.Show(this, "You are about to create the Enzo system tables in the master database identified above. The database must already exist. If the tables already exist, this call will fail. Proceed? ", "Create System Tables", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {
                    SqlConnection conn = new SqlConnection(RootConnectionString);
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK);
            }
        }

        private void linkLabelRefresh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RefreshTenants();
        }

        private void RefreshTenants()
        {
            #region SQL
            string sql = @"
SELECT 
    SD.id as serverid,
    servername, 
    port, 
    TD.uid, 
    TD.pwd, 
    TD.id as databaseid, 
    TD.databasename,
    TD.TenantKey ,
    TD.Enabled 
FROM 
    serversdef SD JOIN TenantsDef TD ON SD.id = TD.serverid 
ORDER BY
    SD.id, TD.id";
            #endregion
            try
            {
                listViewTenants.Items.Clear();
                ClearInputArea();

                SqlConnection conn = new SqlConnection(RootConnectionString);
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = null;
                conn.Open();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dr.Read())
                {
                    ListViewItem item = new ListViewItem(dr["servername"].ToString());
                    item.SubItems.Add(dr["databasename"].ToString());
                    item.SubItems.Add(dr["uid"].ToString());
                    item.SubItems.Add(dr["tenantkey"].ToString());
                    item.SubItems.Add((bool.Parse(dr["enabled"].ToString()) ? "Yes" : "No"));
                    listViewTenants.Items.Add(item);
                }

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK);
            }
        }

        private void buttonTryConnection_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection sqlConn = new SqlConnection(TenantConnectionString);
                sqlConn.Open();
                sqlConn.Close();
                System.Windows.Forms.MessageBox.Show(this, "Connection was successful", "Success", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// Returns a SQL Server id given its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private int GetServerId()
        {
            int res = -1;
            string sql = @"SELECT id FROM serversdef WHERE servername = '{0}'";
            SqlConnection sqlConn = new SqlConnection(RootConnectionString);
            sqlConn.Open();
            sql = string.Format(sql, textBoxTenantServer.Text);
            SqlCommand cmd0 = new SqlCommand(sql, sqlConn);
            SqlDataReader dr = cmd0.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                res = int.Parse(dr["id"].ToString());
                sqlConn.Close();
            }
            return res;

        }

        /// <summary>
        /// Create a new server based on the information provided on the screen
        /// </summary>
        private void CreateServer()
        {
            string sql = @"INSERT INTO serversdef VALUES('{0}', {1}, '{2}', cast('{3}' as varbinary(50)), 1)";
            SqlConnection sqlConn = new SqlConnection(RootConnectionString);
            sqlConn.Open();
            sql = string.Format(sql, textBoxTenantServer.Text, 1433, textBoxTenantUID.Text, textBoxTenantPWD.Text);
            SqlCommand cmd0 = new SqlCommand(sql, sqlConn);
            cmd0.ExecuteNonQuery();
            sqlConn.Close();
        }

        /// <summary>
        /// Retrieve the current server or create a new server based on the information on the screen, and return the ID of the server
        /// </summary>
        /// <returns></returns>
        private int CreateOrGetServer()
        {
            int serverId = GetServerId();

            if (serverId == -1)
            {
                CreateServer();
                serverId = GetServerId();
            }

            return serverId;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            
            string sql = @"
IF(Exists(SELECT * FROM tenantsdef WHERE id = {0}))
  UPDATE tenantsdef SET serverId = {1}, databaseName = '{2}', tenantKey = '{3}', uid = '{4}', pwd = cast('{5}' as varbinary(50)), enabled = {6} WHERE id = {0}
ELSE
  INSERT INTO tenantsdef VALUES ({1}, '{2}', '{3}', '{4}', cast('{5}' as varbinary(50)), {6})
";

            try
            {
                int serverId = CreateOrGetServer();
                int databaseId = int.Parse(labelID.Text);

                SqlConnection sqlConn = new SqlConnection(RootConnectionString);
                sqlConn.Open();

                sql = string.Format(sql,databaseId, serverId.ToString(), textBoxTenantDB.Text, textBoxCustomerKey.Text, textBoxTenantUID.Text, textBoxTenantPWD.Text, ((checkBoxTenantEnabled.Checked) ? "1" : "0") );
                SqlCommand cmd1 = new SqlCommand(sql, sqlConn);
                cmd1.ExecuteNonQuery();
                sqlConn.Close();

                RefreshTenants();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK);
            }
        }

        private void ClearInputArea()
        {
            textBoxTenantDB.Text = "";
            textBoxTenantPWD.Text = "";
            textBoxTenantServer.Text = "";
            textBoxTenantUID.Text = "";
            textBoxCustomerKey.Text = "";
            checkBoxTenantEnabled.Checked = false;
            labelID.Text = "-1";
        }

        private void listViewTenants_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearInputArea();

            linkLabelRemove.Enabled = (listViewTenants.SelectedItems.Count > 0);

            #region SQL
            string sql = @"
SELECT 
    SD.id as serverid,
    servername, 
    port, 
    TD.uid, 
    TD.pwd, 
    TD.id as databaseid, 
    TD.databasename,
    TD.TenantKey,
    TD.Enabled  
FROM 
    serversdef SD JOIN TenantsDef TD ON SD.id = TD.serverid 
WHERE
    TD.TenantKey = '{0}'
    ";
            #endregion
            try
            {
                if (listViewTenants.SelectedItems.Count == 0) return;

                string key = listViewTenants.SelectedItems[0].SubItems[3].Text;
                SqlConnection conn = new SqlConnection(RootConnectionString);
                sql = string.Format(sql, key);
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = null;
                conn.Open();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                dr.Read();
                if (dr.HasRows)
                {
                    textBoxTenantServer.Text = dr["servername"].ToString();
                    textBoxTenantDB.Text = dr["databasename"].ToString();
                    textBoxTenantUID.Text = dr["uid"].ToString();
                    textBoxTenantPWD.Text = System.Text.UTF8Encoding.UTF8.GetString((byte[])dr["pwd"]);
                    textBoxCustomerKey.Text = dr["tenantkey"].ToString();
                    labelID.Text = dr["databaseid"].ToString();
                    checkBoxTenantEnabled.Checked = bool.Parse(dr["enabled"].ToString());
                }

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK);
            }


        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabelRemove_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            if (listViewTenants.SelectedItems.Count == 0) return;

            if (System.Windows.Forms.MessageBox.Show(this, "You are about to delete the selected Tenant definition (the database will not be dropped). Continue?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                return;

            try
            {

                #region SQL
                string sql = @"
DELETE FROM TenantsDef 
WHERE
    id = {0}
    ";
                #endregion
                try
                {
                    string key = labelID.Text;
                    SqlConnection conn = new SqlConnection(RootConnectionString);
                    sql = string.Format(sql, key);
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    RefreshTenants();
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK);
                }

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK);
            }
        }


        private void linkLabelNew_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (listViewTenants.SelectedItems.Count > 0)
            {
                listViewTenants.SelectedItems[0].Selected = false;
            }
        }

        private void buttonCreateTestHistory_Click(object sender, EventArgs e)
        {
            try
            {
                if (listViewTenants.SelectedItems.Count == 0) return;

                if (System.Windows.Forms.MessageBox.Show(this, "You are about to drop/create a test HISTORY table in this database/tenant. Continue?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                    return;

                try
                {

                    #region SQL
                    string sql = @"
IF (EXISTS(SELECT * FROM sys.tables WHERE name = 'history' AND schema_id = 1))
    DROP TABLE dbo.history
CREATE TABLE dbo.history (id int identity(1,1) primary key, username nvarchar(10), amount money, dateadded datetime)
INSERT INTO dbo.history values ('user-{0}', 100.00, getdate())
    ";
                    #endregion
                    try
                    {
                        string key = labelID.Text;
                        SqlConnection conn = new SqlConnection(TenantConnectionString);
                        sql = string.Format(sql, key);
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK);
                    }

                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK);
                }
            }
            catch { }
        }

    }
}
