using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OwnerDrawnListBox;

namespace Historia
{
    public partial class FormDetails : Form
    {
        String m_name;
        String m_date;
        String m_duration;
        String m_type;
        public FormDetails(String name, String date, String duration, String type)
        {
            m_name = name;
            m_date = date;
            m_duration = duration;
            m_type = type;
            InitializeComponent();
            addCustomListBox();
        }
        private void addCustomListBox()
        {
            // Create a new instance of DetailsListBox.
            DetailsListBox detailsListBox = new DetailsListBox();
            detailsListBox.Items.Add(m_type);
            detailsListBox.Items.Add(m_date);
            detailsListBox.Items.Add(m_duration);
            detailsListBox.Items.Add(m_name);
            detailsListBox.Parent = this;

            // Draw the bounds of the DetailsListBox.
            detailsListBox.Bounds = new Rectangle(5, 5, 150, 100);
            detailsListBox.Dock = DockStyle.Fill;
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            //go on to main form
            this.Owner.Enabled = true;  // go on to main form
            this.Owner.Show();
            this.Close();
        }
    }
}