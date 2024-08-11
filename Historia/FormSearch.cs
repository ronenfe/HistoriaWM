using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Historia
{
    public partial class FormSearch : Form
    {
        public string stringSearch;
        public FormSearch()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)  // search button clicked.
        {
            stringSearch = this.textBox1.Text;

            if (testPhoneNumber(stringSearch) == false)
            {

                MessageBox.Show("Enter Phone number only.");
            }
            else
            {
                stringSearch = stringSearch.Replace("*", "[*]");
                this.Owner.Enabled = true;  // return to form1
                this.Owner.Show();
                this.Close();
            }

        }
        bool testPhoneNumber(string str)   // test if text is a number
        {
            str = str.Trim();
            for (int i = 0; i < str.Length; i++)
            {
                if (char.IsNumber(str[i]) == false && str[i] != '*')
                    return false;
            }
            return true;
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            stringSearch = "";
            this.Owner.Enabled = true;  // return to form1
            this.Owner.Show();
            this.Close();
        }
    }
}