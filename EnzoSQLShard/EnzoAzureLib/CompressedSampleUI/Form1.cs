using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using PYN.EnzoAzureLib.CompressedShard;
using PYN.EnzoAzureLib;

namespace CompressedSampleUI
{
    public partial class Form1 : Form
    {

        RootDatabase root_db = null;

        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ShowConfigurationScreen();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string conn = textBoxRootConnection.Text;
                SqlConnection sqlConn = new SqlConnection(conn);
                sqlConn.Open();
                sqlConn.Close();
                System.Windows.Forms.MessageBox.Show(this, "Connection was successful", "Success", MessageBoxButtons.OK);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK);
            }
        }

        private void buttonExecute_Click(object sender, EventArgs e)
        {
            try
            {
                this.labelInfo.Text = "Processing...";
                this.labelInfo.Refresh();
                DataTable data = null;
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;

                if (root_db == null)
                    root_db = new RootDatabase(this.textBoxRootConnection.Text);


                //root_db["customerfederation"].DefaultExecutionContext.Mode = ShardCore.ShardOperationMode.FAN_OUT;
                //root_db["customerfederation"].ExecuteDataTable("select * from customer");

                // the checkBoxPassThrough checkbox determines if a Distributed Query should be sent for processing or not
                // If not, the settings in the UI will be used to build the execution options.
                if (!checkBoxPassThrough.Checked)
                {

                    if (comboBoxOperation.SelectedIndex == -1)
                        throw new Exception("Choose an operation mode");

                    if (comboBoxOperation.SelectedIndex == 1 && textBoxMemberValue.Text.Trim() == "")
                        throw new Exception("Please enter a customer Key before running a filtered statement");

                    string value = this.textBoxMemberValue.Text;
                    string indexName = comboBoxIndex.Text;
                    ShardCore.ShardOperationMode mode = ShardCore.ShardOperationMode.UNKNOWN;

                    switch (comboBoxOperation.Text.ToLower())
                    {
                        case "root": mode = ShardCore.ShardOperationMode.ROOT; break;
                        case "filter on": mode = ShardCore.ShardOperationMode.FILTERING_ON; break;
                        case "filter off": mode = ShardCore.ShardOperationMode.FILTERING_OFF; break;
                        case "fan out": mode = ShardCore.ShardOperationMode.FAN_OUT; break;
                    }

                    root_db.Tenants.DefaultExecutionContext.Mode = mode;
                    root_db.Tenants.DefaultExecutionContext.MemberValue = value;
                    root_db.Tenants.DefaultExecutionContext.IndexName = indexName;

                    sw.Start();

                    if (mode == ShardCore.ShardOperationMode.ROOT)
                        data = root_db.ExecuteDataTable(this.textBoxSQL.Text);
                    else
                        data = root_db.Tenants.ExecuteDataTable(this.textBoxSQL.Text);

                }
                else
                {
                    sw.Start();

                    // Execute a distributed query. The execution options will be automatically created by the API.
                    data = root_db.ExecuteDataTable(this.textBoxSQL.Text);
                }

                sw.Stop();

                this.dataGridView1.DataSource = data;

                this.labelInfo.Text = "Execution time: " + sw.ElapsedMilliseconds.ToString() + " milliseconds";
                this.labelRecordCount.Text = dataGridView1.Rows.Count.ToString() + " records";
            }
            catch (AggregateException ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.InnerExceptions[0].Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.labelInfo.Text = "Error";
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.labelInfo.Text = "Error";
            }

            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
        }


        private void buttonConfigureShard_Click(object sender, EventArgs e)
        {
            ShowConfigurationScreen();
        }

        private void ShowConfigurationScreen()
        {
            try
            {
                FormConfigure form = new FormConfigure();
                form.ShowDialog(this);

                textBoxRootConnection.Text = form.RootConnectionString;

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.labelInfo.Text = "Error";
            }
        }

        private void comboBoxSQL_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxSQL.Text = comboBoxSQL.Text;
        }

        private void overviewOfTheEnzoExpandedShardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormConfigureHelp form = new FormConfigureHelp();
            form.ShowDialog(this);
        }

        private void howToConfigureTheTestApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormHowToUseApp form = new FormHowToUseApp();
            form.ShowDialog(this);
        }
    }
}
