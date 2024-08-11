using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using OpenNETCF.Data.Text;  //csv to datatable
using FreshLogicStudios.Atlas.Mobile.PocketPC;  // configuration manager  
using System.Globalization; // for localization
using System.Text.RegularExpressions;   // for regex
using MakeLogic.MG; // for graphs

namespace Historia
{
    public partial class FormBill : Form
    {
        private DataSet dsBills;    // stores the dataseti
        private TextDataAdapter daBills;   // gets the data from the csv
        private DataTable dtBills;      // stores the data table
        private DataTable dtBillsOutgoing;  // store the outgoing datatable
        private DataGridTableStyle dtStyle = new DataGridTableStyle();  //  new datagrid style

        private BarGraph barGraph;  // for creating a graph

        double[] xValues;   // x values for the graph
        double[] yValues;   // y values for the graph

        private string systemCulture = CultureInfo.CurrentCulture.Name; // get system culture

        public FormBill(DataTable dtBillsOutgoingPassed)
        {
            InitializeComponent();
            getSettingsFromFile();  //get settings from file
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Historia\Bills.txt") && textBoxDiscountPrice.Text == "0" && textBoxNormalPrice.Text == "0" && textBoxFee.Text == "0" && textBoxEveningPrice.Text == "0" && textBoxWeekendPrice.Text == "0" )
            {   // if this is the first time you run historia and bills.txt exists, delete it.
                string targetFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Historia"; //where to save and get the csv file (my documents\historia)
                string billsFileName = Path.Combine(targetFolder, "Bills.txt"); // file name of bills
                string billsFileNameBackup = Path.Combine(targetFolder, "BillsBackup.txt"); // file name of bills
                MessageBox.Show("This is the first time you run this version, bills.txt has to be cleared. It is saved as billsBackup.txt for backup. Change some of the bill settings to prevent this message.");
                File.Copy(billsFileName, billsFileNameBackup,true);
                File.Delete(billsFileName);
            }

            try
            {
                dtBillsOutgoing = dtBillsOutgoingPassed;    // get the outgoing calls table
                createBill();   // get the data from the csv
                if (dtBillsOutgoing.Rows.Count != 0)    // if calls exist
                {
                    updateBill();     // make the grid  
                }

      
                if (dtBills.Rows.Count != 0)  // if list of bills is not empty
                {
                    SetDefaultLocale(new CultureInfo("en-US"));
                    int size = dtBills.Rows.Count <= 12 ? dtBills.Rows.Count : 12;  // number of bars will be 12 max
                    xValues = new double[size]; // create array for the x values
                    yValues = new double[size]; // create array for the y values
                    try
                    {
                        for (int i = 0; i < size; i++) // set bars values
                        {
                            xValues[i] = int.Parse(dtBills.Rows[i]["S/N"].ToString(), new CultureInfo("en-US")); // x value will be a month
                            yValues[i] = double.Parse(dtBills.Rows[i]["Total Price"].ToString());   // y value will be the price
                        }
                    }
                    catch
                    {

                        if (MessageBox.Show("There was an error related to Bills.txt. you can try to launch the application again. if it doesn't help, you can clear the file by tapping Yes. The old file will be saved as BillsBackup.txt for backup purpuses. If error persists please contact support or reinstall the software. Do you want to clear the file?", "confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            string targetFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Historia"; //where to save and get the csv file (my documents\historia)
                            string billsFileName = Path.Combine(targetFolder, "Bills.txt"); // file name of bills
                            string billsFileNameBackup = Path.Combine(targetFolder, "BillsBackup.txt"); // file name of bills

                            File.Copy(billsFileName, billsFileNameBackup, true);
                            dtBills.Clear();    // clear the bills
                            daBills.Update(dsBills, "Historia Bills");
                        }
                        MessageBox.Show("Application will now exit, try to start it again.");
                        this.Close();
                     }
                    try
                    {
                        barGraph = new BarGraph(this.Width, this.Height - 20);  // create new graph

                        barGraph.XValues = xValues; // set graph x values
                        barGraph.YValues = yValues; // set graph y values
                        barGraph.TitleFontSize = 8;
                        barGraph.XLabelOn = false;
                        barGraph.YLabelOn = false;
                        barGraph.Title = "Price Per Month";
                    }
                    catch (IncompatibleDataException e)
                    {
                        MessageBox.Show(e.Message, "MicroGraphs");
                        Application.Exit();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "MicroGraphs");
                        Application.Exit();
                    }
                    this.tabPageGraph.Controls.Add(barGraph);   //add graph
                }

                foreach (DataRow dr in dtBills.Rows)    // set price as currency
                {
                    TimeSpan duration = TimeSpan.Parse((string)dr["Duration"]);
                    dr["Total Price"] = decimal.Parse(dr["Total Price"].ToString()).ToString("C2", new CultureInfo(systemCulture));
                    dr["Duration"] = Math.Floor(duration.TotalHours).ToString() + ":" + duration.Minutes.ToString().PadLeft(2, '0') + ":" + duration.Seconds.ToString().PadLeft(2, '0');
                }
            }
            catch
            {

                if (MessageBox.Show("There was an error related to Bills.txt. you can try to launch the application again. if it doesn't help, you can clear the file by tapping Yes. The old file will be saved as BillsBackup.txt for backup purpuses. If error persists please contact support or reinstall the software. Do you want to clear the file?", "confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    string targetFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Historia"; //where to save and get the csv file (my documents\historia)
                    string billsFileName = Path.Combine(targetFolder, "Bills.txt"); // file name of bills
                    string billsFileNameBackup = Path.Combine(targetFolder, "BillsBackup.txt"); // file name of bills
  
                    File.Copy(billsFileName, billsFileNameBackup, true);
                    dtBills.Clear();    // clear the bills
                    daBills.Update(dsBills, "Historia Bills");
                }
                MessageBox.Show("Application will now exit, try to start it again.");
                this.Close();
            } 
            Cursor.Current = Cursors.Default;    // display wait cursor
      }      

        private void menuItemBack_Click(object sender, EventArgs e) // back is tapped
        {
            ConfigurationManager.AppSettings["BillsColWidth0"] = dtStyle.GridColumnStyles[0].Width.ToString();
            ConfigurationManager.AppSettings["BillsColWidth1"] = dtStyle.GridColumnStyles[1].Width.ToString();
            ConfigurationManager.AppSettings["BillsColWidth2"] = dtStyle.GridColumnStyles[2].Width.ToString();
            ConfigurationManager.Save();
            SetDefaultLocale(new CultureInfo(systemCulture));
            this.Owner.Enabled = true;
            this.Owner.Show();
            this.Close();
        }
        private void createBill()   // get data from csv file
        {
            Cursor.Current = Cursors.WaitCursor;    // display wait cursor
            string targetFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\Historia"; //where to save and get the csv file (my documents\historia)
            string billsFileName = Path.Combine(targetFolder, "Bills.txt"); // file name of bills

            if (!File.Exists(billsFileName)) // if csv file does not exist , create it
            {
                using (Stream myStream = File.Open(billsFileName, FileMode.Append, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(myStream);

                    sw.WriteLine("Month" + ',' + "Duration" + ',' + "Total Price" + ',' + "S/N"); // header line
                    sw.Flush();
                }
            }
            dsBills = new DataSet("Bills"); // new dataset
            daBills = new TextDataAdapter(billsFileName, true, ','); // get csv data
            daBills.ForceWrite = true;  // this will make the csv file get updated when rows are deleted from datatable
            daBills.Fill(dsBills, "Historia Bills");    // store it in the dataset
            dtBills = dsBills.Tables["Historia Bills"]; // assign the datatable to the data retrived from the csv
            setStyle();
            dataGridBills.DataSource = dtBills; // display the data in the data grid
  
            Cursor.Current = Cursors.Default;   // display default cursor
        }

        private void updateBill()    // update file and table
        {

            Cursor.Current = Cursors.WaitCursor;    // display wait cursor

            TimeSpan totalDuration = new TimeSpan(0);    // duration of calls in month
            TimeSpan duration = new TimeSpan(0);        // duration of current call
            int discountMinutes = int.Parse(textBoxDiscountMinutes.Text);   // number of discounted minutes
            int billDay = int.Parse(numericUpDownBillDay.Value.ToString()); //bill day chosen by user
            decimal totalPrice= 0;    // total monthly bill
            decimal discountPrice = decimal.Parse(textBoxDiscountPrice.Text,  new CultureInfo("en-US")) / 60; //price of discounted minute
            decimal normalPrice = decimal.Parse(textBoxNormalPrice.Text,  new CultureInfo("en-US")) / 60;   // normal price
            decimal fee = decimal.Parse(textBoxFee.Text, new CultureInfo("en-US"));   // constant monthly fee 
            bool specialFlag;   // raise when special number is made
            string durationString;  // display datetime duration as string 
            string specialNumbersPrice = Regex.Replace(textBoxSpecial.Text, "\r\n" , ",");  // get special numbers string
            specialNumbersPrice = Regex.Replace(textBoxSpecial.Text, "\r", ",");  // get special numbers string
            specialNumbersPrice = Regex.Replace(specialNumbersPrice, " ", "");  // trim spaces
            string specialNumbersPriceNotIncluded = Regex.Replace(textBoxSpecialNotIncluded.Text, "\r\n", ",");  // get special numbers string
            specialNumbersPriceNotIncluded = Regex.Replace(textBoxSpecialNotIncluded.Text, "\r", ",");
            specialNumbersPriceNotIncluded = Regex.Replace(specialNumbersPriceNotIncluded, " ", "");  // trim spaces
            string[] splitted = Regex.Split(specialNumbersPrice, ",");  // split duration string to array
            string[] splittedNotIncluded = Regex.Split(specialNumbersPriceNotIncluded, ",");  // split duration string to array
            string freeNumbers = Regex.Replace(textBoxFree.Text, "\r\n", ",");  // get special numbers string
            freeNumbers = Regex.Replace(textBoxFree.Text, "\r", ",");  // get special numbers string
            freeNumbers = Regex.Replace(freeNumbers, " ", "");  // trim spaces
            string[] splittedFreeNumbers = Regex.Split(freeNumbers, ",");  // get free numbers string
            decimal freeSeconds = decimal.Parse(textBoxNumberOfFree.Text, new CultureInfo("en-US")) * 60;   // number of free minutes
            decimal roundUp = decimal.Parse(textBoxRoundUp.Text, new CultureInfo("en-US"));
            for (int i = 0, j = 0; i < dtBillsOutgoing.Rows.Count ; i++)   // run on the outgoing table
            {
                specialFlag = false;    // lower special number flag

                DateTime rowDate = new DateTime(long.Parse(dtBillsOutgoing.Rows[i]["S/N"] + "0000000"));    // get current date of row
                DateTime nextRowDate = new DateTime(0); // get next row date
                DateTime billDate = new DateTime(rowDate.Year, rowDate.Month, billDay > DateTime.DaysInMonth(rowDate.Year, rowDate.Month) ? DateTime.DaysInMonth(rowDate.Year, rowDate.Month) : billDay, 0, 0, 0);   // set bill date
                DateTime nextBillDate = new DateTime(0);    // next row bill date 
                if (billDate.Day <= rowDate.Day) // if bill date is in the next month
                    billDate = new DateTime(rowDate.AddMonths(1).Year, rowDate.AddMonths(1).Month, billDay > DateTime.DaysInMonth(rowDate.AddMonths(1).Year, rowDate.AddMonths(1).Month) ? DateTime.DaysInMonth(rowDate.AddMonths(1).Year, rowDate.AddMonths(1).Month) : billDay, 0, 0, 0);   // set bill date
                DateTime fromBillDate = new DateTime(billDate.AddMonths(-1).Year, billDate.AddMonths(-1).Month, billDay > DateTime.DaysInMonth(billDate.AddMonths(-1).Year, billDate.AddMonths(-1).Month) ? DateTime.DaysInMonth(billDate.AddMonths(-1).Year, billDate.AddMonths(-1).Month) : billDay, 0, 0, 0);   // set bill date

                if (i != dtBillsOutgoing.Rows.Count - 1)  // if row is not last
                {
                    nextRowDate = new DateTime(long.Parse(dtBillsOutgoing.Rows[i + 1]["S/N"] + "0000000"));   // get next row date
                    nextBillDate = new DateTime(nextRowDate.Year, nextRowDate.Month, billDay > DateTime.DaysInMonth(nextRowDate.Year, nextRowDate.Month) ? DateTime.DaysInMonth(nextRowDate.Year, nextRowDate.Month) : billDay, 0, 0, 0);   // set next row bill date
                    if (nextBillDate.Day <= nextRowDate.Day) // if bill date is in the next month
                        nextBillDate = new DateTime(nextRowDate.AddMonths(1).Year, nextRowDate.AddMonths(1).Month, billDay > DateTime.DaysInMonth(nextRowDate.AddMonths(1).Year, nextRowDate.AddMonths(1).Month) ? DateTime.DaysInMonth(nextRowDate.AddMonths(1).Year, nextRowDate.AddMonths(1).Month) : billDay, 0, 0, 0);   // set bill date
                }
                durationString = dtBillsOutgoing.Rows[i]["duration"].ToString();   // get duration of call
                if (durationString.Length == 5) // add hours to string
                    durationString = "00:" + durationString;
                duration = TimeSpan.Parse(durationString);  // duration calculation
                decimal billedSeconds = numericUpDownTimeUnit.Value * (decimal)Math.Ceiling(duration.TotalSeconds / (double)numericUpDownTimeUnit.Value);   // calculate billed seconds
                if (0 < billedSeconds && billedSeconds < roundUp)
                    billedSeconds = roundUp;
                if (splittedFreeNumbers.Length > 0 && freeSeconds > 0)    // if free numbers exist
                {
                    for (int k = 0; k < splittedFreeNumbers.Length; k++)   // go over the free numbers list
                    {
                        if (splittedFreeNumbers[k] == "") continue;    // if string is empty continue

                        if (dtBillsOutgoing.Rows[i]["Number"].ToString().Length >= splittedFreeNumbers[k].Length && dtBillsOutgoing.Rows[i]["Number"].ToString().Substring(0, splittedFreeNumbers[k].Length) == splittedFreeNumbers[k])  // if row is a special number
                        {
                            if (freeSeconds >= billedSeconds)
                            {
                                specialFlag = true; // raise special flag
                                freeSeconds -= billedSeconds;
                            }
                            else
                            {
                                billedSeconds -= freeSeconds;
                                freeSeconds = 0;
                            }
                            break;
                        }
                    }
                }
                if (splitted.Length > 0 && specialFlag == false)    // if special prices exist
                {
                    for (int k = 0; k < splitted.Length; k += 2)   // go over the special rates list
                    {
                        if (splitted[k] == "") continue;    // if string is empty continue

                        if (dtBillsOutgoing.Rows[i]["Number"].ToString().Length >= splitted[k].Length && dtBillsOutgoing.Rows[i]["Number"].ToString().Substring(0, splitted[k].Length) == splitted[k])  // if row is a special number
                        {
                            totalPrice += billedSeconds * decimal.Parse(splitted[k + 1],  new CultureInfo("en-US")) / 60;   // add to total price
                            totalDuration += duration;  // add to total duration
                            specialFlag = true; // raise special flag
                            break;
                        }
                    }
                }
                if (splittedNotIncluded.Length > 0 && specialFlag == false)   // if  special number which is not included in special duration
                {
                    for (int k = 0; k < splittedNotIncluded.Length; k += 2)   // go over the special rates list
                    {
                        if (splittedNotIncluded[k] == "") continue;    // if string is empty continue

                        if (dtBillsOutgoing.Rows[i]["Number"].ToString().Length >= splittedNotIncluded[k].Length && dtBillsOutgoing.Rows[i]["Number"].ToString().Substring(0, splittedNotIncluded[k].Length) == splittedNotIncluded[k])  // if row is a special number
                        {
                            totalPrice += billedSeconds * decimal.Parse(splittedNotIncluded[k + 1],  new CultureInfo("en-US")) / 60;   // add to total price
                            specialFlag = true; // raise special flag
                            break;
                        }
                    }
                }
                if (checkBoxWeekend.Checked == true && specialFlag == false)   // if weekend checkbox is checked
                {
                    TimeSpan weekendStartTime = TimeSpan.Parse(ConfigurationManager.AppSettings["Weekend Start"].ToString());   // start hour of weekend
                    TimeSpan weekendEndTime = TimeSpan.Parse(ConfigurationManager.AppSettings["Weekend End"].ToString());   // end hour of weekend

                    if (((int)rowDate.DayOfWeek > listBoxWeekendFromDay.SelectedIndex || ((int)rowDate.DayOfWeek == listBoxWeekendFromDay.SelectedIndex) && rowDate.TimeOfDay > weekendStartTime  ) || ((int)rowDate.DayOfWeek < listBoxWeekendUntilDay.SelectedIndex || (int)rowDate.DayOfWeek == listBoxWeekendUntilDay.SelectedIndex && weekendEndTime <= rowDate.TimeOfDay))    // if call is in weekend
                    {
                        totalPrice += billedSeconds * decimal.Parse(textBoxWeekendPrice.Text,  new CultureInfo("en-US")) / 60;
                        specialFlag = true; // raise special flag
                    }

                }
                if (checkBoxEvening.Checked == true && specialFlag == false)   // if call is in evening
                {
                    TimeSpan eveningTime = TimeSpan.Parse(ConfigurationManager.AppSettings["Evening Start"].ToString());    // start hour of evening
                    DateTime eveningStart = rowDate.Date.AddHours(eveningTime.Hours).AddMinutes(eveningTime.Minutes);      // start time of evening
                    eveningTime = TimeSpan.Parse(ConfigurationManager.AppSettings["Evening End"].ToString());   // end hour of evening
                    DateTime eveningEnd = rowDate.Date.AddDays(1).AddHours(eveningTime.Hours).AddMinutes(eveningTime.Minutes);  // end time of evening

                    if (rowDate.CompareTo(eveningStart) >= 0 && rowDate.CompareTo(eveningEnd) < 0)  // if call is in evening
                    {
                        totalPrice += billedSeconds * decimal.Parse(textBoxEveningPrice.Text,  new CultureInfo("en-US")) / 60;
                        specialFlag = true;
                    }

                }
                if (specialFlag == false)   // if not special number or evening or weekend
                {
                    if ((decimal)totalDuration.TotalSeconds + billedSeconds <= discountMinutes*60 + numericUpDownTimeUnit.Value)    // if duration is still in discount
                    {
                        totalPrice += billedSeconds * discountPrice;   // add to total price
                    }
                    else if ((totalDuration.TotalMinutes < discountMinutes) && (decimal)totalDuration.TotalSeconds + billedSeconds > discountMinutes*60 + numericUpDownTimeUnit.Value)    // if some of the duration is over the discounted duration and some is below
                    {
                        decimal tempTimeUnits = (decimal)Math.Ceiling((discountMinutes - (double)totalDuration.TotalMinutes) * 60 /(double) numericUpDownTimeUnit.Value) ;
                        totalPrice += tempTimeUnits * discountPrice * numericUpDownTimeUnit.Value + (decimal)(Math.Ceiling((duration.TotalSeconds - (double)tempTimeUnits*(double)numericUpDownTimeUnit.Value) / (double)numericUpDownTimeUnit.Value) * (double)numericUpDownTimeUnit.Value *(double) normalPrice);
                    }
                    else // if duration is over the disounted duration
                    {
                        totalPrice += billedSeconds * normalPrice;
                    }
                    if (duration.Ticks > 0)
                        totalPrice += decimal.Parse(textBoxCallFee.Text,  new CultureInfo("en-US"));
                    totalDuration += duration;  // add to total duration
                }


                if (i == dtBillsOutgoing.Rows.Count - 1 || nextBillDate.CompareTo(billDate) != 0) // when month is over add a line
                {     
                    //^^ if  month already exists and list is not empty
                    if (dtBills.Rows.Count -1 >= j && int.Parse(dtBills.Rows[j]["S/N"].ToString()) == fromBillDate.Month)  // if month exists change it
                    {
                        dtBills.Rows.RemoveAt(j);   // remove row
                        DataRow row = dtBills.NewRow(); // create a new row
                        totalPrice += fee;
                        row["Month"] = fromBillDate.ToString("d", CultureInfo.CurrentCulture.DateTimeFormat) + " - " + billDate.AddDays(-1).ToString("d", CultureInfo.CurrentCulture.DateTimeFormat);
                        row["Total Price"] = totalPrice.ToString(new CultureInfo("en-US"));
                        row["Duration"] = totalDuration.ToString();
                        row["S/N"] = fromBillDate.Month;
                        dtBills.Rows.InsertAt(row, j);

                        break;
                    }
               
                    else // if month does not exist add new row
                    {
                        DataRow row = dtBills.NewRow(); // create a new row
                        totalPrice += fee;
                        row["Month"] = fromBillDate.ToString("d", CultureInfo.CurrentCulture.DateTimeFormat) + " - " + billDate.AddDays(-1).ToString("d", CultureInfo.CurrentCulture.DateTimeFormat);
                        row["Total Price"] = totalPrice.ToString(new CultureInfo("en-US"));
                        row["Duration"] = totalDuration.ToString();
                        row["S/N"] = fromBillDate.Month;
                        dtBills.Rows.InsertAt(row, j++);  // insert the row
                        totalPrice = 0;
                        totalDuration = new TimeSpan(0);
                        freeSeconds = decimal.Parse(textBoxNumberOfFree.Text, new CultureInfo("en-US")) * 60;   // restore number of free minutes
                    }
                    daBills.Update(dsBills, "Historia Bills");  // update the csv file 
                
                }
 
            }
            Cursor.Current = Cursors.Default;   // display default cursor
        }

        bool testFloat(string str)   // test if text is a float number
      {
          str = str.Trim();  // trim spaces from end and start of string
          int countPoints = 0;  // for counting number of points
          for (int i = 0; i < str.Length; i++ )
          {
             if (str[i] != '.') //if char is not a digit or point
             {
                if (char.IsNumber(str[i]) == false)
                  return false;
             }
             else // if char is a point
             {
                 if ( i == 0 || i == str.Length)    // point is in the edge of string
                     return false;
                 countPoints++;
             }
           }
           if ( countPoints > 1 )   // if more than 1 point
               return false;
          return true;
              
       }
        bool testNumber(string str)   // test if text is a number
        {
            str = str.Trim(); 
            for (int i = 0; i < str.Length; i++)
            {
               if (char.IsNumber(str[i]) == false)
                        return false;
            }
            return true;
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
        private void setStyle()     // set datagrid style
        {
            try
            {
                string[] headers = { "Month", "Duration", "Total Price", "S/N" };   //  for changing header styles
                dtStyle.MappingName = "Historia Bills";     // map style to the data table

                for (int i = 0; i < dtBills.Columns.Count; i++) // run on each column
                {
                    ColumnStyle myStyle = new ColumnStyle(i);   // create a new column style
                    myStyle.MappingName = dtBills.Columns[i].ColumnName;    // map the new style to the column
                    myStyle.HeaderText = headers[i];    // set header text
                    if (i == 0)
                        myStyle.Width = int.Parse(ConfigurationManager.AppSettings["BillsColWidth0"]);
                    else if (i == 1)
                        myStyle.Width = int.Parse(ConfigurationManager.AppSettings["BillsColWidth1"]);
                    else if (i == 2)
                    {
                        myStyle.Width = int.Parse(ConfigurationManager.AppSettings["BillsColWidth2"]); // column width will be taken from the app.config
                    }
                    else
                        myStyle.Width = 0;

                    dtStyle.GridColumnStyles.Add(myStyle);  // add column style to the new data table style
                }

                dataGridBills.TableStyles.Add(dtStyle); // add style to the datagrid
            }
            catch
            {
                MessageBox.Show("Error setting styles. Application will now exit, try to start it again.");
                this.Close();
            }
            
        }
        bool testCommasEnters(string str)   // test for comma separated strings
        {
            str = Regex.Replace(str, " ", "");  // trim spaces
            string[] test = Regex.Split(str, "\r\n");   // split string to array

            for (int i = 0; i < test.Length ; i++)  // go over array
            {
                if (test[i] == "") continue;    //if empty string continue
                string[] testElements = test[i].Split(','); //split line to 2 strings
                if (testElements.Length != 2)   // if more than 2 commas per line
                    return false;
                if (testFloat(testElements[1]) == false)    // if price is not a float.
                    return false;
            }

            return true;
        }
    
        private void saveBillSettings() // save settings
        {

            if (testFloat(textBoxDiscountPrice.Text) == true)
            {
                ConfigurationManager.AppSettings["Discount Price"] = textBoxDiscountPrice.Text.Trim();
            }
            if (testFloat(textBoxNormalPrice.Text) == true)
            {
                ConfigurationManager.AppSettings["Normal Price"] = textBoxNormalPrice.Text.Trim();
            }
            if (testNumber(textBoxDiscountMinutes.Text) == true)
            {
                ConfigurationManager.AppSettings["Discount Minutes"] = textBoxDiscountMinutes.Text.Trim();
            }
            if (testFloat(textBoxFee.Text) == true)
            {
                ConfigurationManager.AppSettings["Fee"] = textBoxFee.Text.Trim();
            }
            if (testFloat(textBoxCallFee.Text) == true)
            {
                ConfigurationManager.AppSettings["Call Fee"] = textBoxCallFee.Text.Trim();
            }
            if (testCommasEnters(textBoxSpecial.Text) == true)
            {
                ConfigurationManager.AppSettings["Special Numbers"] = textBoxSpecial.Text.Trim();
            }
            ConfigurationManager.AppSettings["Free Numbers"] = textBoxFree.Text.Trim();
            if (testNumber(textBoxDiscountMinutes.Text) == true)
            {
                ConfigurationManager.AppSettings["Number Of Free Minutes"] = textBoxNumberOfFree.Text.Trim();
            }
            if (testNumber(textBoxRoundUp.Text) == true)
            {
                ConfigurationManager.AppSettings["Round Up"] = textBoxRoundUp.Text.Trim();
            }
            ConfigurationManager.AppSettings["Evening CheckBox"] = checkBoxEvening.Checked.ToString();
            if (testFloat(textBoxEveningPrice.Text) == true)
            {
                ConfigurationManager.AppSettings["Evening Price"] = textBoxEveningPrice.Text.Trim();
            }
            ConfigurationManager.AppSettings["Evening Start"] = numericUpDownEveningFrom.Value.ToString().PadLeft(2, '0') + ":" + numericUpDownEveningFromMinutes.Value.ToString().PadLeft(2, '0');
            ConfigurationManager.AppSettings["Evening End"] = numericUpDownEveningUntil.Value.ToString().PadLeft(2, '0') + ":" + numericUpDownEveningUntilMinutes.Value.ToString().PadLeft(2, '0');

            if (testFloat(textBoxWeekendPrice.Text) == true)
            {
                ConfigurationManager.AppSettings["Weekend Price"] = textBoxWeekendPrice.Text.Trim();
            }
            if (listBoxWeekendFromDay.SelectedIndex > listBoxWeekendUntilDay.SelectedIndex)
            {
                ConfigurationManager.AppSettings["Weekend CheckBox"] = checkBoxWeekend.Checked.ToString();
                ConfigurationManager.AppSettings["Weekend Start"] = numericUpDownWeekendFromHour.Value.ToString().PadLeft(2, '0') + ":" + numericUpDownWeekendFromMinutes.Value.ToString().PadLeft(2, '0');
                ConfigurationManager.AppSettings["Weekend End"] = numericUpDownWeekendUntilHour.Value.ToString().PadLeft(2, '0') + ":" + numericUpDownWeekendUntilMinutes.Value.ToString().PadLeft(2, '0');
                ConfigurationManager.AppSettings["Weekend Start Day"] = listBoxWeekendFromDay.SelectedIndex.ToString();
                ConfigurationManager.AppSettings["Weekend End Day"] = listBoxWeekendUntilDay.SelectedIndex.ToString();
            }
            ConfigurationManager.AppSettings["Time Unit"] = numericUpDownTimeUnit.Value.ToString();

            if (testCommasEnters(textBoxSpecialNotIncluded.Text) == true)
            {
                ConfigurationManager.AppSettings["Special Numbers Not Included"] = textBoxSpecialNotIncluded.Text.Trim();
            }
            ConfigurationManager.AppSettings["BillsColWidth0"] = dtStyle.GridColumnStyles[0].Width.ToString();
            ConfigurationManager.AppSettings["BillsColWidth1"] = dtStyle.GridColumnStyles[1].Width.ToString();
            ConfigurationManager.AppSettings["BillsColWidth2"] = dtStyle.GridColumnStyles[2].Width.ToString();
            ConfigurationManager.Save();
        }
        private void getSettingsFromFile()
        {
            try
            {
                // get settings from file ------------------------------------------------------------
                textBoxDiscountMinutes.Text = ConfigurationManager.AppSettings["Discount Minutes"];
                textBoxDiscountPrice.Text = ConfigurationManager.AppSettings["Discount Price"];
                textBoxFee.Text = ConfigurationManager.AppSettings["Fee"];
                textBoxNormalPrice.Text = ConfigurationManager.AppSettings["Normal Price"];
                textBoxSpecial.Text = ConfigurationManager.AppSettings["Special Numbers"];
                textBoxSpecialNotIncluded.Text = ConfigurationManager.AppSettings["Special Numbers Not Included"];
                textBoxFree.Text = ConfigurationManager.AppSettings["Free Numbers"];
                textBoxNumberOfFree.Text = ConfigurationManager.AppSettings["Number Of Free Minutes"];
                textBoxRoundUp.Text = ConfigurationManager.AppSettings["Round Up"];
                numericUpDownBillDay.Value = decimal.Parse(ConfigurationManager.AppSettings["Bill Day"],  new CultureInfo("en-US"));
                checkBoxEvening.Checked = bool.Parse(ConfigurationManager.AppSettings["Evening CheckBox"]);
                textBoxEveningPrice.Text = ConfigurationManager.AppSettings["Evening Price"];
                numericUpDownEveningFrom.Value = decimal.Parse(TimeSpan.Parse(ConfigurationManager.AppSettings["Evening Start"]).Hours.ToString(),  new CultureInfo("en-US"));
                numericUpDownEveningFromMinutes.Value = decimal.Parse(TimeSpan.Parse(ConfigurationManager.AppSettings["Evening Start"]).Minutes.ToString(),  new CultureInfo("en-US"));
                numericUpDownEveningUntil.Value = decimal.Parse(TimeSpan.Parse(ConfigurationManager.AppSettings["Evening End"]).Hours.ToString());
                numericUpDownEveningUntilMinutes.Value = decimal.Parse(TimeSpan.Parse(ConfigurationManager.AppSettings["Evening End"]).Minutes.ToString(),  new CultureInfo("en-US"));
                checkBoxWeekend.Checked = bool.Parse(ConfigurationManager.AppSettings["Weekend CheckBox"]);
                textBoxWeekendPrice.Text = ConfigurationManager.AppSettings["Weekend Price"];
                listBoxWeekendFromDay.SelectedIndex = int.Parse(ConfigurationManager.AppSettings["Weekend Start Day"]);
                numericUpDownWeekendFromHour.Value = decimal.Parse(TimeSpan.Parse(ConfigurationManager.AppSettings["Weekend Start"]).Hours.ToString(),  new CultureInfo("en-US"));
                numericUpDownWeekendFromMinutes.Value = decimal.Parse(TimeSpan.Parse(ConfigurationManager.AppSettings["Weekend Start"]).Minutes.ToString(),  new CultureInfo("en-US"));
                listBoxWeekendUntilDay.SelectedIndex = int.Parse(ConfigurationManager.AppSettings["Weekend End Day"]);
                numericUpDownWeekendUntilHour.Value = decimal.Parse(TimeSpan.Parse(ConfigurationManager.AppSettings["Weekend End"]).Hours.ToString(),  new CultureInfo("en-US"));
                numericUpDownWeekendUntilMinutes.Value = decimal.Parse(TimeSpan.Parse(ConfigurationManager.AppSettings["Weekend End"]).Minutes.ToString(),  new CultureInfo("en-US"));
                numericUpDownBillDay.Value = decimal.Parse(ConfigurationManager.AppSettings["Bill Day"],  new CultureInfo("en-US"));
                numericUpDownTimeUnit.Value = decimal.Parse(ConfigurationManager.AppSettings["Time Unit"],  new CultureInfo("en-US"));
                textBoxFee.Text = ConfigurationManager.AppSettings["Fee"];
                textBoxCallFee.Text = ConfigurationManager.AppSettings["Call Fee"];
                //------------------------------------------------------------------------------------
            }
            catch
            {
                MessageBox.Show("Error loading bills settings. Application will now exit, try to start it again.");
                this.Close();
            }
            
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (numericUpDownBillDay.Value.ToString() != ConfigurationManager.AppSettings["Bill Day"]) // if bill day is changed
                {
                    if (MessageBox.Show("Changing the Billing Day will clear all previous bills and recreate them again according to the call log. Are you sure?", "Confirm Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        ConfigurationManager.AppSettings["Bill Day"] = numericUpDownBillDay.Value.ToString();
                        saveBillSettings(); // save settings to file
                        dtBills.Clear();    // clear the bills
                        daBills.Update(dsBills, "Historia Bills");
                        SetDefaultLocale(new CultureInfo(systemCulture));
                        this.Owner.Enabled = true;
                        this.Owner.Show();
                        this.Close();
                    }
                    else
                    {
                        numericUpDownBillDay.Value = int.Parse(ConfigurationManager.AppSettings["Bill Day"]); // restore bill day back
                    }
                }
                else
                {
                    DialogResult res = MessageBox.Show("Save settings to all previous months? tap Yes to save to all months (this will clear all previous bills and recreate them again according to to the call log) or No to save just to the current and future months.", "Choose Months", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button3);
                    if (res != DialogResult.Cancel)
                    {
                        saveBillSettings(); // save settings to file
                        if (res == DialogResult.Yes)
                        {
                            dtBills.Clear();    // clear the bills
                            daBills.Update(dsBills, "Historia Bills");
                        }
                        SetDefaultLocale(new CultureInfo(systemCulture));
                        this.Owner.Enabled = true;
                        this.Owner.Show();
                        this.Close();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error saving bills settings. Application will now exit, try to start it again.");
                this.Close();
            }
        }
        public static void SetDefaultLocale(System.Globalization.CultureInfo locale)    // change culture
        {

            if (null == locale)
            {

                throw new ArgumentNullException("locale");

            }

            System.Reflection.FieldInfo fi = typeof(System.Globalization.CultureInfo).GetField("m_userDefaultCulture", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);



            if (null == fi)
            {

                throw new NotSupportedException("Setting locale is not supported in this version of the framework.");

            }



            fi.SetValue(null, locale);

        }
    }
}