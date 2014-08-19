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
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            LoadShardDatabases();
            RefreshShardConnectionList();

        }

        /// <summary>
        /// Loads the Shard with a list of databases. In this example we look for databases defined in the ConnectionStrings section
        /// for which the name starts with SHARD. This is purely a convention for this sample application.
        /// </summary>
        private void LoadShardDatabases()
        {
            // Only do this once... this is a static list... exclude any connection string that doesn't start with SHARD
            // Each database is associated with a GUID in the collection. In this example the GUID is calculated on the fly
            // using certain parts of the connection string. REF #005
            Shard.ShardConnections = new List<SqlConnection>();

            foreach (System.Configuration.ConnectionStringSettings connStr in System.Configuration.ConfigurationManager.ConnectionStrings)
                if (connStr.Name.ToUpper().StartsWith("SHARD"))
                    Shard.ShardConnections.Add(new SqlConnection(connStr.ConnectionString));
        }

        /// <summary>
        /// Executes ad-hoc sql statement through the shard. This will execute any inline statement and load the result in the
        /// grid view, loading all the columns that are returned by the statement, and the _GUID_ virtual column.  
        /// </summary>
        private void buttonLoadGrid_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Running...";

            // Load the data using the Shard library
            SqlCommand cmd = new SqlCommand();
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            DataTable data = new DataTable();

            // Set the cache option
            PYN.EnzoAzureLib.Shard.UseCache = checkBoxUseCache.Checked;

            cmd.CommandText = this.textBox1.Text;
            
            try
            {
                sw.Start();
                data = cmd.ExecuteShardQuery(); 
                
            }
            catch (Exception ex)
            {
                sw.Stop();
                System.Windows.Forms.MessageBox.Show(this, ex.GetBaseException().Message, "Error");
            }

            if (sw.IsRunning)
                sw.Stop();

            toolStripStatusLabel1.Text = "Fetch Time: " + sw.ElapsedMilliseconds.ToString();

            dataGridView2.DataSource = data;

        }

        /// <summary>
        /// Loads a record's details and enables Update and Delete command buttons
        /// </summary>
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {

                panelDetails.Enabled = true;

                try
                {
                    labelIDVal.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                    textBoxUser.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                    labelLastUpdated.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
                    labelGUID.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                }
                catch { }
            }
        }

        /// <summary>
        /// Loads the list of users from the Shard.
        /// </summary>
        private void buttonLoadUserGrid_Click(object sender, EventArgs e)
        {
            //
            // Loads the list of users
            //
            RefreshList(true);

            ClearDetails();

        }

        private void buttonShowDDL_Click(object sender, EventArgs e)
        {
            //
            // Run this batch in all databases that participate in the shard
            //

            System.Windows.Forms.MessageBox.Show(@"
CREATE TABLE TestUsers2 (id int identity(1,1) PRIMARY KEY, name nvarchar(20), LastUpdated DateTime DEFAULT GetDate())
GO
CREATE PROC sproc_add_user (@name nvarchar(20)) AS INSERT INTO TestUsers2 VALUES (@name, default)
GO
CREATE PROC sproc_delete_user (@id int) AS DELETE FROM TestUsers2 WHERE id = @id
GO
CREATE PROC sproc_delete_users (@name nvarchar(20)) AS DELETE FROM TestUsers2 WHERE name like @name
GO
CREATE PROC sproc_update_user (@id int, @name nvarchar(20)) AS UPDATE TestUsers2 SET name=@name, lastUpdated = GETDATE() WHERE id = @id
GO
");
        }

        /// <summary>
        /// Update the name and last date updated of a single user from the list. This method 
        /// sends the virtual column _GUID_ to the shard library to determine which database
        /// connection should be used. 
        /// </summary>
        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            //
            // Updates a single record in the shard
            //
            try
            {
                SqlCommand cmd = new SqlCommand();
                
                cmd.CommandText = "sproc_update_user";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int));
                cmd.Parameters["@id"].Value = int.Parse(labelIDVal.Text);
                
                cmd.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 20));
                cmd.Parameters["@name"].Value = textBoxUser.Text;
                
                // This is the virtual column. Note that we use the Shard's column name definition since we 
                // cannot assume we know what the virtual column name should be. As a result, the code
                // doesn't know the name of the virtual column that contains the breadcrumb, and it doesn't
                // know which database is being called (only it's GUID value). 
                // To optimize this call the Shard library will execute the call against all the connections, but only one will 
                // actually send the SQL to the database
                // The Shard library will determine which database to execute against using this GUID (REF #001)
                // The GUID is verified dynamically using the connection string parameters (REF #005)
                // The Shard library will remove this virtual column before making the final call to the database (REF #002)
                // The Shard will execute all the calls using the Task Parallel Library for optimum performance (REF #003)
                cmd.Parameters.Add(new SqlParameter(PYN.EnzoAzureLib.SharedSettings._TENANTKEY_, labelGUID.Text));    // for updates, this is required

                ExecuteShardNonQueryCall(cmd);

                // An update was made... clear the cache
                Shard.ResetCache();

                RefreshList(false);

                ClearDetails();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.GetBaseException().Message, "Error");
            }

        }

        /// <summary>
        /// Adds one or more records in the Shard. If more than 1 user is being added, a special
        /// method is called in the Shard library to load records in mass.
        /// </summary>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            try
            {

                // Prompt user for a new user name
                FormAskName form = new FormAskName();
                if (form.ShowDialog(this) != DialogResult.OK)
                    return;

                string[] userName = form.UserName.Split('\n');

                if (userName.GetLength(0) == 1)
                {
                    //
                    // Adds a single user in the shard
                    //

                    SqlCommand cmd = new SqlCommand();
                    
                    cmd.CommandText = "sproc_add_user";
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("name", SqlDbType.NVarChar, 20));
                    cmd.Parameters["name"].Value = userName[0];

                    // In this case we are sending an INSERT statement to the next database in the Round Robin. 
                    // The Shard Library keeps a pointer to the last connection that was used. This allows your code
                    // to spread the load of inserts against multiple databases, and more importantly it will allow your
                    // SELECT statements to behave roughly the same throughout the databases in the Shard. 
                    
                    cmd.ExecuteNextRoundRobin();
                    
                    Shard.ResetCache();

                }
                else
                {
                    List<SqlCommand> commands = new List<SqlCommand>();

                    // Load multiple names in the database across multiple databases...
                    foreach (string name in userName)
                    {
                        if (name != null && name.Trim().Length > 0)
                        {
                            SqlCommand cmdToAdd = new SqlCommand();
                            cmdToAdd.CommandText = "sproc_add_user";
                            cmdToAdd.CommandType = CommandType.StoredProcedure;

                            cmdToAdd.Parameters.Add(new SqlParameter("name", SqlDbType.NVarChar, 20));
                            cmdToAdd.Parameters["name"].Value = name;

                            commands.Add(cmdToAdd);
                        }
                    }

                    // Make the call!
                    if (commands.Count > 0)
                    {
                        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                        sw.Start();

                        // In this case we are adding multiple records in the Shard. A method was
                        // created specifically for this case. The Round-Robin method is used.
                        commands.ExecuteParallelRoundRobinLoad();

                        sw.Stop();
                        toolStripStatusLabel1.Text = "Elapsed Time: " + sw.ElapsedMilliseconds.ToString();

                        Shard.ResetCache();

                    }
                }

                RefreshList(false);

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.GetBaseException().Message, "Error");
            }
        }

        /// <summary>
        /// Deletes the selected user
        /// </summary>
        private void buttonDeleteUser_Click(object sender, EventArgs e)
        {
            try
            {
                //
                // Deletes a single user from the shard
                //

                SqlCommand cmd = new SqlCommand();
                
                cmd.CommandText = "sproc_delete_user";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int));
                cmd.Parameters["@id"].Value = int.Parse(labelIDVal.Text);

                // The Delete operation is largely similar to the Update operation. See the buttonUpdate_Click event
                // for detailed information about this statement. 
                cmd.Parameters.Add(new SqlParameter(PYN.EnzoAzureLib.SharedSettings._TENANTKEY_, labelGUID.Text));    // for deletes, this is required

                ExecuteShardNonQueryCall(cmd);

                // Since we deleted a user, we need to reload the records from the database
                Shard.ResetCache();

                RefreshList(false);

                ClearDetails();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.GetBaseException().Message, "Error");
            }
        }

        /// <summary>
        /// Clears the detailed panel for a user. 
        /// </summary>
        private void ClearDetails()
        {
            panelDetails.Enabled = false;
            labelIDVal.Text = "";
            textBoxUser.Text = "";
            labelLastUpdated.Text = "";
            labelGUID.Text = "";
        }

        /// <summary>
        /// Updates all the records' timestamp in the Shard.
        /// </summary>
        private void buttonUpdateAllRecords_Click(object sender, EventArgs e)
        {
            //
            //  Updates all the records from the shard
            //

            try
            {
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "UPDATE TestUsers2 SET LastUpdated = GETDATE()";
                cmd.CommandType = CommandType.Text;

                // Since we are not passing a GUID parameter, all the databases in the shard will
                // be used in this call. 
                ExecuteShardNonQueryCall(cmd);

                Shard.ResetCache();
                
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.GetBaseException().Message, "Error");
            }

        }

        /// <summary>
        /// Centralized routine that makes the final call and updates the time elapsed by the call
        /// </summary>
        private void ExecuteShardNonQueryCall(SqlCommand cmd)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            cmd.ExecuteShardNonQuery();

            sw.Stop();

            toolStripStatusLabel1.Text = "Elapsed Time: " + sw.ElapsedMilliseconds.ToString();
        }

        /// <summary>
        /// Refreshes the list of users
        /// </summary>
        /// <param name="refreshTime">true to show the load time of this refresh</param>
        private void RefreshList(bool refreshTime)
        {
            try
            {
                if (refreshTime)
                    toolStripStatusLabel1.Text = "Running...";

                // Load the data using the Shard library
                SqlCommand cmd = new SqlCommand();
                DataTable data = new DataTable();
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

                // Set the Cache Option
                PYN.EnzoAzureLib.Shard.UseCache = checkBoxUseCache.Checked;

                cmd.CommandText = "SELECT * FROM TestUsers2";
                cmd.CommandType = CommandType.Text;
                
                try
                {
                    sw.Start();

                    QueryOptions qo = new QueryOptions(0, comboBoxOrderBy.Text);    // this is optional
                    data = cmd.ExecuteShardQuery(qo);
                }
                catch (Exception ex)
                {
                    sw.Stop();
                    System.Windows.Forms.MessageBox.Show(this, ex.GetBaseException().Message, "Error");
                }

                if (sw.IsRunning)
                    sw.Stop();

                if (refreshTime)
                    toolStripStatusLabel1.Text = "Fetch Time: " + sw.ElapsedMilliseconds.ToString();

                // reload the data
                dataGridView1.DataSource = data;

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.GetBaseException().Message, "Error");
            }

        }

        /// <summary>
        /// Adds a new connection to the shard
        /// </summary>
        private void toolStripButtonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                FormAddShardConnection form = new FormAddShardConnection(this.textBoxSchema.Text, Shard.ShardConnections);
                form.ShowDialog(this);

                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    Shard.ShardConnections.Add(new SqlConnection(form.ConnectionString));
                    RefreshShardConnectionList();
                }

                // refresh buttons
                listView1_SelectedIndexChanged(sender, e);

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Removes the selected shard connections
        /// </summary>
        private void toolStripButtonRemove_Click(object sender, EventArgs e)
        {
            // Find all the connections with a given GUID and remove them from the shard
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                string guid = item.Text;
                Shard.ShardConnections.RemoveAll( sqlConn => sqlConn.ConnectionGuid() == guid);
            }
            RefreshShardConnectionList();

            // refresh buttons
            listView1_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// Reloads the list of connections from the Shard
        /// </summary>
        private void toolStripButtonReload_Click(object sender, EventArgs e)
        {
            // Reload the shard connections
            LoadShardDatabases();
            RefreshShardConnectionList();
            // refresh buttons
            listView1_SelectedIndexChanged(sender, e);
        }

        /// <summary>
        /// Builds a new connection based on a copy of another
        /// </summary>
        private void toolStripButtonCopy_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count == 0) return;

                ListViewItem item = listView1.SelectedItems[0];
                SqlConnection sqlConnTmp = Shard.ShardConnections.Find(conn => conn.ConnectionGuid() == item.Text);

                FormAddShardConnection form = new FormAddShardConnection(this.textBoxSchema.Text, Shard.ShardConnections);
                form.SetFields(sqlConnTmp);
                form.ShowDialog(this);

                if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    Shard.ShardConnections.Add(new SqlConnection(form.ConnectionString));
                    RefreshShardConnectionList();
                }

                // refresh buttons
                listView1_SelectedIndexChanged(sender, e);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Reloads the list of database connections used in the Shard
        /// </summary>
        private void RefreshShardConnectionList()
        {
            this.listView1.Items.Clear();
            SqlCommand cmd = new SqlCommand();

            foreach (SqlConnection sqlConn in Shard.ShardConnections)
            {
                SqlConnectionStringBuilder cb = new SqlConnectionStringBuilder(sqlConn.ConnectionString);
                ListViewItem item = new ListViewItem(sqlConn.ConnectionGuid());
                item.SubItems.Add(cb.DataSource);
                item.SubItems.Add(cb.InitialCatalog ?? "master");
                item.SubItems.Add(cb.UserID);
                listView1.Items.Add(item);
            }

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            toolStripButtonCopy.Enabled = toolStripButtonRemove.Enabled = (listView1.SelectedItems.Count > 0) ;
        }


    }
}
