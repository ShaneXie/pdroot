namespace ShardSampleUI
{
    partial class FormAddShardConnection
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.radioButtonSSPI = new System.Windows.Forms.RadioButton();
            this.radioButtonSQLAccount = new System.Windows.Forms.RadioButton();
            this.buttonTest = new System.Windows.Forms.Button();
            this.buttonApplySchema = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxServer = new System.Windows.Forms.TextBox();
            this.textBoxDatabaseName = new System.Windows.Forms.TextBox();
            this.textBoxUID = new System.Windows.Forms.TextBox();
            this.textBoxPWD = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.labelGuid = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(36, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Database Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(66, 178);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "User Id:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(66, 201);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Password:";
            // 
            // radioButtonSSPI
            // 
            this.radioButtonSSPI.AutoSize = true;
            this.radioButtonSSPI.Checked = true;
            this.radioButtonSSPI.Location = new System.Drawing.Point(39, 121);
            this.radioButtonSSPI.Name = "radioButtonSSPI";
            this.radioButtonSSPI.Size = new System.Drawing.Size(71, 17);
            this.radioButtonSSPI.TabIndex = 2;
            this.radioButtonSSPI.TabStop = true;
            this.radioButtonSSPI.Text = "Use SSPI";
            this.radioButtonSSPI.UseVisualStyleBackColor = true;
            this.radioButtonSSPI.CheckedChanged += new System.EventHandler(this.radioButtonSSPI_CheckedChanged);
            // 
            // radioButtonSQLAccount
            // 
            this.radioButtonSQLAccount.AutoSize = true;
            this.radioButtonSQLAccount.Location = new System.Drawing.Point(39, 144);
            this.radioButtonSQLAccount.Name = "radioButtonSQLAccount";
            this.radioButtonSQLAccount.Size = new System.Drawing.Size(111, 17);
            this.radioButtonSQLAccount.TabIndex = 3;
            this.radioButtonSQLAccount.Text = "Use SQL Account";
            this.radioButtonSQLAccount.UseVisualStyleBackColor = true;
            this.radioButtonSQLAccount.CheckedChanged += new System.EventHandler(this.radioButtonSQLAccount_CheckedChanged);
            // 
            // buttonTest
            // 
            this.buttonTest.Location = new System.Drawing.Point(38, 264);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(141, 23);
            this.buttonTest.TabIndex = 6;
            this.buttonTest.Text = "Test Connection...";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // buttonApplySchema
            // 
            this.buttonApplySchema.Location = new System.Drawing.Point(38, 293);
            this.buttonApplySchema.Name = "buttonApplySchema";
            this.buttonApplySchema.Size = new System.Drawing.Size(141, 23);
            this.buttonApplySchema.TabIndex = 7;
            this.buttonApplySchema.Text = "Create Test Schema...";
            this.buttonApplySchema.UseVisualStyleBackColor = true;
            this.buttonApplySchema.Click += new System.EventHandler(this.buttonApplySchema_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(430, 293);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(108, 23);
            this.buttonOK.TabIndex = 9;
            this.buttonOK.Text = "Add to Shard";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(316, 293);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(108, 23);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(506, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Use this form to add a connection to the shard.  This connection will not be adde" +
                "d to the configuration file.";
            // 
            // textBoxServer
            // 
            this.textBoxServer.Location = new System.Drawing.Point(158, 58);
            this.textBoxServer.Name = "textBoxServer";
            this.textBoxServer.Size = new System.Drawing.Size(371, 20);
            this.textBoxServer.TabIndex = 0;
            this.textBoxServer.TextChanged += new System.EventHandler(this.textBoxServer_TextChanged);
            // 
            // textBoxDatabaseName
            // 
            this.textBoxDatabaseName.Location = new System.Drawing.Point(158, 86);
            this.textBoxDatabaseName.Name = "textBoxDatabaseName";
            this.textBoxDatabaseName.Size = new System.Drawing.Size(371, 20);
            this.textBoxDatabaseName.TabIndex = 1;
            this.textBoxDatabaseName.TextChanged += new System.EventHandler(this.textBoxDatabaseName_TextChanged);
            // 
            // textBoxUID
            // 
            this.textBoxUID.Enabled = false;
            this.textBoxUID.Location = new System.Drawing.Point(158, 175);
            this.textBoxUID.Name = "textBoxUID";
            this.textBoxUID.Size = new System.Drawing.Size(173, 20);
            this.textBoxUID.TabIndex = 4;
            this.textBoxUID.TextChanged += new System.EventHandler(this.textBoxUID_TextChanged);
            // 
            // textBoxPWD
            // 
            this.textBoxPWD.Enabled = false;
            this.textBoxPWD.Location = new System.Drawing.Point(158, 198);
            this.textBoxPWD.Name = "textBoxPWD";
            this.textBoxPWD.PasswordChar = '*';
            this.textBoxPWD.Size = new System.Drawing.Size(173, 20);
            this.textBoxPWD.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(35, 239);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Guid:";
            // 
            // labelGuid
            // 
            this.labelGuid.AutoSize = true;
            this.labelGuid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGuid.Location = new System.Drawing.Point(73, 239);
            this.labelGuid.Name = "labelGuid";
            this.labelGuid.Size = new System.Drawing.Size(0, 13);
            this.labelGuid.TabIndex = 0;
            // 
            // FormAddShardConnection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 334);
            this.Controls.Add(this.textBoxPWD);
            this.Controls.Add(this.textBoxUID);
            this.Controls.Add(this.textBoxDatabaseName);
            this.Controls.Add(this.textBoxServer);
            this.Controls.Add(this.buttonApplySchema);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonTest);
            this.Controls.Add(this.radioButtonSQLAccount);
            this.Controls.Add(this.radioButtonSSPI);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.labelGuid);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormAddShardConnection";
            this.Text = "Add Shard Connection";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton radioButtonSSPI;
        private System.Windows.Forms.RadioButton radioButtonSQLAccount;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.Button buttonApplySchema;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxServer;
        private System.Windows.Forms.TextBox textBoxDatabaseName;
        private System.Windows.Forms.TextBox textBoxUID;
        private System.Windows.Forms.TextBox textBoxPWD;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelGuid;
    }
}