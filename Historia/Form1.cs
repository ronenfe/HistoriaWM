
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using OpenNETCF.Data.Text;  //csv to datatable
using OpenNETCF.Phone;      // phone features
using System.Globalization;
using FreshLogicStudios.Atlas.Mobile.PocketPC;  // configuration manager    
using Microsoft.WindowsMobile.PocketOutlook;    // sending messages
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using OwnerDrawnListBox;

namespace Historia
{

    public partial class Form1 : Form
    {
        private string lastTitle = "All Calls";
        public bool filteredByType = false;
        public bool filteredByNumber = false;
        private bool gridView = ConfigurationManager.AppSettings["Layout"] == "List" ? false : true;
        private UInt16 key;  // for storing the product key
        private LicenseForm licenseForm; // declaring the license form
        private bool flagRegistered;    // stores registered flag
        private int lastSelectedRow = 0;    // stoer the last selected row so we can unselect it when a new row is selected
        private string lastSelectedSN = "";
        private DataSet dsCalls;    // stores the dataset
        private TextDataAdapter daHistoriaCalls;    // gets the data from the csv
        public DataTable dtHistoriaCalls;      // stores the data table
        public DataTable dtNumberFiltered; // stores the filtered datatable
        public DataTable dtTypedFiltered; // stores the filtered by type table
        private CallLog entries;    // store the calls from the built in call log
        private MenuItem menuItemBack;  // back option when we are in the filtered by number
        private DataGrid dataGridCalls;

        private DataColumn[] keys = new DataColumn[1];      // primary key for datatable
        private DataColumn[] keysTyped = new DataColumn[1];      // primary key for datatable typed filtered
        private DataGridTableStyle dtStyle = new DataGridTableStyle();  //create custom style for the datagrid
        private long lastCallTime;
        private string filteredNumber;
        private CallsListBox callsListBox;
        FormSearch formSearch;   // create new search form         

        //        private Timer m_refreshTimer = new Timer();

        public Form1()
        {

            InitializeComponent();
            this.dataGridCalls = new DataGrid();
            this.menuItemBack = new System.Windows.Forms.MenuItem();    // create the back option for the filtered by by number 
            this.menuItemBack.Text = "Back";
            this.menuItemBack.Click += new System.EventHandler(this.menuItemBack_Click);

            flagRegistered = registered();  // registered flag
            if (flagRegistered == false)  // if software is not registered display license form
            {
                this.Text = "Register";
                this.mainMenu1.MenuItems.Remove(menuItemMenu);  // hide menus
                this.mainMenu1.MenuItems.Remove(menuItemTypeFilter);
                this.licenseForm = new LicenseForm();   // create  a license form
                this.licenseForm.Closed += new EventHandler(licenseForm_Closed);    // add a closed event
                this.licenseForm.Owner = this;  // declare the owner of the license form
                this.licenseForm.Show();    // show license form
                this.Enabled = false;   // disable main form 
            }
            else // if software is registered
            {
                menuItemAbout.Text = "About";
                initializeList();
            }
        }
        private void getCsvData()   // get data from csv file
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;    // display wait cursor
                bool csvExists = true; // flag to indicate if csv was existing
                string targetFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Historia"; //where to save and get the csv file (my documents\historia)
                if (Directory.Exists(targetFolder) == false)    // if directory does not exists, create it
                    Directory.CreateDirectory(targetFolder);
                string historiaFileName = Path.Combine(targetFolder, "Historia.txt");   // csv filename



                if (!File.Exists(historiaFileName)) // if csv file does not exists , create a new one
                {
                    using (Stream myStream = File.Open(historiaFileName, FileMode.Append, FileAccess.Write))
                    {
                        StreamWriter sw = new StreamWriter(myStream);

                        sw.WriteLine(" " + ',' + "Number" + ','
                            + "Name" + ',' + "Time" + ',' + "Duration" + ',' + "Type" + ',' + "S/N"); // header line
                        sw.Flush();
                    }
                    csvExists = false;
                }

                dsCalls = new DataSet("Calls"); // new dataset
                daHistoriaCalls = new TextDataAdapter(historiaFileName, true, ','); // get csv data
                daHistoriaCalls.ForceWrite = true;  // this will make the csv file get updated when rows are deleted from datatable
                daHistoriaCalls.Fill(dsCalls, "Historia Calls");    // store it in the dataset
                dtHistoriaCalls = dsCalls.Tables["Historia Calls"]; // assign the datatable to the data retrived from the csv
                keys[0] = dtHistoriaCalls.Columns["S/N"];  //primary key will be the time column
                dtHistoriaCalls.PrimaryKey = keys;

                dataGridCalls.DataSource = dtHistoriaCalls; // display the data in the data grid

                if (ConfigurationManager.AppSettings["LastCall"] == "0") // if it's the first time software is running
                {
                    if (csvExists == true && dtHistoriaCalls.Rows.Count != 0) // if file csv was existing (from previous versions) and its's not empty
                        lastCallTime = long.Parse((string)dtHistoriaCalls.Rows[0]["S/N"] + "0000000"); // make the first call in the file the lastcall so you can update the new calls from call log
                    else // if csv wasn't existing or is empty
                        lastCallTime = 0;   // set lastcalltime to early date so file will be updated with all calls from call log                
                }
                else    // if it's not the first time the program is running
                {
                    if (csvExists == true)  // csv file was existing
                        lastCallTime = long.Parse(ConfigurationManager.AppSettings["LastCall"]); // get it from file
                    else
                        lastCallTime = 0;   // set lastcalltime to early date so file will be updated with all calls from call log
                }
                dtTypedFiltered = dtHistoriaCalls.Clone();  // clone table to the table used for filtered by type
                dtNumberFiltered = dtHistoriaCalls.Clone(); // clone table to the table used for filtered by number

                Cursor.Current = Cursors.Default;   // display default cursor
            }
            catch
            {
                if (MessageBox.Show("There was an error getting data from Historia.txt. you can try to launch the application again. if  it doesn't help, you can delete the file by tapping Yes. The old file will be saved as HistoriaBackup.txt for backup purpuses. If error persists please contact support or reinstall the software. Do you want to delete the file?", "confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    string targetFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Historia"; //where to save and get the csv file (my documents\historia)
                    string historiaFileName = Path.Combine(targetFolder, "Historia.txt"); // file name of calls
                    string historiaFileNameBackup = Path.Combine(targetFolder, "HistoriaBackup.txt"); // file name of calls
                    if (File.Exists(historiaFileName))
                    {
                        File.Copy(historiaFileName, historiaFileNameBackup, true);
                        File.Delete(historiaFileName);
                    }

                }
                MessageBox.Show("Application will now exit.");
                Application.Exit();
            }


        }
        private void setStyle()     // set datagrid style
        {
            try
            {
                string[] headers = { " ", "Number", "Name", "Time", "Duration", "Type", "S/N" };   //  for changing header styles

                dtStyle.MappingName = "Historia Calls";     // map style to the data table
                for (int i = 0; i < dtHistoriaCalls.Columns.Count; i++) // run on each column
                {
                    ColumnStyle myStyle = new ColumnStyle(i);   // create a new column style
                    myStyle.MappingName = dtHistoriaCalls.Columns[i].ColumnName;    // map the new style to the column
                    myStyle.HeaderText = headers[i];    // set header text
                    if (i == 0) // if column is the first
                    {
                        myStyle.CheckCellEquals += new CheckCellEventHandler(myStyle_isEqual); // create event to paint the cells in first column
                    }
                    myStyle.Width = int.Parse(ConfigurationManager.AppSettings["ColWidth" + i]); // column width will be taken from the app.config

                    dtStyle.GridColumnStyles.Add(myStyle);  // add column style to the new data table style
                }

                dataGridCalls.TableStyles.Add(dtStyle); // add style to the datagrid

                if (ConfigurationManager.AppSettings["FontType"] == "Regular")      // set font from app.config
                    dataGridCalls.Font = new System.Drawing.Font(ConfigurationManager.AppSettings["Font"], float.Parse(ConfigurationManager.AppSettings["FontSize"]), System.Drawing.FontStyle.Regular);
                else if (ConfigurationManager.AppSettings["FontType"] == "Bold")
                    dataGridCalls.Font = new System.Drawing.Font(ConfigurationManager.AppSettings["Font"], float.Parse(ConfigurationManager.AppSettings["FontSize"]), System.Drawing.FontStyle.Bold);
                else
                    dataGridCalls.Font = new System.Drawing.Font(ConfigurationManager.AppSettings["Font"], float.Parse(ConfigurationManager.AppSettings["FontSize"]), System.Drawing.FontStyle.Italic);
            }
            catch
            {
                MessageBox.Show("There was an error setting table styles. you can try to launch the application again. if  it doesn't help, please contact support or reinstall the software");
                MessageBox.Show("Application will now exit.");
                Application.Exit();
            }
        }

        private void updateCsv()    // update file and table
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;    // display wait cursor


                entries = new CallLog();   //stores the call entries
                string duration = null; //stores call duration
                string type = null; //stores type of call
                string paddedSeconds;   // duration seconds padded with zeros
                string paddedMinutes;   // duration minutes padded with zeros
                string name;    // stores name
                string number;  // stores number
                int callsLimit = int.Parse(ConfigurationManager.AppSettings["CallsLimit"]);     // stores max number of calls in file
                int i = 0;  // is is a counter for the for loop
                //                     ^ entries[i] exists and i not reached the end of call log and entry from call log is newer than entry from table)
                for (; entries[i] != null && i < entries.Count && entries[i].StartTime.Ticks > lastCallTime; i++)   // run on call log  and update the begining of the table  if newer calll were made, stops when reached the end of the newer calls
                {

                    TimeSpan span = entries[i].EndTime.Subtract(entries[i].StartTime);  // duration calculation
                    paddedSeconds = span.Seconds.ToString().PadLeft(2, '0'); // pad seconds with a zero
                    paddedMinutes = span.Minutes.ToString().PadLeft(2, '0');    // pad minutes with a zero

                    if (span.Hours == 0)   // if call is less then a hour, don't display hours
                        duration = paddedMinutes + ":" + paddedSeconds;
                    else    // else display hours
                        duration = span.Hours + ":" + paddedMinutes + ":" + paddedSeconds;

                    // display type of call:
                    if (entries[i].CallType.ToString() == "Outgoing") type = "Outgoing";
                    else if (entries[i].Connected == true) type = "Incoming";
                    else type = "Missed";

                    name = (entries[i].Name != null) ? entries[i].Name : "N/A";  // if no name exists, display n/a
                    if (entries[i].Number == null) // if no number exists, display n/a
                        number = "N/A";
                    else
                    {
                        number = Regex.Replace(entries[i].Number, "\\D", "");
                        if (entries[i].Number[0] == '*')  // if star is at the begining
                            number = '*' + number;
                    }
                    DataRow row = dtHistoriaCalls.NewRow(); // create a new row
                    row[" "] = " ";
                    row["Number"] = number;
                    row["Name"] = name;
                    row["Time"] = entries[i].StartTime.ToShortDateString() + ", " + entries[i].StartTime.ToShortTimeString();
                    row["Duration"] = duration;
                    row["Type"] = type;
                    row["S/N"] = entries[i].StartTime.Ticks.ToString().Substring(0, 11);
                    dtHistoriaCalls.Rows.InsertAt(row, i);  // insert the row
                }

                while (callsLimit < dtHistoriaCalls.Rows.Count)     //  truncate list to the size defined by user
                    dtHistoriaCalls.Rows.Remove(dtHistoriaCalls.Rows[callsLimit]);
                if (i != 0) // if new rows were added
                {
                    lastCallTime = long.Parse(dtHistoriaCalls.Rows[0]["S/N"] + "0000000");     // update lastcall
                    ConfigurationManager.AppSettings["LastCall"] = lastCallTime.ToString(); // save first call to app config so it won't get back after deletion and refreshing
                    ConfigurationManager.Save();
                    daHistoriaCalls.Update(dsCalls, "Historia Calls");  // update the csv file 
                }

                Cursor.Current = Cursors.Default;   // display default cursor


            }
            catch
            {
                if (MessageBox.Show("There was an error updating Historia.txt. you can try to launch the application again. if  it doesn't help, you can delete the file by tapping Yes. The old file will be saved as HistoriaBackup.txt for backup purpuses. If error persists please contact support or reinstall the software. Do you want to delete the file?", "confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    string targetFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Historia"; //where to save and get the csv file (my documents\historia)
                    string historiaFileName = Path.Combine(targetFolder, "Historia.txt"); // file name of calls
                    string historiaFileNameBackup = Path.Combine(targetFolder, "HistoriaBackup.txt"); // file name of calls
                    if (File.Exists(historiaFileName))
                    {
                        File.Copy(historiaFileName, historiaFileNameBackup, true);
                        File.Delete(historiaFileName);
                    }

                }
                MessageBox.Show("Application will now exit.");
                Application.Exit();
            }
        }

        private bool registered()   // check key to see if application is registered
        {
            try
            {
                string keyString;
                key = 0;    // initialize key
                rpn();  // run algorithm
                keyString = key.ToString();
                keyString = keyString.PadLeft(5, '0');

                if (ConfigurationManager.AppSettings["KeyNumber"] == keyString && key != 0)    // check if key in app file is the correct key
                {
                    return true;
                }
            }
            catch
            {
                MessageBox.Show("Error checking registration. Make sure you entered an owner information in your device.");
                Application.Exit();
            }

            return false;
        }

        private void menuItemAbout_Click(object sender, EventArgs e)    // about menu
        {
            lastTitle = this.Text;
            if (flagRegistered == true)
                this.Text = "About";
            else
                this.Text = "Register";
            this.mainMenu1.MenuItems.Clear();
            this.licenseForm = new LicenseForm();   // create  a license form
            this.licenseForm.Closed += new EventHandler(licenseForm_Closed);    // add a closed event
            this.licenseForm.Owner = this;  // declare the owner of the license form
            this.licenseForm.Show();    // show license form
            this.Enabled = false;   // disable main form 
        }


        private void menuItemBack_Click(object sender, EventArgs e)     // back menu
        {
            try
            {
                filteredByNumber = false;
                dtNumberFiltered.Clear();   // clear filtered by number menu

                if (filteredByType == true)   // if user was in filtered by type, return to it
                {
                    if (gridView == true)
                        dataGridCalls.DataSource = dtTypedFiltered;
                    else
                        reloadCalls(dtTypedFiltered);
                    if (menuItemMissedFilter.Checked == true)   // if filtered to missed
                        this.Text = "Missed calls";
                    else if (menuItemOutgoingFilter.Checked == true)    // if filtered to outgoing
                        this.Text = "Outgoing calls";
                    else
                        typeFilter("Incoming calls");
                }
                else //if user was in non filtered table
                {
                    if (gridView == true)
                        dataGridCalls.DataSource = dtHistoriaCalls; // display the data in the datagrid
                    else
                        reloadCalls(dtHistoriaCalls);
                    this.Text = "All calls";
                }

                this.mainMenu1.MenuItems.Remove(this.menuItemBack); // remove back menu
                this.mainMenu1.MenuItems.Add(menuItemTypeFilter);   // add filter menu item back to context menu
            }
            catch
            {
                MessageBox.Show("Error going back from filered by number.");
                Application.Exit();
            }

        }
        private void licenseForm_Closed(object sender, EventArgs e) // if license form is closed
        {
            this.Text = lastTitle;
            this.mainMenu1.MenuItems.Add(menuItemMenu); // show menu back
            if (filteredByNumber == false)
                this.mainMenu1.MenuItems.Add(menuItemTypeFilter);   // add filter menu back
            else
                this.mainMenu1.MenuItems.Add(menuItemBack);
            if (callsListBox == null)
                initializeList();
        }
        private void detailsForm_Closed(object sender, EventArgs e) // if details form is closed
        {
            this.Text = lastTitle;
        }
        private void menuItemExit_Click(object sender, EventArgs e) // exit clicked
        {
            try
            {
                if (gridView == true)
                {
                    for (int i = 0; i < 5; i++) // run on each column
                    {
                        ConfigurationManager.AppSettings["ColWidth" + i] = dtStyle.GridColumnStyles[i].Width.ToString(); // save column width to file
                    }
                    ConfigurationManager.Save();    // save
                }
                Application.Exit(); // exit
            }
            catch
            {
                MessageBox.Show("Error exiting.");
                Application.Exit();
            }
        }

        private void contextMenu1_Popup(object sender, EventArgs e) // when context menu is opening
        {
            try
            {
                contextMenu1.MenuItems.Clear(); // clear menu items

                if (gridView == true)
                {
                    DataGrid.HitTestInfo hti = dataGridCalls.HitTest(MousePosition.X, MousePosition.Y - 25);     // get selected row and column

                    if (hti.Row != -1)  // if a row is tapped
                    {

                        if (filteredByNumber == true)   //  if source is filtered by number add the following menu items
                        {
                            contextMenu1.MenuItems.Add(menuItemDetails);
                            contextMenu1.MenuItems.Add(menuItemSeparator1);
                            contextMenu1.MenuItems.Add(menuItemCall);
                            contextMenu1.MenuItems.Add(menuItemSMS);
                            contextMenu1.MenuItems.Add(menuItemContacts);
                            contextMenu1.MenuItems.Add(menuItemSeparator2);
                            contextMenu1.MenuItems.Add(menuItemCopyNumber);
                            contextMenu1.MenuItems.Add(menuItemSeparator3);
                            contextMenu1.MenuItems.Add(menuItemDelete);
                            contextMenu1.MenuItems.Add(menuItemDeleteAll);
                        }
                        else  // else add the folowing items
                        {
                            contextMenu1.MenuItems.Add(menuItemDetails);
                            contextMenu1.MenuItems.Add(menuItemSeparator1);
                            contextMenu1.MenuItems.Add(menuItemCall);
                            contextMenu1.MenuItems.Add(menuItemSMS);
                            contextMenu1.MenuItems.Add(menuItemContacts);
                            contextMenu1.MenuItems.Add(menuItemSeparator2);
                            contextMenu1.MenuItems.Add(menuItemCopyNumber);
                            contextMenu1.MenuItems.Add(menuItemFilter);
                            contextMenu1.MenuItems.Add(menuItemSeparator3);
                            contextMenu1.MenuItems.Add(menuItemDelete);
                            contextMenu1.MenuItems.Add(menuItemDeleteAll);
                        }

                        dataGridCalls.UnSelect(lastSelectedRow);    //unselect last selected row
                        dataGridCalls.CurrentCell = new DataGridCell(hti.Row, 0);   //select the pressed row
                        dataGridCalls.Select(hti.Row);  // select new row
                        this.lastSelectedRow = hti.Row; // set new row
                        this.lastSelectedSN = dataGridCalls[hti.Row, 6].ToString();

                    }
                }
                else
                {
                    if ( callsListBox.Items.Count == 0 )
                        return;
                    Point pt = callsListBox.PointToClient(Control.MousePosition);
                    callsListBox.IndexFromPoint(pt);
                    this.lastSelectedRow = callsListBox.SelectedIndex; // set new row
                    this.lastSelectedSN = callsListBox.Items[this.lastSelectedRow].ToString();
                    if (callsListBox.SelectedIndex >= 0)
                    {
                        if (filteredByNumber == true)   //  if source is filtered by number add the following menu items
                        {
                            contextMenu1.MenuItems.Add(menuItemDetails);
                            contextMenu1.MenuItems.Add(menuItemSeparator1);
                            contextMenu1.MenuItems.Add(menuItemCall);
                            contextMenu1.MenuItems.Add(menuItemSMS);
                            contextMenu1.MenuItems.Add(menuItemContacts);
                            contextMenu1.MenuItems.Add(menuItemSeparator2);
                            contextMenu1.MenuItems.Add(menuItemCopyNumber);
                            contextMenu1.MenuItems.Add(menuItemSeparator3);
                            contextMenu1.MenuItems.Add(menuItemDelete);
                            contextMenu1.MenuItems.Add(menuItemDeleteAll);
                        }
                        else  // else add the folowing items
                        {
                            contextMenu1.MenuItems.Add(menuItemDetails);
                            contextMenu1.MenuItems.Add(menuItemSeparator1);
                            contextMenu1.MenuItems.Add(menuItemCall);
                            contextMenu1.MenuItems.Add(menuItemSMS);
                            contextMenu1.MenuItems.Add(menuItemContacts);
                            contextMenu1.MenuItems.Add(menuItemSeparator2);
                            contextMenu1.MenuItems.Add(menuItemCopyNumber);
                            contextMenu1.MenuItems.Add(menuItemFilter);
                            contextMenu1.MenuItems.Add(menuItemSeparator3);
                            contextMenu1.MenuItems.Add(menuItemDelete);
                            contextMenu1.MenuItems.Add(menuItemDeleteAll);
                        }
                    }
                }

            }
            catch
            {
                MessageBox.Show("Error in context menu.");
            }

        }

        private void menuItemCopyNumber_Click(object sender, EventArgs e)   // copy number to clipboard
        {
            try
            {
                Clipboard.SetDataObject(dtHistoriaCalls.Rows.Find(lastSelectedSN)[1].ToString());
            }
            catch
            {
                MessageBox.Show("Error copying number");
            }
        }

        private void menuItemCall_Click(object sender, EventArgs e) // call a number
        {
            try
            {
                Microsoft.WindowsMobile.Telephony.Phone ph = new Microsoft.WindowsMobile.Telephony.Phone();
                ph.Talk(dtHistoriaCalls.Rows.Find(lastSelectedSN)[1].ToString());
            }
            catch
            {
                MessageBox.Show("Error calling number.");
            }
        }

        private void menuItemFilter_Click(object sender, EventArgs e)   // filter by number
        {
            try
            {
                filteredNumber = dtHistoriaCalls.Rows.Find(lastSelectedSN)[1].ToString();   // get number to be filtered
                lastSelectedRow = 0;    //  unselect the row
                numberFilter();   // filter by number
                this.mainMenu1.MenuItems.Remove(menuItemTypeFilter);    // remove filter menu
                this.mainMenu1.MenuItems.Add(this.menuItemBack);    // add back menu
            }
            catch
            {
                MessageBox.Show("Error filtering a number.");
            }
        }

        private void menuItemDelete_Click(object sender, EventArgs e)       // delete a number
        {
            try
            {
                if (filteredByNumber == false && filteredByType == false)    // if table is not filtered
                {
                    dtHistoriaCalls.Rows.Remove(dtHistoriaCalls.Rows[this.lastSelectedRow]);   // delete selected row
                }
                else if (filteredByNumber == true)  // if table is filtered by number
                {
                    DataRow tempRow = dtHistoriaCalls.Rows.Find(lastSelectedSN);
                    if (tempRow == null)    // if row wasn't found
                    {
                        MessageBox.Show("Call was not found in file");
                        return;
                    }
                    dtHistoriaCalls.Rows.Remove(tempRow);   // remove row in original table

                    if (filteredByType == true) // if was filtered by type
                    {
                        tempRow = dtTypedFiltered.Rows.Find(lastSelectedSN);
                        if (tempRow == null)    // if row wasn't found
                        {
                            MessageBox.Show("Call was not found in typed filtered table");
                            return;
                        }
                        dtTypedFiltered.Rows.Remove(tempRow);
                    }

                    dtNumberFiltered.Rows.Remove(dtNumberFiltered.Rows[this.lastSelectedRow]); // delete row
                }
                else // if filtered by type
                {
                    DataRow tempRow = dtHistoriaCalls.Rows.Find(lastSelectedSN);
                    if (tempRow == null)    // if row wasn't found
                    {
                        MessageBox.Show("Call was not found in file");
                        return;
                    }
                    dtHistoriaCalls.Rows.Remove(tempRow);   //find location of row in original table
                    dtTypedFiltered.Rows.Remove(dtTypedFiltered.Rows[lastSelectedRow]);   // delete row
                }
                lastSelectedRow = 0;    // unselect row

                daHistoriaCalls.Update(dsCalls, "Historia Calls");  // update the csv file
                if (gridView == false)
                {
                    if (filteredByNumber == true && filteredByType == false)
                        reloadCalls(dtNumberFiltered);
                    else if (filteredByNumber == false && filteredByType == true)
                        reloadCalls(dtTypedFiltered);
                    else
                        reloadCalls(dtHistoriaCalls);
                }
            }
            catch
            {
                MessageBox.Show("Error deleting a number.");
            }
        }
        private void calls_GotFocus(object sender, EventArgs e) // if window is shown again after user swicthed to other windows, refresh display
        {
            try
            {
                entries = new CallLog();    // get updated call list

                if (entries[0] != null && entries[0].StartTime.Ticks > lastCallTime)    // if table is empty or new calls were made
                {
                    updateCsv(); // create list again


                    if (filteredByType == true)    // if table is filtered by type
                    {
                        if (menuItemMissedFilter.Checked == true)   // if filtered to missed
                            typeFilter("Missed");   //filter again
                        else if (menuItemOutgoingFilter.Checked == true)    // if filtered to outgoing
                            typeFilter("Outgoing"); // filter again
                        else if (menuItemIncomingFilter.Checked == true)   // if filtered to incoming
                            typeFilter("Incoming");

                        if (filteredByNumber == true)   // if filtered by number
                        {
                            numberFilter();   // filter by number again
                        }
                    }
                    else if (gridView == false)
                    {
                        if (filteredByNumber == true && filteredByType == false)
                            reloadCalls(dtNumberFiltered);
                        else if (filteredByNumber == false && filteredByType == true)
                            reloadCalls(dtTypedFiltered);
                        else
                            reloadCalls(dtHistoriaCalls);
                    }

                }
            }
            catch
            {
                MessageBox.Show("Error in automatic refresh.");
                MessageBox.Show("Application will now exit.");
                Application.Exit();
            }
        }

        public void myStyle_isEqual(object sender, DataGridEnableEventArgs e)   // function to draw style 
        {
            if ((string)dataGridCalls[e.Row, 5] == "Outgoing")  // if outgoing
                e.MeetsCriteria = meetsCriteriaEnum.outgoing;   // mark as outgoing
            else if ((string)dataGridCalls[e.Row, 5] == "Incoming") // if incoming
                e.MeetsCriteria = meetsCriteriaEnum.incoming;   // mark as incoming
            else
                e.MeetsCriteria = meetsCriteriaEnum.missed; //mark as missed


        }

        private void menuItemSettings_Click(object sender, EventArgs e) // if settings menu is clicked
        {
            try
            {
                SettingsForm settingsForm = new SettingsForm(dtHistoriaCalls.Rows.Count);   // create new setting form
                settingsForm.Closed += new EventHandler(settingsForm_Closed);    // add a closed event
                settingsForm.Owner = this;  // declare the owner of the settings form
                settingsForm.Show();    // show settings form
                this.Enabled = false;   // disable main form 
            }
            catch
            {
                MessageBox.Show("Error in settings form.");
            }

        }
        private void settingsForm_Closed(object sender, EventArgs e) // if settigns form is closed
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                int callsLimit = int.Parse(ConfigurationManager.AppSettings["CallsLimit"]); // get call limit from file
                if (callsLimit < dtHistoriaCalls.Rows.Count)    // if call limit is less then current number of calls
                {
                    while (callsLimit < dtHistoriaCalls.Rows.Count) // loop and delete all lines after the call limit location
                        dtHistoriaCalls.Rows.Remove(dtHistoriaCalls.Rows[callsLimit]);

                    daHistoriaCalls.Update(dsCalls, "Historia Calls");  // update the csv file

                    if (filteredByType == true)    // if table is filtered by type
                    {
                        if (menuItemMissedFilter.Checked == true)   // if filtered to missed
                            typeFilter("Missed");   //filter again
                        else if (menuItemOutgoingFilter.Checked == true)    // if filtered to outgoing
                            typeFilter("Outgoing"); // filter again
                        else    // if filtered to incoming
                            typeFilter("Incoming");
                    }
                    if (dtHistoriaCalls.Rows.Count != 0 && filteredByNumber == true)   // if filtered by number
                    {
                        numberFilter();   // filter by number again
                    }
                }

                //-------setting the font----------
                if (gridView == true)
                {
                    if (ConfigurationManager.AppSettings["FontType"] == "Regular")
                    {
                        dataGridCalls.Font = new System.Drawing.Font(ConfigurationManager.AppSettings["Font"], float.Parse(ConfigurationManager.AppSettings["FontSize"]), System.Drawing.FontStyle.Regular);
                    }
                    else if (ConfigurationManager.AppSettings["FontType"] == "Bold")
                    {
                        dataGridCalls.Font = new System.Drawing.Font(ConfigurationManager.AppSettings["Font"], float.Parse(ConfigurationManager.AppSettings["FontSize"]), System.Drawing.FontStyle.Bold);
                    }
                    else
                    {
                        dataGridCalls.Font = new System.Drawing.Font(ConfigurationManager.AppSettings["Font"], float.Parse(ConfigurationManager.AppSettings["FontSize"]), System.Drawing.FontStyle.Italic);
                    }
                }
                else
                {
                    this.Controls.Remove(callsListBox);
                    callsListBox.Dispose();
                    // Create a new instance of FontListBox.
                    callsListBox = new CallsListBox(this);
                    callsListBox.Parent = this;

                    // Draw the bounds of the FontListBox.
                    callsListBox.Bounds = new Rectangle(5, 5, 150, 100);
                    callsListBox.Dock = DockStyle.Fill;
                    if (filteredByNumber == true && filteredByType == false)
                        reloadCalls(dtNumberFiltered);
                    else if (filteredByNumber == false && filteredByType == true)
                        reloadCalls(dtTypedFiltered);
                    else
                        reloadCalls(dtHistoriaCalls);
                    callsListBox.ContextMenu = contextMenu1;
                }
                Cursor.Current = Cursors.Default;
            }
            catch
            {
                MessageBox.Show("Error in settings form closed event.");
                MessageBox.Show("Application will now exit.");
                Application.Exit();
            }
        }

        private void menuItemMissedFilter_Click(object sender, EventArgs e)    // filter by missed clicked
        {
            try
            {
                if (menuItemMissedFilter.Checked == true)   // if already was filtered 
                {
                    filteredByType = false;
                    menuItemMissedFilter.Checked = false;   // uncheck v mark
                    if (gridView == true)
                        dataGridCalls.DataSource = dtHistoriaCalls; // display all data in the datagrid
                    else
                        reloadCalls(dtHistoriaCalls);
                    dtTypedFiltered.Clear();
                    this.Text = "All calls";
                }
                else    //filter by missed
                {
                    menuItemMissedFilter.Checked = true;
                    menuItemOutgoingFilter.Checked = false;
                    menuItemIncomingFilter.Checked = false;
                    typeFilter("Missed");   // filter by missed
                }
            }
            catch
            {
                MessageBox.Show("Error in filtering by type.");
                MessageBox.Show("Application will now exit.");
                Application.Exit();
            }


        }

        private void menuItemIncomingFilter_Click(object sender, EventArgs e)   // filter by incoming
        {
            try
            {
                if (menuItemIncomingFilter.Checked == true) // if already filtered by incoming
                {
                    filteredByType = false;
                    menuItemIncomingFilter.Checked = false;
                    if (gridView == true)
                        dataGridCalls.DataSource = dtHistoriaCalls; // display all data in the datagrid
                    else
                    {
                        reloadCalls(dtHistoriaCalls);
                    }
                    dtTypedFiltered.Clear();
                    this.Text = "All calls";
                }
                else    // filter by incoming
                {
                    menuItemIncomingFilter.Checked = true;
                    menuItemOutgoingFilter.Checked = false;
                    menuItemMissedFilter.Checked = false;
                    typeFilter("Incoming"); // filter by incoming
                }
            }
            catch
            {
                MessageBox.Show("Error in filtering by type.");
                MessageBox.Show("Application will now exit.");
                Application.Exit();
            }

        }
        private void reloadCalls(DataTable table)
        {
            Cursor.Current = Cursors.WaitCursor;
            callsListBox.Items.Clear();
            foreach (DataRow dr in table.Rows)
                callsListBox.Items.Add(dr["S/N"]);
            callsListBox.refreshScroll();
            callsListBox.Refresh();
            Cursor.Current = Cursors.Default;
        }
        private void menuItemOutgoingFilter_Click(object sender, EventArgs e)   // filter by outgoing clicked
        {
            try
            {
                if (menuItemOutgoingFilter.Checked == true) // if already filtered by outgoing
                {
                    filteredByType = false;
                    menuItemOutgoingFilter.Checked = false;
                    if (gridView == true)
                        dataGridCalls.DataSource = dtHistoriaCalls; // display all data in the datagrid
                    else
                    {
                        reloadCalls(dtHistoriaCalls);
                    }
                    dtTypedFiltered.Clear();
                    this.Text = "All calls";
                }
                else    // filter by outgoing
                {
                    menuItemOutgoingFilter.Checked = true;
                    menuItemMissedFilter.Checked = false;
                    menuItemIncomingFilter.Checked = false;

                    typeFilter("Outgoing"); // filter by outgoing
                }
            }
            catch
            {
                MessageBox.Show("Error in filtering by type.");
                MessageBox.Show("Application will now exit.");
                Application.Exit();
            }
        }

        private void menuItemSMS_Click(object sender, EventArgs e)  // sending sms
        {
            try
            {
                SmsMessage sms = new SmsMessage(dtHistoriaCalls.Rows.Find(lastSelectedSN)["Number"].ToString(), ""); // create sms message with clicked number
                MessagingApplication.DisplayComposeForm(sms);    // show sms message in outlook
            }
            catch
            {
                MessageBox.Show("Error sending sms.");
            }
        }
        private void typeFilter(string stringType)  // filter by type
        {
            Cursor.Current = Cursors.WaitCursor;
            this.Text = stringType + " calls";
            if (gridView == true)
                dataGridCalls.DataSource = null;
            dtTypedFiltered.Clear();    // clear filtered table
            dtTypedFiltered.Columns["S/N"].DataType = typeof(long);    // set type of column to datetime
            DataRow[] rows = dtHistoriaCalls.Select("Type = '" + stringType + "'"); // create array for filtered data

            foreach (DataRow dr in rows)    // copy rows from array to table
                dtTypedFiltered.ImportRow(dr);


            DataView dv = new DataView(dtTypedFiltered);    // sort descneding
            dv.Sort = "S/N DESC";
            dtTypedFiltered = dv.ToTable();

            keysTyped[0] = dtTypedFiltered.Columns["S/N"];
            dtTypedFiltered.PrimaryKey = keysTyped;
            if (gridView == true)
                this.dataGridCalls.DataSource = dtTypedFiltered;
            else
            {
                reloadCalls(dtTypedFiltered);
            }
            filteredByType = true;
            Cursor.Current = Cursors.Default;
        }
        private DataTable billFilter() // outgoing filter for bills
        {
            try
            {
                DataTable dtBills = dtHistoriaCalls.Clone();
                dtBills.Columns["S/N"].DataType = typeof(long);    // set type of column to datetime
                DataRow[] rows = dtHistoriaCalls.Select("Type = 'Outgoing'"); // create array for filtered data

                foreach (DataRow dr in rows)    // copy rows from array to table
                    dtBills.ImportRow(dr);


                DataView dv = new DataView(dtBills);    // sort descneding
                dv.Sort = "S/N DESC";
                dtBills = dv.ToTable();
                return dtBills;
            }
            catch
            {
                MessageBox.Show("Error in filtering for bills.");
                return null;
            }
        }
        private void numberFilter()  // filter by number
        {
            this.Text = filteredNumber;
            filteredByNumber = true;
            dtNumberFiltered.Clear();   // clear filtered table
            dtNumberFiltered.Columns["S/N"].DataType = typeof(long);    // set type of column to datetime
            DataRow[] rows = dtHistoriaCalls.Select("Number Like '%" + filteredNumber + "%'"); //create array for filtered data

            foreach (DataRow dr in rows)    // copy rows from array to table
                dtNumberFiltered.ImportRow(dr);



            DataView dv = new DataView(dtNumberFiltered);   // sort descending
            dv.Sort = "S/N DESC";
            dtNumberFiltered = dv.ToTable();
            if (gridView == true)
                this.dataGridCalls.DataSource = dtNumberFiltered;
            else
            {
                reloadCalls(dtNumberFiltered);
            }
        }

        private void menuItemDeleteAll_Click(object sender, EventArgs e)    // delete all shown calls
        {
            try
            {
                if (MessageBox.Show("All calls shown in current display will be deleted, are you sure?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)  // warn the user
                {
                    // if user agreed to delete calls
                    Cursor.Current = Cursors.WaitCursor;
                    if (filteredByNumber == false && filteredByType == false)    // if table is not filtered
                    {
                        dtHistoriaCalls.Clear(); // delete all rows
                    }
                    else if (filteredByNumber == true)  // if table is filtered by number
                    {
                        foreach (DataRow dr in dtNumberFiltered.Rows)
                        {
                            DataRow tempRow = dtHistoriaCalls.Rows.Find(dr["S/N"]);
                            if (tempRow == null)    // if row wasn't found
                            {
                                MessageBox.Show("Some calls cannot be deleted from file because of a mismatch, please delete those calls from the non filtered display.");
                                continue;
                            }
                            dtHistoriaCalls.Rows.Remove(tempRow);   // find location of row in original table and delete it

                            if (menuItemMissedFilter.Checked == true || menuItemOutgoingFilter.Checked == true || menuItemIncomingFilter.Checked == true) // if was filtered by type
                            {
                                tempRow = dtTypedFiltered.Rows.Find(dr["S/N"]);
                                if (tempRow == null)    // if row wasn't found
                                    continue;

                                dtTypedFiltered.Rows.Remove(tempRow);
                            }
                        }
                        dtNumberFiltered.Clear();
                    }
                    else // if filtered by type
                    {
                        foreach (DataRow dr in dtTypedFiltered.Rows)
                        {
                            DataRow tempRow = dtHistoriaCalls.Rows.Find(dr["Time"]);
                            if (tempRow == null)    // if row wasn't found
                            {
                                MessageBox.Show("Some calls cannot be deleted from file because of a mismatch, please delete those calls from the non filtered display.");
                                continue;
                            }
                            dtHistoriaCalls.Rows.Remove(tempRow);   // find location of row in original table and delete it

                        }
                        dtTypedFiltered.Clear();
                    }
                    lastSelectedRow = 0;    // unselect row

                    daHistoriaCalls.Update(dsCalls, "Historia Calls");  // update the csv file
                    if (gridView == false)
                    if (gridView == false)
                    {
                        if (filteredByNumber == true && filteredByType == false)
                            reloadCalls(dtNumberFiltered);
                        else if (filteredByNumber == false && filteredByType == true)
                            reloadCalls(dtTypedFiltered);
                        else
                            reloadCalls(dtHistoriaCalls);
                    }

                    Cursor.Current = Cursors.Default; // show default cursor

                }
            }
            catch
            {
                MessageBox.Show("Error deleting all calls.");
            }
        }

        private void menuItemContacts_Click(object sender, EventArgs e) // save number to contacts
        {
            try
            {
                Contact contact = new Contact();
                contact.MobileTelephoneNumber = dtHistoriaCalls.Rows.Find(lastSelectedSN)[1].ToString();
                contact.ShowDialog();
            }
            catch
            {
                MessageBox.Show("Error adding to contacts.");
            }
        }
        private void rpn()
        {
            string owner;
            try
            {
                if (System.Environment.OSVersion.Version.Major < 5) // if ppc 03
                {
                    Byte[] ownerBytes = new Byte[100];   // get owner name
                    ownerBytes = (Byte[])Registry.CurrentUser.OpenSubKey("ControlPanel\\Owner").GetValue("Owner");
                    owner = System.Text.Encoding.Unicode.GetString(ownerBytes, 0, 72).TrimEnd('\0');
                }
                else
                    owner = Microsoft.WindowsMobile.Status.SystemState.OwnerName.ToString();
                if (owner.Length > 10)
                {
                    owner = owner.Substring(0, 5) + owner.Substring(owner.Length - 5, 5);
                }
            }
            catch
            {
                owner = "";
            }

            for (UInt16 i = 0; i < owner.Length; i++)  // run on each char of text
            {
                key += 6791;
                key *= (ushort)(owner[i] * 13);
                key += (ushort)(6829 + i);
            }
        }

        private void menuItemBills_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;    // display wait cursor
            DataTable dtBills;
            dtBills = billFilter(); // filter by outgoing

            FormBill formBill = new FormBill(dtBills);   // create new setting form

            formBill.Owner = this;  // declare the owner of the bill form
            formBill.Show();    // show settings form
            this.Enabled = false;   // disable main form

        }
        private void menuItemSummary_Click(object sender, EventArgs e)
        {
            try
            {
                TimeSpan duration = new TimeSpan(0);
                DataTable tempDataTable;
                string durationString;

                if (filteredByNumber == true && filteredByType == false)
                    tempDataTable = dtNumberFiltered;
                else if (filteredByNumber == false && filteredByType == true)
                    tempDataTable = dtTypedFiltered;
                else
                    tempDataTable = dtHistoriaCalls;

                foreach (DataRow dr in tempDataTable.Rows)
                {
                    durationString = (string)dr["Duration"];
                    if (durationString.Length == 5) // add hours to string
                        durationString = "00:" + durationString;
                    duration += TimeSpan.Parse(durationString);
                }
                MessageBox.Show("Number of Calls: " + tempDataTable.Rows.Count + "\r\n" + "Total Duration: " + Math.Floor(duration.TotalHours).ToString() + ":" + duration.Minutes.ToString().PadLeft(2, '0') + ":" + duration.Seconds.ToString().PadLeft(2, '0'));
            }
            catch
            {
                MessageBox.Show("Error in summarizing.");
            }
        }


        private void formSearch_Closed(object sender, EventArgs e) // if search form is closed
        {

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    if (formSearch.stringSearch != "")
                    {
                        filteredNumber = formSearch.stringSearch; // get number to be filtered
                        numberFilter();   // filter by number
                        this.mainMenu1.MenuItems.Remove(menuItemTypeFilter);    // remove filter menu
                        this.mainMenu1.MenuItems.Add(this.menuItemBack);    // add back menu
                    }
                }
                catch
                {
                    MessageBox.Show("Error filtering a number.");
                }
                Cursor.Current = Cursors.Default;
            }
            catch
            {
                MessageBox.Show("Error in searching");
            }
        }

        private void menuItemSearch_Click(object sender, EventArgs e)
        {
            try
            {
                formSearch = new FormSearch();
                formSearch.Closed += new EventHandler(formSearch_Closed);    // add a closed event for search form
                formSearch.Owner = this;  // declare the owner of the search form  
                formSearch.Show();    // show settings form
                this.Enabled = false;   // disable main form 
            }
            catch
            {
                MessageBox.Show("Error in search form.");
            }
        }

        private void addCustomListBox()
        {
            // Create a new instance of CallsListBox.
            callsListBox = new CallsListBox(this);
            callsListBox.Parent = this;

            // Draw the bounds of the callsListBox.
//            callsListBox.Bounds = new Rectangle(5, 5, 150, 100);
            callsListBox.Dock = DockStyle.Fill;
            reloadCalls(dtHistoriaCalls);
            callsListBox.ContextMenu = contextMenu1;
        }
        private void addDataGrid()
        {
            this.dataGridCalls.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dataGridCalls.ContextMenu = this.contextMenu1;
            this.dataGridCalls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridCalls.Location = new System.Drawing.Point(0, 0);
            this.dataGridCalls.Name = "dataGridCalls";
            this.dataGridCalls.RowHeadersVisible = false;
            this.dataGridCalls.Size = new System.Drawing.Size(240, 268);
            this.dataGridCalls.TabIndex = 0;
            this.Controls.Add(this.dataGridCalls);
        }
        private void initializeList()
        {
            getCsvData();   // get the data from the csv
            updateCsv();     // update csv from call log
            if (gridView == true)
            {
                setStyle();     // set styles
                addDataGrid();  // add datagrid
                dataGridCalls.GotFocus += new System.EventHandler(this.calls_GotFocus);     // set get focus event for dealing with changes in the list when the program is active
            }
            else
            {
                addCustomListBox(); // add custom listbox
                callsListBox.GotFocus += new System.EventHandler(this.calls_GotFocus);  // set get focus event for dealing with changes in the list when the program is active
            }
        }
        private void menuItemDetails_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow row = dtHistoriaCalls.Rows.Find(lastSelectedSN);
                this.Text = row["Number"].ToString();
                FormDetails formDetails = new FormDetails(row["Name"].ToString(),
                    row["Time"].ToString(),
                    row["Duration"].ToString(),
                    row["Type"].ToString() + " Call");
                formDetails.Closed += new EventHandler(detailsForm_Closed);    // add a closed event for details form
                formDetails.Owner = this;  // declare the owner of the search form  
                formDetails.Show();    // show settings form
                this.Enabled = false;   // disable main form 
            }
            catch
            {
                MessageBox.Show("Error in Details form.");
            }
        }
    }
}
