namespace Historia
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
            this.menuItemExit = new System.Windows.Forms.MenuItem();
            this.menuItemMenu = new System.Windows.Forms.MenuItem();
            this.menuItemBills = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItemSummary = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItemSettings = new System.Windows.Forms.MenuItem();
            this.menuItemAbout = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItemTypeFilter = new System.Windows.Forms.MenuItem();
            this.menuItemMissedFilter = new System.Windows.Forms.MenuItem();
            this.menuItemIncomingFilter = new System.Windows.Forms.MenuItem();
            this.menuItemOutgoingFilter = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItemSearch = new System.Windows.Forms.MenuItem();
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menuItemDetails = new System.Windows.Forms.MenuItem();
            this.menuItemSeparator3 = new System.Windows.Forms.MenuItem();
            this.menuItemCall = new System.Windows.Forms.MenuItem();
            this.menuItemSMS = new System.Windows.Forms.MenuItem();
            this.menuItemSeparator1 = new System.Windows.Forms.MenuItem();
            this.menuItemFilter = new System.Windows.Forms.MenuItem();
            this.menuItemCopyNumber = new System.Windows.Forms.MenuItem();
            this.menuItemContacts = new System.Windows.Forms.MenuItem();
            this.menuItemSeparator2 = new System.Windows.Forms.MenuItem();
            this.menuItemDelete = new System.Windows.Forms.MenuItem();
            this.menuItemDeleteAll = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // menuItemExit
            // 
            this.menuItemExit.Text = "Exit";
            this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
            // 
            // menuItemMenu
            // 
            this.menuItemMenu.MenuItems.Add(this.menuItemBills);
            this.menuItemMenu.MenuItems.Add(this.menuItem1);
            this.menuItemMenu.MenuItems.Add(this.menuItemSummary);
            this.menuItemMenu.MenuItems.Add(this.menuItem2);
            this.menuItemMenu.MenuItems.Add(this.menuItemSettings);
            this.menuItemMenu.MenuItems.Add(this.menuItemAbout);
            this.menuItemMenu.MenuItems.Add(this.menuItem3);
            this.menuItemMenu.MenuItems.Add(this.menuItemExit);
            this.menuItemMenu.Text = "Menu";
            // 
            // menuItemBills
            // 
            this.menuItemBills.Text = "Bills";
            this.menuItemBills.Click += new System.EventHandler(this.menuItemBills_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "-";
            // 
            // menuItemSummary
            // 
            this.menuItemSummary.Text = "Summary";
            this.menuItemSummary.Click += new System.EventHandler(this.menuItemSummary_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Text = "-";
            // 
            // menuItemSettings
            // 
            this.menuItemSettings.Text = "Settings";
            this.menuItemSettings.Click += new System.EventHandler(this.menuItemSettings_Click);
            // 
            // menuItemAbout
            // 
            this.menuItemAbout.Text = "Register/About";
            this.menuItemAbout.Click += new System.EventHandler(this.menuItemAbout_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Text = "-";
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItemMenu);
            this.mainMenu1.MenuItems.Add(this.menuItemTypeFilter);
            // 
            // menuItemTypeFilter
            // 
            this.menuItemTypeFilter.MenuItems.Add(this.menuItemMissedFilter);
            this.menuItemTypeFilter.MenuItems.Add(this.menuItemIncomingFilter);
            this.menuItemTypeFilter.MenuItems.Add(this.menuItemOutgoingFilter);
            this.menuItemTypeFilter.MenuItems.Add(this.menuItem5);
            this.menuItemTypeFilter.MenuItems.Add(this.menuItemSearch);
            this.menuItemTypeFilter.Text = "Filter";
            // 
            // menuItemMissedFilter
            // 
            this.menuItemMissedFilter.Text = "Missed";
            this.menuItemMissedFilter.Click += new System.EventHandler(this.menuItemMissedFilter_Click);
            // 
            // menuItemIncomingFilter
            // 
            this.menuItemIncomingFilter.Text = "Incoming";
            this.menuItemIncomingFilter.Click += new System.EventHandler(this.menuItemIncomingFilter_Click);
            // 
            // menuItemOutgoingFilter
            // 
            this.menuItemOutgoingFilter.Text = "Outgoing";
            this.menuItemOutgoingFilter.Click += new System.EventHandler(this.menuItemOutgoingFilter_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Text = "-";
            // 
            // menuItemSearch
            // 
            this.menuItemSearch.Text = "Search";
            this.menuItemSearch.Click += new System.EventHandler(this.menuItemSearch_Click);
            // 
            // contextMenu1
            // 
            this.contextMenu1.MenuItems.Add(this.menuItemDetails);
            this.contextMenu1.MenuItems.Add(this.menuItemSeparator3);
            this.contextMenu1.MenuItems.Add(this.menuItemCall);
            this.contextMenu1.MenuItems.Add(this.menuItemSMS);
            this.contextMenu1.MenuItems.Add(this.menuItemSeparator1);
            this.contextMenu1.MenuItems.Add(this.menuItemFilter);
            this.contextMenu1.MenuItems.Add(this.menuItemCopyNumber);
            this.contextMenu1.MenuItems.Add(this.menuItemContacts);
            this.contextMenu1.MenuItems.Add(this.menuItemSeparator2);
            this.contextMenu1.MenuItems.Add(this.menuItemDelete);
            this.contextMenu1.MenuItems.Add(this.menuItemDeleteAll);
            this.contextMenu1.Popup += new System.EventHandler(this.contextMenu1_Popup);
            // 
            // menuItemDetails
            // 
            this.menuItemDetails.Text = "Details";
            this.menuItemDetails.Click += new System.EventHandler(this.menuItemDetails_Click);
            // 
            // menuItemSeparator3
            // 
            this.menuItemSeparator3.Text = "-";
            // 
            // menuItemCall
            // 
            this.menuItemCall.Text = "Call";
            this.menuItemCall.Click += new System.EventHandler(this.menuItemCall_Click);
            // 
            // menuItemSMS
            // 
            this.menuItemSMS.Text = "SMS";
            this.menuItemSMS.Click += new System.EventHandler(this.menuItemSMS_Click);
            // 
            // menuItemSeparator1
            // 
            this.menuItemSeparator1.Text = "-";
            // 
            // menuItemFilter
            // 
            this.menuItemFilter.Text = "Filter";
            this.menuItemFilter.Click += new System.EventHandler(this.menuItemFilter_Click);
            // 
            // menuItemCopyNumber
            // 
            this.menuItemCopyNumber.Text = "Copy Number";
            this.menuItemCopyNumber.Click += new System.EventHandler(this.menuItemCopyNumber_Click);
            // 
            // menuItemContacts
            // 
            this.menuItemContacts.Text = "Save To Contacts";
            this.menuItemContacts.Click += new System.EventHandler(this.menuItemContacts_Click);
            // 
            // menuItemSeparator2
            // 
            this.menuItemSeparator2.Text = "-";
            // 
            // menuItemDelete
            // 
            this.menuItemDelete.Text = "Delete";
            this.menuItemDelete.Click += new System.EventHandler(this.menuItemDelete_Click);
            // 
            // menuItemDeleteAll
            // 
            this.menuItemDeleteAll.Text = "Delete All";
            this.menuItemDeleteAll.Click += new System.EventHandler(this.menuItemDeleteAll_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "All calls";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem menuItemExit;
        private System.Windows.Forms.MenuItem menuItemMenu;
        private System.Windows.Forms.MenuItem menuItemAbout;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.ContextMenu contextMenu1;
        private System.Windows.Forms.MenuItem menuItemCall;
        private System.Windows.Forms.MenuItem menuItemFilter;
        private System.Windows.Forms.MenuItem menuItemDelete;
        private System.Windows.Forms.MenuItem menuItemCopyNumber;
        private System.Windows.Forms.MenuItem menuItemSettings;
        private System.Windows.Forms.MenuItem menuItemTypeFilter;
        private System.Windows.Forms.MenuItem menuItemMissedFilter;
        private System.Windows.Forms.MenuItem menuItemIncomingFilter;
        private System.Windows.Forms.MenuItem menuItemOutgoingFilter;
        private System.Windows.Forms.MenuItem menuItemSMS;
        private System.Windows.Forms.MenuItem menuItemDeleteAll;
        private System.Windows.Forms.MenuItem menuItemSeparator1;
        private System.Windows.Forms.MenuItem menuItemSeparator2;
        private System.Windows.Forms.MenuItem menuItemContacts;
        private System.Windows.Forms.MenuItem menuItemBills;
        private System.Windows.Forms.MenuItem menuItemSummary;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem menuItemSearch;
        private System.Windows.Forms.MenuItem menuItemDetails;
        private System.Windows.Forms.MenuItem menuItemSeparator3;

    }
}

