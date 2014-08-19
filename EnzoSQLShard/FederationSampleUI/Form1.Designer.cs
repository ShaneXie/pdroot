namespace FederationSampleUI
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.buttonExecute = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxRootConnection = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxSQL = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelInfo = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panelAPIOptions = new System.Windows.Forms.Panel();
            this.textBoxFederationName = new System.Windows.Forms.TextBox();
            this.textBoxMemberValue = new System.Windows.Forms.TextBox();
            this.comboBoxIndex = new System.Windows.Forms.ComboBox();
            this.comboBoxOperation = new System.Windows.Forms.ComboBox();
            this.checkBoxPassThrough = new System.Windows.Forms.CheckBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.comboBoxSQL = new System.Windows.Forms.ComboBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel6 = new System.Windows.Forms.Panel();
            this.labelRecordCount = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBoxAddFederationKey = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelAPIOptions.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel6.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonExecute
            // 
            this.buttonExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExecute.Location = new System.Drawing.Point(984, 4);
            this.buttonExecute.Name = "buttonExecute";
            this.buttonExecute.Size = new System.Drawing.Size(97, 23);
            this.buttonExecute.TabIndex = 0;
            this.buttonExecute.Text = "Execute";
            this.buttonExecute.UseVisualStyleBackColor = true;
            this.buttonExecute.Click += new System.EventHandler(this.buttonExecute_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Root DB Connection:";
            // 
            // textBoxRootConnection
            // 
            this.textBoxRootConnection.Location = new System.Drawing.Point(126, 8);
            this.textBoxRootConnection.Name = "textBoxRootConnection";
            this.textBoxRootConnection.Size = new System.Drawing.Size(758, 20);
            this.textBoxRootConnection.TabIndex = 2;
            this.textBoxRootConnection.UseSystemPasswordChar = true;
            this.textBoxRootConnection.TextChanged += new System.EventHandler(this.textBoxRootConnection_TextChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(868, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(110, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Build Index...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Statement:";
            // 
            // textBoxSQL
            // 
            this.textBoxSQL.AcceptsReturn = true;
            this.textBoxSQL.AcceptsTab = true;
            this.textBoxSQL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxSQL.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSQL.Location = new System.Drawing.Point(8, 29);
            this.textBoxSQL.Multiline = true;
            this.textBoxSQL.Name = "textBoxSQL";
            this.textBoxSQL.Size = new System.Drawing.Size(547, 128);
            this.textBoxSQL.TabIndex = 4;
            this.textBoxSQL.Text = "SELECT * FROM Customer";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelInfo);
            this.panel1.Controls.Add(this.buttonExecute);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 382);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1096, 30);
            this.panel1.TabIndex = 5;
            // 
            // labelInfo
            // 
            this.labelInfo.AutoSize = true;
            this.labelInfo.Location = new System.Drawing.Point(12, 8);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(0, 13);
            this.labelInfo.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.textBoxRootConnection);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1096, 34);
            this.panel2.TabIndex = 6;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.splitContainer1);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 34);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1096, 348);
            this.panel3.TabIndex = 7;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(120, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panelAPIOptions);
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxPassThrough);
            this.splitContainer1.Panel1.Controls.Add(this.panel5);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Panel2.Controls.Add(this.panel6);
            this.splitContainer1.Size = new System.Drawing.Size(976, 348);
            this.splitContainer1.SplitterDistance = 563;
            this.splitContainer1.TabIndex = 9;
            // 
            // panelAPIOptions
            // 
            this.panelAPIOptions.Controls.Add(this.checkBoxAddFederationKey);
            this.panelAPIOptions.Controls.Add(this.textBoxFederationName);
            this.panelAPIOptions.Controls.Add(this.textBoxMemberValue);
            this.panelAPIOptions.Controls.Add(this.comboBoxIndex);
            this.panelAPIOptions.Controls.Add(this.comboBoxOperation);
            this.panelAPIOptions.Location = new System.Drawing.Point(8, 197);
            this.panelAPIOptions.Name = "panelAPIOptions";
            this.panelAPIOptions.Size = new System.Drawing.Size(547, 145);
            this.panelAPIOptions.TabIndex = 11;
            // 
            // textBoxFederationName
            // 
            this.textBoxFederationName.Location = new System.Drawing.Point(3, 7);
            this.textBoxFederationName.Name = "textBoxFederationName";
            this.textBoxFederationName.Size = new System.Drawing.Size(319, 20);
            this.textBoxFederationName.TabIndex = 7;
            this.textBoxFederationName.Text = "customerfederation";
            // 
            // textBoxMemberValue
            // 
            this.textBoxMemberValue.Location = new System.Drawing.Point(3, 60);
            this.textBoxMemberValue.Name = "textBoxMemberValue";
            this.textBoxMemberValue.Size = new System.Drawing.Size(319, 20);
            this.textBoxMemberValue.TabIndex = 7;
            this.textBoxMemberValue.Text = "70";
            // 
            // comboBoxIndex
            // 
            this.comboBoxIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIndex.FormattingEnabled = true;
            this.comboBoxIndex.Location = new System.Drawing.Point(3, 34);
            this.comboBoxIndex.Name = "comboBoxIndex";
            this.comboBoxIndex.Size = new System.Drawing.Size(319, 21);
            this.comboBoxIndex.TabIndex = 9;
            // 
            // comboBoxOperation
            // 
            this.comboBoxOperation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOperation.FormattingEnabled = true;
            this.comboBoxOperation.Items.AddRange(new object[] {
            "ROOT",
            "FILTER ON",
            "FILTER OFF",
            "FAN OUT"});
            this.comboBoxOperation.Location = new System.Drawing.Point(1, 90);
            this.comboBoxOperation.Name = "comboBoxOperation";
            this.comboBoxOperation.Size = new System.Drawing.Size(321, 21);
            this.comboBoxOperation.TabIndex = 8;
            // 
            // checkBoxPassThrough
            // 
            this.checkBoxPassThrough.AutoSize = true;
            this.checkBoxPassThrough.Location = new System.Drawing.Point(10, 171);
            this.checkBoxPassThrough.Name = "checkBoxPassThrough";
            this.checkBoxPassThrough.Size = new System.Drawing.Size(168, 17);
            this.checkBoxPassThrough.TabIndex = 10;
            this.checkBoxPassThrough.Text = "Use Distributed T-SQL Engine";
            this.checkBoxPassThrough.UseVisualStyleBackColor = true;
            this.checkBoxPassThrough.CheckedChanged += new System.EventHandler(this.checkBoxPassThrough_CheckedChanged);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.textBoxSQL);
            this.panel5.Controls.Add(this.comboBoxSQL);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Margin = new System.Windows.Forms.Padding(8);
            this.panel5.Name = "panel5";
            this.panel5.Padding = new System.Windows.Forms.Padding(8);
            this.panel5.Size = new System.Drawing.Size(563, 165);
            this.panel5.TabIndex = 6;
            // 
            // comboBoxSQL
            // 
            this.comboBoxSQL.Dock = System.Windows.Forms.DockStyle.Top;
            this.comboBoxSQL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSQL.FormattingEnabled = true;
            this.comboBoxSQL.Items.AddRange(new object[] {
            "--- REQUIRE API SETTINGS ---",
            "SELECT * FROM Customer",
            "SELECT * FROM Customer WHERE FirstName like \'l%\'",
            "--- REQUIRES CHECKING THE \'USE DISTRIBUTED T-SQL ENGINE\' OPTION  ---",
            "SELECT MIN(customerid), MAX(customerid), COUNT(customerid), AVG(customerid) USING" +
                " (SELECT CustomerId FROM CUSTOMER) FEDERATED ON (customerfederation) ",
            "SELECT firstname, lastname, phone using (select * FROM Customer) FEDERATED ON (cu" +
                "stomerfederation) ",
            "SELECT firstname, lastname, phone using (select * FROM Customer) FEDERATED ON (cu" +
                "stomerfederation, cid = 70) ",
            "SELECT firstname, lastname, phone using (select * FROM Customer) FEDERATED ON (cu" +
                "stomerfederation, cid = 70, FILTERED) ",
            "SELECT * using (select * FROM Sales) FEDERATED ON (purchasefederation, pid = 10, " +
                "FILTERED) ",
            resources.GetString("comboBoxSQL.Items"),
            resources.GetString("comboBoxSQL.Items1"),
            resources.GetString("comboBoxSQL.Items2"),
            resources.GetString("comboBoxSQL.Items3"),
            "--- INDEX CREATION ---",
            "IF (Exists(SELECT * FROM Customer WHERE FirstName like \'l%\')) SELECT CAST(1 as BI" +
                "T) ELSE SELECT CAST(0 as BIT)"});
            this.comboBoxSQL.Location = new System.Drawing.Point(8, 8);
            this.comboBoxSQL.Name = "comboBoxSQL";
            this.comboBoxSQL.Size = new System.Drawing.Size(547, 21);
            this.comboBoxSQL.TabIndex = 5;
            this.comboBoxSQL.SelectedIndexChanged += new System.EventHandler(this.comboBoxSQL_SelectedIndexChanged);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(409, 329);
            this.dataGridView1.TabIndex = 0;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.labelRecordCount);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel6.Location = new System.Drawing.Point(0, 329);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(409, 19);
            this.panel6.TabIndex = 1;
            // 
            // labelRecordCount
            // 
            this.labelRecordCount.AutoSize = true;
            this.labelRecordCount.Location = new System.Drawing.Point(3, 3);
            this.labelRecordCount.Name = "labelRecordCount";
            this.labelRecordCount.Size = new System.Drawing.Size(51, 13);
            this.labelRecordCount.TabIndex = 0;
            this.labelRecordCount.Text = "0 records";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label6);
            this.panel4.Controls.Add(this.label5);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(120, 348);
            this.panel4.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 288);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Operation:";
            this.label6.Click += new System.EventHandler(this.label5_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 260);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Member Value:";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 234);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Index Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 207);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Federation:";
            // 
            // checkBoxAddFederationKey
            // 
            this.checkBoxAddFederationKey.AutoSize = true;
            this.checkBoxAddFederationKey.Location = new System.Drawing.Point(3, 117);
            this.checkBoxAddFederationKey.Name = "checkBoxAddFederationKey";
            this.checkBoxAddFederationKey.Size = new System.Drawing.Size(250, 17);
            this.checkBoxAddFederationKey.TabIndex = 10;
            this.checkBoxAddFederationKey.Text = "Add the Federation Member Value to the output";
            this.checkBoxAddFederationKey.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1096, 412);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Federation Sample UI";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelAPIOptions.ResumeLayout(false);
            this.panelAPIOptions.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonExecute;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxRootConnection;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxSQL;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox textBoxFederationName;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxMemberValue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBoxOperation;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ComboBox comboBoxSQL;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBoxIndex;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.CheckBox checkBoxPassThrough;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label labelRecordCount;
        private System.Windows.Forms.Panel panelAPIOptions;
        private System.Windows.Forms.CheckBox checkBoxAddFederationKey;
    }
}

