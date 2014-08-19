/*
 * Overview
 * ********
 * 
 * IMPORTANT: THIS PROGRAM EXPECTS A SQL AZURE DATABASE WITH DATA FEDERATION 
 * -------------------------------------------------------------------------
 * 
 * This user interface provides an example on how to use the data federation shard strategy.
 * 
 * The statements provided as examples expect the following federated database layout:
 * 
 *      Federation 1:  
 *          Name:       customerfederation
 *          Key Name:   cid
 *          Table Name: customer [must contain these fields at a minimum: CustomerId,  firstname, lastname, phone]
 *          
                CREATE TABLE [dbo].[Customer](
	                [CustomerID] [bigint] NOT NULL,
	                [Title] [nvarchar](8) NULL,
	                [FirstName] [nvarchar](50) NOT NULL,
	                [MiddleName] [nvarchar](50) NULL,
	                [LastName] [nvarchar](50) NOT NULL,
	                [Suffix] [nvarchar](10) NULL,
	                [CompanyName] [nvarchar](128) NULL,
	                [SalesPerson] [nvarchar](256) NULL,
	                [EmailAddress] [nvarchar](50) NULL,
	                [Phone] [nvarchar](25) NULL,
                    CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
                    (
	                    [CustomerID] ASC
                    ) 
                )FEDERATED ON (cid=CustomerID)
 * 
 * 
 *      Federation 2:
 *          Name:       purchasefederation
 *          Key Name:   pid
 *          Table Name: purchase [must contain a CustomerId field, and a StoreId field]
 * 
 * 
                CREATE TABLE dbo.Sales
                (
	                PurchaseId bigint,
	                CustomerId int,
	                DateCreated datetime DEFAULT(GETDATE()), 
	                Amount decimal(10,2),
	                StoreId int
                    CONSTRAINT [PK_Sales] PRIMARY KEY CLUSTERED 
                    (
	                    [PurchaseId] ASC
                    ) 
                )FEDERATED ON (pid=PurchaseId)
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PYN.EnzoAzureLib.DataFederation;
using System.Data.SqlClient;
using PYN.EnzoAzureLib;

namespace FederationSampleUI
{
    public partial class Form1 : Form
    {

        RootDatabase root_db = null;

        public Form1()
        {
            InitializeComponent();
        }

        //private void buttonConnect_Click(object sender, EventArgs e)
        //{
        //    try
        //    {

        //        root_db = new RootDatabase(this.textBoxRootConnection.Text);

        //        // Executes against the ROOT database
        //        DataTable table0 = root_db.ExecuteDataTable("SELECT * FROM sys.objects");
        //        // Automatically fans out the query across the customerfederation federation
        //        DataTable table1 = root_db["customerfederation"].ExecuteDataTable("SELECT * FROM Customer");
        //        // Automatically filters the query to the correct customerfederation federation member
        //        DataTable table2 = root_db["customerfederation"].ExecuteDataTable(70, "SELECT * FROM Customer");

        //        string sqlFilter = @"IF (Exists(SELECT * FROM Customer WHERE FirstName like 'l%')) SELECT CAST(1 as BIT) ELSE SELECT CAST(0 as BIT)";
        //        root_db["customerfederation"].MemberIndexes.Add("Index_FirstNameLike_L", sqlFilter);

        //        // The first time this executes, the cache filter is built against all federation members
        //        root_db["customerfederation"].DefaultExecutionContext.IndexName = "Index_FirstNameLike_L";
        //        DataTable table4 = root_db["customerfederation"].ExecuteDataTable("SELECT * FROM Customer WHERE FirstName like 'l%'");

        //        // The second time this executes, the cache filter is used, ignoring the federation members that do not have a hit
        //        DataTable table5 = root_db["customerfederation"].ExecuteDataTable("SELECT * FROM Customer WHERE FirstName like 'l%'");

        //        //DataTable table3 = root_db["simple_federation"].ExecuteDataTable(70, "SELECT * FROM BACKUPTEST.SimpleTable");

        //    }
        //    catch (Exception ex)
        //    {

        //        System.Windows.Forms.MessageBox.Show(this, ex.GetBaseException().Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        //    }
        //}

        //private void button1_Click(object sender, EventArgs e)
        //{

        //    DataTable dt = new DataTable();

        //    try
        //    {
        //        SqlCommand command = new SqlCommand("SELECT * FROM BACKUPTEST.SimpleTable");
        //        SqlConnection connectionToUse = new SqlConnection(this.textBoxRootConnection.Text);
        //        connectionToUse.Open();

        //        string preExecute = "";

        //        preExecute = "USE FEDERATION {0}({1}={2}) WITH RESET, FILTERING = {3} ";
        //        preExecute = string.Format(preExecute, "simple_federation", "cid", 60, "ON");
        //        SqlCommand cmd_preliminary = new SqlCommand(preExecute, connectionToUse);
        //        cmd_preliminary.ExecuteNonQuery();

        //        SqlDataAdapter da = new SqlDataAdapter(command);
        //        command.Connection = connectionToUse;

        //        da.Fill(dt);
        //        connectionToUse.Close();

        //    }
        //    catch (Exception ex)
        //    {
        //        System.Windows.Forms.MessageBox.Show(this, ex.GetBaseException().Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    root_db = new RootDatabase(this.textBoxRootConnection.Text);


        //    string sql1 = "SELECT MIN(customerid), MAX(customerid), COUNT(customerid), AVG(customerid) USING (SELECT CustomerId FROM CUSTOMER)";
        //    string sql2 = "SELECT CustomerId FROM CUSTOMER";

        //    DistributedQuery dq1 = new DistributedQuery(sql1);

        //    root_db["customerfederation"].DefaultExecutionContext.Mode = ShardCore.ShardOperationMode.FILTERING_OFF;
        //    DataTable table1 = root_db["customerfederation"].ExecuteDataTable(70, sql1);

        //    DistributedQuery dq2 = new DistributedQuery(sql2);

        //}

        private void label5_Click(object sender, EventArgs e)
        {

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


                    string value = this.textBoxMemberValue.Text;
                    string federationName = textBoxFederationName.Text;
                    string indexName = comboBoxIndex.Text;
                    ShardCore.ShardOperationMode mode = ShardCore.ShardOperationMode.UNKNOWN;

                    switch (comboBoxOperation.Text.ToLower())
                    {
                        case "root": mode = ShardCore.ShardOperationMode.ROOT; break;
                        case "filter on": mode = ShardCore.ShardOperationMode.FILTERING_ON; break;
                        case "filter off": mode = ShardCore.ShardOperationMode.FILTERING_OFF; break;
                        case "fan out": mode = ShardCore.ShardOperationMode.FAN_OUT; break;
                    }

                    sw.Start();

                    if (mode == ShardCore.ShardOperationMode.ROOT)
                        data = root_db.ExecuteDataTable(this.textBoxSQL.Text);
                    else
                    {
                        if (!root_db.Exists(p => p.Name.ToLower() == federationName.ToLower()))
                            throw new Exception("The federation name does not exist");
                        root_db[federationName].DefaultExecutionContext.Mode = mode;
                        root_db[federationName].DefaultExecutionContext.MemberValue = value;
                        root_db[federationName].DefaultExecutionContext.IndexName = indexName;
                        root_db[federationName].DefaultExecutionContext.AddTenantKey = checkBoxAddFederationKey.Checked;
                        data = root_db[federationName].ExecuteDataTable(this.textBoxSQL.Text);
                    }

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
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.labelInfo.Text = "Error";
            }

            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;

        }

        private void textBoxRootConnection_TextChanged(object sender, EventArgs e)
        {
            root_db = null;
        }

        private void comboBoxSQL_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.textBoxSQL.Text = comboBoxSQL.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Build a BitMap index in memory. The statement must return 1 or 0 as a BIT field.

                FormBuildIndex form = new FormBuildIndex(root_db, this.textBoxSQL.Text, this.textBoxFederationName.Text);
                if (form.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    string savedIndex = this.comboBoxIndex.Text;

                    comboBoxIndex.Items.Clear();
                    comboBoxIndex.Items.Add("");

                    foreach (string key in root_db[textBoxFederationName.Text].MemberIndexes.Keys)
                        comboBoxIndex.Items.Add(key);

                    try
                    {
                        comboBoxIndex.Text = savedIndex;
                    }
                    catch { }

                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkBoxPassThrough_CheckedChanged(object sender, EventArgs e)
        {
            panelAPIOptions.Enabled = !checkBoxPassThrough.Checked;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }
}
