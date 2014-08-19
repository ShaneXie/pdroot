using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using PYN.EnzoAzureLib;

namespace ShardSampleUI
{
    public partial class FormAddShardConnection : Form
    {
        private string _schema = "";
        private List<SqlConnection> _connections = null;

        public FormAddShardConnection(string schema, List<SqlConnection> connections)
        {
            InitializeComponent();
            _schema = schema;
            _connections = connections;
        }

        public void SetFields(SqlConnection sqlConn)
        {
            SqlConnectionStringBuilder cb = new SqlConnectionStringBuilder(sqlConn.ConnectionString);
            this.textBoxServer.Text = cb.DataSource;
            this.textBoxUID.Text = cb.UserID;
            this.textBoxPWD.Text = cb.Password;
            this.textBoxDatabaseName.Text = cb.InitialCatalog;

            if (cb.IntegratedSecurity)
                radioButtonSSPI.Checked = true;
            else
                radioButtonSQLAccount.Checked = true;
        }

        private void RefreshGuid()
        {
            labelGuid.Text = new SqlConnection(ConnectionString).ConnectionGuid();
        }

        public string ConnectionString
        {
            get
            {
                string connStr = "Server={0};Database={1};Trusted_Connection={2};";
                string uidpwd = "User ID={0};Password={1}";
                connStr = string.Format(connStr, textBoxServer.Text, textBoxDatabaseName.Text, radioButtonSSPI.Checked.ToString());
                if (!radioButtonSSPI.Checked)
                    connStr += string.Format(uidpwd, textBoxUID.Text, textBoxPWD.Text);
                return connStr;
            }
        }

        private void radioButtonSSPI_CheckedChanged(object sender, EventArgs e)
        {
            this.textBoxUID.Enabled = this.textBoxPWD.Enabled = false;
            RefreshGuid();
        }

        private void radioButtonSQLAccount_CheckedChanged(object sender, EventArgs e)
        {
            this.textBoxUID.Enabled = this.textBoxPWD.Enabled = true;
            RefreshGuid();
        }

        private void textBoxServer_TextChanged(object sender, EventArgs e)
        {
            RefreshGuid();
        }

        private void textBoxDatabaseName_TextChanged(object sender, EventArgs e)
        {
            RefreshGuid();
        }

        private void textBoxUID_TextChanged(object sender, EventArgs e)
        {
            RefreshGuid();
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection sqlConn = new SqlConnection(ConnectionString);
                sqlConn.Open();
                sqlConn.Close();
                System.Windows.Forms.MessageBox.Show(this, "Connection Successful", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonApplySchema_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection sqlConn = new SqlConnection(ConnectionString);
                sqlConn.Open();
                
                string[] commands = _schema.Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
                
                SqlTransaction trans = sqlConn.BeginTransaction();

                foreach (string command in commands)
                {
                    SqlCommand sqlCmd = new SqlCommand(command, sqlConn);
                    sqlCmd.Transaction = trans;
                    sqlCmd.ExecuteNonQuery();
                }

                trans.Commit();
                sqlConn.Close();
                System.Windows.Forms.MessageBox.Show(this, "Schema Created Successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // Check that the connection is not already defined in the shard...
            try
            {
                SqlConnection sqlConn = new SqlConnection(ConnectionString);
                SqlCommand cmd = new SqlCommand();

                // Is the GUID already in the list of connections from the Shard?
                if (Shard.ShardConnections.Where(conn => conn.ConnectionGuid() == sqlConn.ConnectionGuid()).Count() > 0)
                    throw new Exception("This GUID already exists in the shard... cannot add a duplicate GUID.");

                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
