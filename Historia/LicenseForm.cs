using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FreshLogicStudios.Atlas.Mobile.PocketPC;      // for saving and reading app config file
using Microsoft.Win32;
namespace Historia
{
    public partial class LicenseForm : Form
    {
        private const int TRIAL_PERIOD = 12;
        private UInt16 key;  // store the product key
        private DateTime firstTime;  // first datetime historia is running
        // The name of the key must include a valid root.
        private const string keyName = "HKEY_CURRENT_USER\\Software\\Historia";
        private int tryCount = 10;  // number of tries to enter a key.
        private TimeSpan daysPassed;
        private string keyString;
        public LicenseForm()
        {
            InitializeComponent();
            if (Registry.CurrentUser.OpenSubKey("Software\\Historia") == null)    //if  it's first time program is running, set registry key
            {
                Registry.CurrentUser.CreateSubKey("Software\\Historia");   // create historia2 subkey
                Registry.SetValue(keyName, "FirstTime", DateTime.Now.Ticks.ToString()); // first datetime historia is running
            }
            try
            {
                firstTime = new DateTime(long.Parse((string)Registry.GetValue(keyName, "FirstTime", "0")));
            }
            catch
            {
                Registry.SetValue(keyName, "FirstTime", "0"); // first datetime historia is running
            }
            string owner;
            try
            {
                if (System.Environment.OSVersion.Version.Major < 5) // if ppc03
                {
                    Byte[] ownerBytes = new Byte[100];   // get owner name
                    ownerBytes = (Byte[])Registry.CurrentUser.OpenSubKey("ControlPanel\\Owner").GetValue("Owner");
                    owner = System.Text.Encoding.Unicode.GetString(ownerBytes, 0, 72).TrimEnd('\0');
                }
                else
                    owner = Microsoft.WindowsMobile.Status.SystemState.OwnerName.ToString();
            }
            catch
            {
                owner = "";
            }
            if (owner.Length <= 10)
            {
                labelID.Text = owner;
            }
            else
            {
                labelID.Text = owner.Substring(0, 5) + owner.Substring(owner.Length - 5, 5);
            }
            key = 0;   // initialize key
            // generate key        
            rpn();
            keyString = key.ToString();
            keyString = keyString.PadLeft(5, '0');
            if (keyString != ConfigurationManager.AppSettings["KeyNumber"])    // if not registered
            {
                daysPassed = DateTime.Now - firstTime;
                this.buttonTrial.Visible = true;
                this.buttonOk.Visible = true;
                this.labelID.Visible = true;
                this.labelTextID.Visible = true;
                this.label1.Visible = true;
                this.textBoxKey.Visible = true;
                if (daysPassed.Days < TRIAL_PERIOD - 1)
                    labelTrial.Text = "Trial version, " + (TRIAL_PERIOD - daysPassed.Days) + " days left";
                else if (daysPassed.Days == TRIAL_PERIOD - 1)
                    labelTrial.Text = "Trial version, 1 day left";
                else   // if more than 12 days passed , disable Trial button
                {
                    this.buttonTrial.Enabled = false;
                    labelTrial.Text = "Trial version expired";
                    labelTrial.ForeColor = System.Drawing.Color.Red;
                }
                textBoxKey.Focus();
                this.Refresh();
            }

        }


        private void buttonOk_Click(object sender, EventArgs e) // register button is clicked
        {
            key = 0;   // initialize key
            string keyString;
            if (labelID.Text.Length == 0)   // if device id is empty
            {
                MessageBox.Show("To be able to enter a key you first have to enter your name in your device \"Owner Information\". Tap Start, Settings, Owner Information, and enter a name in the name field. Then run Historia again and you will see a Device ID. Send us the Device ID upon purchasing and we will send you the Key.");
            }
            else if (textBoxKey.Text != "") //if text is entered in key box
            {
                // generate key        
                rpn();
                keyString = key.ToString();
                keyString = keyString.PadLeft(5, '0');
                if (textBoxKey.Text == keyString) // if key entered is correct
                {
                    //save values to file
                    ConfigurationManager.AppSettings["KeyNumber"] = textBoxKey.Text;
                    ConfigurationManager.Save();

                    MessageBox.Show("Registration OK.");
                    //go on to main form
                    Cursor.Current = Cursors.WaitCursor;    // display wait cursor
                    this.Owner.Enabled = true;
                    this.Owner.Show();
                    this.Close();
                }
                else // if key is wrong
                {
                    tryCount--; //decrease trycount
                    if (tryCount == 0)
                    {
                        MessageBox.Show("You have reached maximum number of tries, application will now get closed.");
                        Application.Exit();
                    }
                    else
                        MessageBox.Show("Wrong key, try again.");
                }
            }
            else // if box is empty
            {
                MessageBox.Show("Please enter your key.");
            }
        }




        private void rpn()  // c 13 * key 6791 + * 6829 + i +
        {
            for (UInt16 i = 0; i < this.labelID.Text.Length; i++)  // run on each char of text
            {
                key += 6791;
                key *= (ushort)(this.labelID.Text[i] * 13);
                key += (ushort)(6829 + i);
            }
        }

        private void buttonTrial_Click(object sender, EventArgs e)
        {
            //go on to main form
            this.Owner.Enabled = true;  // go on to main form
            this.Owner.Show();
            this.Close();
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.sbsh.net", "");
        }

        private void linkLabel2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:support@sbsh.net", "");
        }

    }
}