using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PYN.EnzoAzureLib.CompressedShard;
using PYN.EnzoAzureLib;

namespace CompressedSampleUI
{
    public partial class FormMain : Form
    {

        RootDatabase root_db = null;

        public FormMain()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void comboBoxTenantKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                labelCustomerKey.Text = comboBoxTenantKey.Text;
                LoadHistory(labelCustomerKey.Text);
            }
            catch { }
        }

        /// <summary>
        /// Loads the history records for a single tenant
        /// </summary>
        /// <param name="tenantId"></param>
        private void LoadHistory(string tenantId)
        {
            try
            {
                // Connect to the customer database, then fetch the records
                DataTable data = null;
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

                if (root_db == null)
                    throw new Exception("Please connect to the root database (File -> Manage Shard)");

                // We are filtering the query for a specific tenant
                ShardCore.ShardOperationMode mode = ShardCore.ShardOperationMode.FILTERING_ON;
                root_db.Tenants.DefaultExecutionContext.Mode = mode;            // The mode to use: filter_on
                root_db.Tenants.DefaultExecutionContext.MemberValue = tenantId; // The tenant id
                root_db.Tenants.DefaultExecutionContext.IndexName = "";         // no tenant indexing needed
                root_db.Tenants.DefaultExecutionContext.AddTenantKey = false;   // No need for the TenantKey 

                // Execute the statement now
                data = root_db.Tenants.ExecuteDataTable("SELECT * FROM history");
                
                // Show the data
                this.dataGridView1.DataSource = data;
                this.labelRecordCount.Text = dataGridView1.Rows.Count.ToString() + " records";
            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default; }
        }

        /// <summary>
        /// Loads the history records for all tenants
        /// </summary>
        private void LoadHistory2()
        {
            try
            {
                // Connect to the customer database, then fetch the records
                DataTable data = null;
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

                if (root_db == null)
                    throw new Exception("Please connect to the root database (File -> Manage Shard)");

                

                if (!checkBoxUseDTSQL.Checked)
                {
                    // We are filtering the query for a specific tenant
                    ShardCore.ShardOperationMode mode = ShardCore.ShardOperationMode.FAN_OUT;
                    root_db.Tenants.DefaultExecutionContext.Mode = mode;            // The mode to use: fan_out
                    root_db.Tenants.DefaultExecutionContext.MemberValue = "";       // The tenant id
                    root_db.Tenants.DefaultExecutionContext.IndexName = "";         // no tenant indexing needed
                    root_db.Tenants.DefaultExecutionContext.AddTenantKey = checkBoxAddTenantColumn.Checked; // Add TenantKey column?

                    // Execute the statement now
                    data = root_db.Tenants.ExecuteDataTable("SELECT * FROM history");
                }
                else
                {
                    // Use the simpler distributed SQL approach
                    string dsql = "SELECT * USING (SELECT * FROM history) FEDERATED ON (tenant)";
                    if (checkBoxUseCaching.Checked && numericUpDownCacheTTL.Value > 0)
                        dsql += " CACHED FOR " + numericUpDownCacheTTL;
                    data = root_db.ExecuteDataTable(dsql);
                }

                // Show the data
                this.dataGridView2.DataSource = data;
                this.labelRecordCount.Text = dataGridView1.Rows.Count.ToString() + " records";
            }
            catch (Exception ex) 
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.dataGridView2.DataSource = null; 
            }
            finally { System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default; }
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoadHistory(labelCustomerKey.Text);
        }

        private void linkLabelRefreshTenants_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RefreshTenants(comboBoxTenantKey);
        }

        private void RefreshTenants(System.Windows.Forms.ComboBox combo)
        {
            try
            {
                labelCustomerKey.Text = "";
                root_db.Refresh();

                combo.Items.Clear();

                foreach (var t in root_db.Tenants)
                    combo.Items.Add(t.TenantKey);

            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            ShowConfigurationScreen();
            RefreshTenants(comboBoxTenantKey);
        }

        private void ShowConfigurationScreen()
        {
            try
            {
                FormConfigure form = new FormConfigure();
                form.ShowDialog(this);

                root_db = new RootDatabase(form.RootConnectionString);

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void generalShardConfigurationOverviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FormConfigureHelp form = new FormConfigureHelp();
                form.ShowDialog(this);
            }
            catch { }
        }

        private void freeformQueryWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Form1 form = new Form1();
                form.ShowDialog(this);
            }
            catch { }
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowConfigurationScreen();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void checkBoxUseDTSQL_CheckedChanged(object sender, EventArgs e)
        {
            labelUseDSQL.Text = (checkBoxUseDTSQL.Checked) ? "Yes" : "No";
            checkBoxUseCaching.Enabled = checkBoxUseDTSQL.Checked;
            
        }

        private void checkBoxUseCaching_CheckedChanged(object sender, EventArgs e)
        {
            labelUseCaching.Text = (checkBoxUseCaching.Checked && checkBoxUseCaching.Enabled) ? "Yes" : "No";
        }

        private void linkLabelRefreshHistory2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoadHistory2();
        }

        private void linkLabelRefreshTenants2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RefreshTenants(comboBoxTenantKey2);
        }

        private void comboBoxTenantKey2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void linkLabelAddHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
                string tenantId = comboBoxTenantKey2.Text;

                if (root_db == null)
                    throw new Exception("Please connect to the root database (File -> Manage Shard)");

                // We are filtering the query for a specific tenant
                ShardCore.ShardOperationMode mode = ShardCore.ShardOperationMode.FILTERING_ON;
                root_db.Tenants.DefaultExecutionContext.Mode = mode;            // The mode to use: filter_on
                root_db.Tenants.DefaultExecutionContext.MemberValue = tenantId; // The tenant id
                root_db.Tenants.DefaultExecutionContext.IndexName = "";         // no tenant indexing needed

                // Execute the statement now
                string sqlToExec = "INSERT INTO history VALUES ('{0}', {1}, GETDATE())";
                sqlToExec = string.Format(sqlToExec, textBoxUserName.Text, textBoxAmount.Text);
                root_db.Tenants.ExecuteDataTable(sqlToExec);

                System.Windows.Forms.MessageBox.Show(this, "Record saved successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex) { System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default; }
        }

        private void checkBoxAddTenantColumn_CheckedChanged(object sender, EventArgs e)
        {
            labelAddTenantKey.Text = (checkBoxAddTenantColumn.Checked) ? "Yes" : "No";
        }
    }
}
