using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FreshLogicStudios.Atlas.Mobile.PocketPC;      // for saving and reading app config file

namespace Historia
{
    public partial class SettingsForm : Form
    {
        private int callsCount;     // stores current calls number

        public SettingsForm(int callsCountPassed)
        {
            InitializeComponent();
            callsCount = callsCountPassed;  // set local calls number with the passed one
            numericUpDownFontSize.Value = int.Parse(ConfigurationManager.AppSettings["FontSize"]);  // load settings
            numericUpDownCallsLimit.Value = int.Parse(ConfigurationManager.AppSettings["CallsLimit"]);
            labelCurrentCallsValue.Text = callsCount.ToString();    // show current num of calls
            listBoxFont.SelectedItem = ConfigurationManager.AppSettings["Font"];
            listBoxFontType.SelectedItem = ConfigurationManager.AppSettings["FontType"];
            listBoxLayout.SelectedItem = ConfigurationManager.AppSettings["Layout"];
        }

        private void menuItemOk_Click(object sender, EventArgs e)   // ok was clicked
        {
            Cursor.Current = Cursors.WaitCursor;    // display wait cursor
            int callsLimit = int.Parse(numericUpDownCallsLimit.Value.ToString());  // save calls limit from input

            if (callsLimit < callsCount)   // if calls limit chosen is less than current number of calls
            {
                if (MessageBox.Show("This will delete all calls after call number " + numericUpDownCallsLimit.Value + ", are you sure ?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)  // warn the user
                {
                    // if user agreed to delete calls
                    ConfigurationManager.AppSettings["CallsLimit"] = callsLimit.ToString();   // save chosen limit to file
                    ConfigurationManager.AppSettings["FontSize"] = numericUpDownFontSize.Value.ToString();   //save font
                    ConfigurationManager.AppSettings["Font"] = (string)listBoxFont.SelectedItem;
                    ConfigurationManager.AppSettings["FontType"] = (string)listBoxFontType.SelectedItem;
                    if ((string)listBoxLayout.SelectedItem != ConfigurationManager.AppSettings["Layout"])
                    {
                        ConfigurationManager.AppSettings["Layout"] = (string)listBoxLayout.SelectedItem;
                        MessageBox.Show("Changes to layout will take effect after restarting the application.");
                    }
                    ConfigurationManager.Save();
                    this.Owner.Enabled = true;      // return to form1
                    this.Owner.Show();
                    Cursor.Current = Cursors.Default; // show default cursor
                    this.Close();

                }
                else // if user canceled restore back call limit from file
                {
                    Cursor.Current = Cursors.Default;    // display wait cursor
                    numericUpDownCallsLimit.Value = int.Parse(ConfigurationManager.AppSettings["CallsLimit"]);
                }
            }
            else // if calls limit larger than current calls limit
            {
                ConfigurationManager.AppSettings["CallsLimit"] = callsLimit.ToString();   // save settings
                ConfigurationManager.AppSettings["FontSize"] = numericUpDownFontSize.Value.ToString();   //
                ConfigurationManager.AppSettings["Font"] = (string)listBoxFont.SelectedItem;
                ConfigurationManager.AppSettings["FontType"] = (string)listBoxFontType.SelectedItem;
                if ((string)listBoxLayout.SelectedItem != ConfigurationManager.AppSettings["Layout"])
                {
                    ConfigurationManager.AppSettings["Layout"] = (string)listBoxLayout.SelectedItem;
                    MessageBox.Show("Changes to layout will take effect after restarting the application.");
                }
                ConfigurationManager.Save();

                this.Owner.Enabled = true;  // return to form1
                this.Owner.Show();
                Cursor.Current = Cursors.Default; // show default cursor
                this.Close();
            }



        }

        private void menuItemCancel_Click(object sender, EventArgs e)   // cancel clicked, return to form1
        {
            this.Owner.Enabled = true;
            this.Owner.Show();
            this.Close();
        }

    }
}