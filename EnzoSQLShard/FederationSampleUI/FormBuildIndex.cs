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
    public partial class FormBuildIndex : Form
    {

        private RootDatabase _root_db = null;
        
        public FormBuildIndex()
        {
            InitializeComponent();
        }

        public FormBuildIndex(RootDatabase root_db, string sql, string federation)
        {
            InitializeComponent();

            _root_db = root_db;
            this.textBoxSQL.Text = sql;
            this.textBoxFederationName.Text = federation;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {

            try
            {

                if (_root_db[textBoxFederationName.Text].MemberIndexes.ContainsKey(this.textBoxName.Text))
                {
                    if (System.Windows.Forms.MessageBox.Show(this, "This index already exists. Rebuild?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
                        return;
                }

                _root_db[textBoxFederationName.Text].MemberIndexes.Add(this.textBoxName.Text, this.textBoxSQL.Text);

                if (checkBoxBuildNow.Checked)
                    _root_db[textBoxFederationName.Text].MemberIndexes[this.textBoxName.Text].Build();

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
