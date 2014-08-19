namespace CompressedSampleUI
{
    partial class FormConfigure
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfigure));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxPWD = new System.Windows.Forms.TextBox();
            this.textBoxUID = new System.Windows.Forms.TextBox();
            this.textBoxDB = new System.Windows.Forms.TextBox();
            this.textBoxServer = new System.Windows.Forms.TextBox();
            this.buttonCreateConfig = new System.Windows.Forms.Button();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.linkLabelNew = new System.Windows.Forms.LinkLabel();
            this.linkLabelRemove = new System.Windows.Forms.LinkLabel();
            this.linkLabelRefresh = new System.Windows.Forms.LinkLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonCreateTestHistory = new System.Windows.Forms.Button();
            this.checkBoxTenantEnabled = new System.Windows.Forms.CheckBox();
            this.labelID = new System.Windows.Forms.Label();
            this.textBoxTenantPWD = new System.Windows.Forms.TextBox();
            this.textBoxCustomerKey = new System.Windows.Forms.TextBox();
            this.textBoxTenantUID = new System.Windows.Forms.TextBox();
            this.textBoxTenantServer = new System.Windows.Forms.TextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxTenantDB = new System.Windows.Forms.TextBox();
            this.buttonTryConnection = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.listViewTenants = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label5 = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.textBoxPWD);
            this.groupBox1.Controls.Add(this.textBoxUID);
            this.groupBox1.Controls.Add(this.textBoxDB);
            this.groupBox1.Controls.Add(this.textBoxServer);
            this.groupBox1.Controls.Add(this.buttonCreateConfig);
            this.groupBox1.Controls.Add(this.buttonConnect);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(718, 129);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Root Database";
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(21, 19);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(691, 18);
            this.label10.TabIndex = 3;
            this.label10.Text = "The root database must be first created manually if it doesn\'t exist. It can be o" +
    "n SQL Server or SQL Database.";
            // 
            // textBoxPWD
            // 
            this.textBoxPWD.Location = new System.Drawing.Point(325, 68);
            this.textBoxPWD.Name = "textBoxPWD";
            this.textBoxPWD.Size = new System.Drawing.Size(168, 20);
            this.textBoxPWD.TabIndex = 3;
            this.toolTip1.SetToolTip(this.textBoxPWD, "The password of the root database");
            this.textBoxPWD.UseSystemPasswordChar = true;
            // 
            // textBoxUID
            // 
            this.textBoxUID.Location = new System.Drawing.Point(99, 68);
            this.textBoxUID.Name = "textBoxUID";
            this.textBoxUID.Size = new System.Drawing.Size(168, 20);
            this.textBoxUID.TabIndex = 2;
            this.toolTip1.SetToolTip(this.textBoxUID, "The User Id of the root database. Leave blank when using Integrated Security with" +
        " SQL Server");
            // 
            // textBoxDB
            // 
            this.textBoxDB.Location = new System.Drawing.Point(501, 44);
            this.textBoxDB.Name = "textBoxDB";
            this.textBoxDB.Size = new System.Drawing.Size(203, 20);
            this.textBoxDB.TabIndex = 1;
            this.toolTip1.SetToolTip(this.textBoxDB, "The database name of the root database (by default this is RootDB)");
            // 
            // textBoxServer
            // 
            this.textBoxServer.Location = new System.Drawing.Point(99, 44);
            this.textBoxServer.Name = "textBoxServer";
            this.textBoxServer.Size = new System.Drawing.Size(269, 20);
            this.textBoxServer.TabIndex = 0;
            this.toolTip1.SetToolTip(this.textBoxServer, "The SQL Server or SQL Database server that contains the Root database.");
            // 
            // buttonCreateConfig
            // 
            this.buttonCreateConfig.Enabled = false;
            this.buttonCreateConfig.Location = new System.Drawing.Point(152, 94);
            this.buttonCreateConfig.Name = "buttonCreateConfig";
            this.buttonCreateConfig.Size = new System.Drawing.Size(216, 23);
            this.buttonCreateConfig.TabIndex = 5;
            this.buttonCreateConfig.Text = "Create Empty Config Tables...";
            this.toolTip1.SetToolTip(this.buttonCreateConfig, "This button creates an empty root database (serversdef and tenantsdef tables).  M" +
        "ake sure the database already exists.");
            this.buttonCreateConfig.UseVisualStyleBackColor = true;
            this.buttonCreateConfig.Click += new System.EventHandler(this.buttonCreateConfig_Click);
            // 
            // buttonConnect
            // 
            this.buttonConnect.BackColor = System.Drawing.Color.Yellow;
            this.buttonConnect.Location = new System.Drawing.Point(24, 94);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(95, 23);
            this.buttonConnect.TabIndex = 4;
            this.buttonConnect.Text = "Connect...";
            this.toolTip1.SetToolTip(this.buttonConnect, "Connect to the root database");
            this.buttonConnect.UseVisualStyleBackColor = false;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(283, 71);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "PWD:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "UID:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(408, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Database Name:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server Name:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.linkLabelNew);
            this.groupBox2.Controls.Add(this.linkLabelRemove);
            this.groupBox2.Controls.Add(this.linkLabelRefresh);
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Controls.Add(this.listViewTenants);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Enabled = false;
            this.groupBox2.Location = new System.Drawing.Point(12, 147);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(718, 286);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tenant Databases";
            // 
            // linkLabelNew
            // 
            this.linkLabelNew.AutoSize = true;
            this.linkLabelNew.Location = new System.Drawing.Point(624, 123);
            this.linkLabelNew.Name = "linkLabelNew";
            this.linkLabelNew.Size = new System.Drawing.Size(66, 13);
            this.linkLabelNew.TabIndex = 3;
            this.linkLabelNew.TabStop = true;
            this.linkLabelNew.Text = "New Tenant";
            this.toolTip1.SetToolTip(this.linkLabelNew, "Clears the Input area below to add a new Tenant in the tenantsdef table");
            this.linkLabelNew.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelNew_LinkClicked);
            // 
            // linkLabelRemove
            // 
            this.linkLabelRemove.AutoSize = true;
            this.linkLabelRemove.Enabled = false;
            this.linkLabelRemove.Location = new System.Drawing.Point(624, 98);
            this.linkLabelRemove.Name = "linkLabelRemove";
            this.linkLabelRemove.Size = new System.Drawing.Size(47, 13);
            this.linkLabelRemove.TabIndex = 2;
            this.linkLabelRemove.TabStop = true;
            this.linkLabelRemove.Text = "Remove";
            this.toolTip1.SetToolTip(this.linkLabelRemove, "Delete the selected tenant from the tenantsdef table");
            this.linkLabelRemove.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelRemove_LinkClicked);
            // 
            // linkLabelRefresh
            // 
            this.linkLabelRefresh.AutoSize = true;
            this.linkLabelRefresh.Location = new System.Drawing.Point(624, 75);
            this.linkLabelRefresh.Name = "linkLabelRefresh";
            this.linkLabelRefresh.Size = new System.Drawing.Size(44, 13);
            this.linkLabelRefresh.TabIndex = 1;
            this.linkLabelRefresh.TabStop = true;
            this.linkLabelRefresh.Text = "Refresh";
            this.toolTip1.SetToolTip(this.linkLabelRefresh, "Reloads the list of tenants");
            this.linkLabelRefresh.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelRefresh_LinkClicked);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonCreateTestHistory);
            this.panel1.Controls.Add(this.checkBoxTenantEnabled);
            this.panel1.Controls.Add(this.labelID);
            this.panel1.Controls.Add(this.textBoxTenantPWD);
            this.panel1.Controls.Add(this.textBoxCustomerKey);
            this.panel1.Controls.Add(this.textBoxTenantUID);
            this.panel1.Controls.Add(this.textBoxTenantServer);
            this.panel1.Controls.Add(this.buttonSave);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.textBoxTenantDB);
            this.panel1.Controls.Add(this.buttonTryConnection);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Location = new System.Drawing.Point(24, 168);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(680, 112);
            this.panel1.TabIndex = 2;
            // 
            // buttonCreateTestHistory
            // 
            this.buttonCreateTestHistory.BackColor = System.Drawing.Color.Azure;
            this.buttonCreateTestHistory.Location = new System.Drawing.Point(15, 84);
            this.buttonCreateTestHistory.Name = "buttonCreateTestHistory";
            this.buttonCreateTestHistory.Size = new System.Drawing.Size(243, 23);
            this.buttonCreateTestHistory.TabIndex = 7;
            this.buttonCreateTestHistory.Text = "Create Test History Records...";
            this.buttonCreateTestHistory.UseVisualStyleBackColor = false;
            this.buttonCreateTestHistory.Click += new System.EventHandler(this.buttonCreateTestHistory_Click);
            // 
            // checkBoxTenantEnabled
            // 
            this.checkBoxTenantEnabled.AutoSize = true;
            this.checkBoxTenantEnabled.Location = new System.Drawing.Point(531, 37);
            this.checkBoxTenantEnabled.Name = "checkBoxTenantEnabled";
            this.checkBoxTenantEnabled.Size = new System.Drawing.Size(96, 17);
            this.checkBoxTenantEnabled.TabIndex = 4;
            this.checkBoxTenantEnabled.Text = "Enable Tenant";
            this.toolTip1.SetToolTip(this.checkBoxTenantEnabled, "When true, the tenant is available through the Shard API. ");
            this.checkBoxTenantEnabled.UseVisualStyleBackColor = true;
            // 
            // labelID
            // 
            this.labelID.AutoSize = true;
            this.labelID.Location = new System.Drawing.Point(329, 61);
            this.labelID.Name = "labelID";
            this.labelID.Size = new System.Drawing.Size(16, 13);
            this.labelID.TabIndex = 6;
            this.labelID.Text = "-1";
            // 
            // textBoxTenantPWD
            // 
            this.textBoxTenantPWD.Location = new System.Drawing.Point(332, 35);
            this.textBoxTenantPWD.Name = "textBoxTenantPWD";
            this.textBoxTenantPWD.Size = new System.Drawing.Size(168, 20);
            this.textBoxTenantPWD.TabIndex = 3;
            this.toolTip1.SetToolTip(this.textBoxTenantPWD, "The password for the tenant\'s user id");
            this.textBoxTenantPWD.UseSystemPasswordChar = true;
            // 
            // textBoxCustomerKey
            // 
            this.textBoxCustomerKey.Location = new System.Drawing.Point(90, 58);
            this.textBoxCustomerKey.Name = "textBoxCustomerKey";
            this.textBoxCustomerKey.Size = new System.Drawing.Size(168, 20);
            this.textBoxCustomerKey.TabIndex = 5;
            this.toolTip1.SetToolTip(this.textBoxCustomerKey, "The Tenant Key that is used to identify which tenant you are querying. This value" +
        " must be unique across all tenants.");
            // 
            // textBoxTenantUID
            // 
            this.textBoxTenantUID.Location = new System.Drawing.Point(90, 35);
            this.textBoxTenantUID.Name = "textBoxTenantUID";
            this.textBoxTenantUID.Size = new System.Drawing.Size(168, 20);
            this.textBoxTenantUID.TabIndex = 2;
            this.toolTip1.SetToolTip(this.textBoxTenantUID, "The database User ID to use for this tenant. When storing multiple tenants in the" +
        " same database, you should use different user ids to map the default schema of t" +
        "he tenant.");
            // 
            // textBoxTenantServer
            // 
            this.textBoxTenantServer.Location = new System.Drawing.Point(90, 9);
            this.textBoxTenantServer.Name = "textBoxTenantServer";
            this.textBoxTenantServer.Size = new System.Drawing.Size(254, 20);
            this.textBoxTenantServer.TabIndex = 0;
            this.toolTip1.SetToolTip(this.textBoxTenantServer, "The SQL Server or SQL Database server on which the tenant database will be stored" +
        "");
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(568, 84);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(102, 23);
            this.buttonSave.TabIndex = 9;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Server Name:";
            // 
            // textBoxTenantDB
            // 
            this.textBoxTenantDB.Location = new System.Drawing.Point(474, 9);
            this.textBoxTenantDB.Name = "textBoxTenantDB";
            this.textBoxTenantDB.Size = new System.Drawing.Size(196, 20);
            this.textBoxTenantDB.TabIndex = 1;
            this.toolTip1.SetToolTip(this.textBoxTenantDB, "The database that contains the tenant tables and objects");
            // 
            // buttonTryConnection
            // 
            this.buttonTryConnection.Location = new System.Drawing.Point(467, 84);
            this.buttonTryConnection.Name = "buttonTryConnection";
            this.buttonTryConnection.Size = new System.Drawing.Size(95, 23);
            this.buttonTryConnection.TabIndex = 8;
            this.buttonTryConnection.Text = "Try connection...";
            this.buttonTryConnection.UseVisualStyleBackColor = true;
            this.buttonTryConnection.Click += new System.EventHandler(this.buttonTryConnection_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(382, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Database Name:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(264, 61);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(58, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Tenant ID:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(274, 38);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(36, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "PWD:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 61);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(75, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Customer Key:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 38);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "UID:";
            // 
            // listViewTenants
            // 
            this.listViewTenants.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.listViewTenants.FullRowSelect = true;
            this.listViewTenants.HideSelection = false;
            this.listViewTenants.Location = new System.Drawing.Point(24, 66);
            this.listViewTenants.MultiSelect = false;
            this.listViewTenants.Name = "listViewTenants";
            this.listViewTenants.Size = new System.Drawing.Size(576, 96);
            this.listViewTenants.TabIndex = 0;
            this.listViewTenants.UseCompatibleStateImageBehavior = false;
            this.listViewTenants.View = System.Windows.Forms.View.Details;
            this.listViewTenants.SelectedIndexChanged += new System.EventHandler(this.listViewTenants_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Server Name";
            this.columnHeader1.Width = 160;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Database";
            this.columnHeader2.Width = 120;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "User ID";
            this.columnHeader3.Width = 109;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Customer Key";
            this.columnHeader4.Width = 120;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Enabled";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(21, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(691, 38);
            this.label5.TabIndex = 0;
            this.label5.Text = resources.GetString("label5.Text");
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(636, 439);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(95, 23);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // FormConfigure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(743, 480);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonClose);
            this.Name = "FormConfigure";
            this.Text = "Configure Shard";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.TextBox textBoxPWD;
        private System.Windows.Forms.TextBox textBoxUID;
        private System.Windows.Forms.TextBox textBoxDB;
        private System.Windows.Forms.TextBox textBoxServer;
        private System.Windows.Forms.Button buttonCreateConfig;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListView listViewTenants;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TextBox textBoxTenantPWD;
        private System.Windows.Forms.TextBox textBoxTenantUID;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxTenantDB;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonTryConnection;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.LinkLabel linkLabelRefresh;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TextBox textBoxCustomerKey;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.LinkLabel linkLabelRemove;
        private System.Windows.Forms.Label labelID;
        private System.Windows.Forms.TextBox textBoxTenantServer;
        private System.Windows.Forms.LinkLabel linkLabelNew;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox checkBoxTenantEnabled;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Button buttonCreateTestHistory;
    }
}