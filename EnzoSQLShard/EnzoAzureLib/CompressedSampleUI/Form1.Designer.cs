namespace CompressedSampleUI
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
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelRecordCount = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel6 = new System.Windows.Forms.Panel();
            this.comboBoxSQL = new System.Windows.Forms.ComboBox();
            this.checkBoxPassThrough = new System.Windows.Forms.CheckBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.textBoxSQL = new System.Windows.Forms.TextBox();
            this.textBoxMemberValue = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label3 = new System.Windows.Forms.Label();
            this.panelAPIOptions = new System.Windows.Forms.Panel();
            this.comboBoxIndex = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBoxOperation = new System.Windows.Forms.ComboBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxRootConnection = new System.Windows.Forms.TextBox();
            this.buttonConfigureShard = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.labelInfo = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonExecute = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelAPIOptions.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 287);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Operation:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 259);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Customer Key:";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label6);
            this.panel4.Controls.Add(this.label5);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(120, 395);
            this.panel4.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 233);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Index Name:";
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
            // labelRecordCount
            // 
            this.labelRecordCount.AutoSize = true;
            this.labelRecordCount.Location = new System.Drawing.Point(3, 3);
            this.labelRecordCount.Name = "labelRecordCount";
            this.labelRecordCount.Size = new System.Drawing.Size(51, 13);
            this.labelRecordCount.TabIndex = 0;
            this.labelRecordCount.Text = "0 records";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(352, 376);
            this.dataGridView1.TabIndex = 0;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.labelRecordCount);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel6.Location = new System.Drawing.Point(0, 376);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(352, 19);
            this.panel6.TabIndex = 1;
            // 
            // comboBoxSQL
            // 
            this.comboBoxSQL.Dock = System.Windows.Forms.DockStyle.Top;
            this.comboBoxSQL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSQL.FormattingEnabled = true;
            this.comboBoxSQL.Items.AddRange(new object[] {
            "--- REQUIRE API SETTINGS  - ON THE ROOT DATABASE ---",
            "SELECT * FROM TenantsDef",
            "SELECT * FROM serversdef",
            "--- REQUIRE API SETTINGS (Operation=FILTER_ON or FAN_OUT and Customer Key=CUST1 o" +
                "r CUST2) ---",
            "SELECT * FROM History",
            "SELECT * FROM History WHERE amount >= 100",
            "--- REQUIRES CHECKING THE \'USE DISTRIBUTED T-SQL ENGINE\' OPTION  ---",
            "SELECT MIN(amount), MAX(amount), COUNT(username), AVG(amount) USING (SELECT * FRO" +
                "M HISTORY) FEDERATED ON (tenants) "});
            this.comboBoxSQL.Location = new System.Drawing.Point(8, 8);
            this.comboBoxSQL.Name = "comboBoxSQL";
            this.comboBoxSQL.Size = new System.Drawing.Size(467, 21);
            this.comboBoxSQL.TabIndex = 5;
            this.comboBoxSQL.SelectedIndexChanged += new System.EventHandler(this.comboBoxSQL_SelectedIndexChanged);
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
            this.panel5.Size = new System.Drawing.Size(483, 165);
            this.panel5.TabIndex = 6;
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
            this.textBoxSQL.Size = new System.Drawing.Size(467, 128);
            this.textBoxSQL.TabIndex = 4;
            this.textBoxSQL.Text = "-- Sample statement to run on the Root database\r\nSELECT * FROM tenantsdef";
            // 
            // textBoxMemberValue
            // 
            this.textBoxMemberValue.Location = new System.Drawing.Point(3, 34);
            this.textBoxMemberValue.Name = "textBoxMemberValue";
            this.textBoxMemberValue.Size = new System.Drawing.Size(319, 20);
            this.textBoxMemberValue.TabIndex = 7;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(120, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.panelAPIOptions);
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxPassThrough);
            this.splitContainer1.Panel1.Controls.Add(this.panel5);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Panel2.Controls.Add(this.panel6);
            this.splitContainer1.Size = new System.Drawing.Size(839, 395);
            this.splitContainer1.SplitterDistance = 483;
            this.splitContainer1.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(8, 188);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(450, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "This options requires use of a modifed T-SQL. See http://enzosqlshard.codeplex.co" +
    "m for details.";
            // 
            // panelAPIOptions
            // 
            this.panelAPIOptions.Controls.Add(this.textBoxMemberValue);
            this.panelAPIOptions.Controls.Add(this.comboBoxIndex);
            this.panelAPIOptions.Controls.Add(this.label8);
            this.panelAPIOptions.Controls.Add(this.label9);
            this.panelAPIOptions.Controls.Add(this.label7);
            this.panelAPIOptions.Controls.Add(this.comboBoxOperation);
            this.panelAPIOptions.Location = new System.Drawing.Point(8, 223);
            this.panelAPIOptions.Name = "panelAPIOptions";
            this.panelAPIOptions.Size = new System.Drawing.Size(547, 130);
            this.panelAPIOptions.TabIndex = 11;
            // 
            // comboBoxIndex
            // 
            this.comboBoxIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxIndex.FormattingEnabled = true;
            this.comboBoxIndex.Location = new System.Drawing.Point(3, 8);
            this.comboBoxIndex.Name = "comboBoxIndex";
            this.comboBoxIndex.Size = new System.Drawing.Size(319, 21);
            this.comboBoxIndex.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(328, 36);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(132, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "(required with FILTER ON)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(328, 67);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(51, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "(required)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(328, 11);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "(optional)";
            // 
            // comboBoxOperation
            // 
            this.comboBoxOperation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxOperation.FormattingEnabled = true;
            this.comboBoxOperation.Items.AddRange(new object[] {
            "ROOT",
            "FILTER ON",
            "FAN OUT"});
            this.comboBoxOperation.Location = new System.Drawing.Point(1, 64);
            this.comboBoxOperation.Name = "comboBoxOperation";
            this.comboBoxOperation.Size = new System.Drawing.Size(321, 21);
            this.comboBoxOperation.TabIndex = 8;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.splitContainer1);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 34);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(959, 395);
            this.panel3.TabIndex = 10;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.textBoxRootConnection);
            this.panel2.Controls.Add(this.buttonConfigureShard);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(959, 34);
            this.panel2.TabIndex = 9;
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
            this.textBoxRootConnection.Enabled = false;
            this.textBoxRootConnection.Location = new System.Drawing.Point(126, 8);
            this.textBoxRootConnection.Name = "textBoxRootConnection";
            this.textBoxRootConnection.ReadOnly = true;
            this.textBoxRootConnection.Size = new System.Drawing.Size(549, 20);
            this.textBoxRootConnection.TabIndex = 2;
            this.textBoxRootConnection.UseSystemPasswordChar = true;
            // 
            // buttonConfigureShard
            // 
            this.buttonConfigureShard.BackColor = System.Drawing.SystemColors.Control;
            this.buttonConfigureShard.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonConfigureShard.Location = new System.Drawing.Point(797, 6);
            this.buttonConfigureShard.Name = "buttonConfigureShard";
            this.buttonConfigureShard.Size = new System.Drawing.Size(133, 23);
            this.buttonConfigureShard.TabIndex = 0;
            this.buttonConfigureShard.Text = "Configure Shard...";
            this.buttonConfigureShard.UseVisualStyleBackColor = false;
            this.buttonConfigureShard.Click += new System.EventHandler(this.buttonConfigureShard_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(681, 6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(110, 23);
            this.button2.TabIndex = 0;
            this.button2.Text = "Try Connection...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // labelInfo
            // 
            this.labelInfo.AutoSize = true;
            this.labelInfo.Location = new System.Drawing.Point(12, 8);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(0, 13);
            this.labelInfo.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelInfo);
            this.panel1.Controls.Add(this.buttonExecute);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 429);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(959, 30);
            this.panel1.TabIndex = 8;
            // 
            // buttonExecute
            // 
            this.buttonExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonExecute.Location = new System.Drawing.Point(847, 4);
            this.buttonExecute.Name = "buttonExecute";
            this.buttonExecute.Size = new System.Drawing.Size(97, 23);
            this.buttonExecute.TabIndex = 0;
            this.buttonExecute.Text = "Execute";
            this.buttonExecute.UseVisualStyleBackColor = true;
            this.buttonExecute.Click += new System.EventHandler(this.buttonExecute_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(731, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(110, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Build Index...";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(959, 459);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Compressed Shard Sample UI";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelAPIOptions.ResumeLayout(false);
            this.panelAPIOptions.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelRecordCount;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.ComboBox comboBoxSQL;
        private System.Windows.Forms.CheckBox checkBoxPassThrough;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TextBox textBoxSQL;
        private System.Windows.Forms.TextBox textBoxMemberValue;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panelAPIOptions;
        private System.Windows.Forms.ComboBox comboBoxIndex;
        private System.Windows.Forms.ComboBox comboBoxOperation;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxRootConnection;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonExecute;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonConfigureShard;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
    }
}

