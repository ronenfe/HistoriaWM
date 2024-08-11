using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using FreshLogicStudios.Atlas.Mobile.PocketPC;
using Historia;
using System.Reflection;
using System.Drawing.Imaging;
using System.Data;

namespace OwnerDrawnListBox
{
    // Derive an implementation of the
    // OwnerDrawnListBox class
    public class CallsListBox : OwnerDrawnListBox
    {
        const int DRAW_OFFSET = 5;
        private Font myFont;
        private Form1 m_form1;
        private int m_fontHeight;
        public CallsListBox(Form1 form1)
        {
            m_form1 = form1;
            if (ConfigurationManager.AppSettings["FontType"] == "Regular")      // set font from app.config
                myFont = new System.Drawing.Font(ConfigurationManager.AppSettings["Font"], float.Parse(ConfigurationManager.AppSettings["FontSize"]), System.Drawing.FontStyle.Regular);
            else if (ConfigurationManager.AppSettings["FontType"] == "Bold")
                myFont = new System.Drawing.Font(ConfigurationManager.AppSettings["Font"], float.Parse(ConfigurationManager.AppSettings["FontSize"]), System.Drawing.FontStyle.Bold);
            else
                myFont = new System.Drawing.Font(ConfigurationManager.AppSettings["Font"], float.Parse(ConfigurationManager.AppSettings["FontSize"]), System.Drawing.FontStyle.Italic);
            
            Graphics g = this.CreateGraphics();
//            myFont = new Font(this.Font.Name, FONT_SIZE, FontStyle.Regular);
            m_fontHeight = (int)g.MeasureString("123", myFont).Height;
            this.ItemHeight = (int)(m_fontHeight * 275 / 100);
            g.Dispose();
        }


        // Determine what the text color should be
        // for the selected item drawn as highlighted
        Color CalcTextColor(Color backgroundColor)
        {
            if (backgroundColor.Equals(Color.Empty))
                return Color.Black;

            int sum = backgroundColor.R + backgroundColor.G + backgroundColor.B;

            if (sum > 256)
                return Color.Black;
            else
                return Color.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Color fontColor;
            int pos = 25;	// starting position of strings
            // The base class contains a bitmap, offScreen, for constructing
            // the control and is rendered when all items are populated.
            // This technique prevents flicker.
            Graphics gOffScreen = Graphics.FromImage(this.OffScreen);
            gOffScreen.FillRectangle(new SolidBrush(this.BackColor), this.ClientRectangle);

            int itemTop = 0;
            int rightBound;
            DataTable tempDataTable;
            Pen grayPen = new Pen(Color.LightGray);
            String stringNumber;
            String stringName;
            String stringTime;
            String stringDuration;
            // Draw the calls in the list.

            if (m_form1.filteredByNumber == true)
                tempDataTable = m_form1.dtNumberFiltered;
            else if (m_form1.filteredByType == true)
                tempDataTable = m_form1.dtTypedFiltered;
            else
                tempDataTable = m_form1.dtHistoriaCalls;
            for (int n = this.VScrollBar.Value; n < Math.Min(this.VScrollBar.Value + DrawCount + 1, this.Items.Count); n++)
            {
                 // If the scroll bar is visible, subtract the scrollbar width
                 // otherwise subtract 2 for the width of the rectangle
                rightBound = this.ClientSize.Width - (this.VScrollBar.Visible ? this.VScrollBar.Width : 2);

                stringNumber = ellipsisText(gOffScreen, tempDataTable.Rows[n]["Number"].ToString(), myFont, (rightBound - pos - DRAW_OFFSET) * 41 / 100);
                stringName = ellipsisText(gOffScreen, tempDataTable.Rows[n]["Name"].ToString(), myFont, (rightBound - pos - DRAW_OFFSET) * 75 / 100);
                stringTime = ellipsisText(gOffScreen, tempDataTable.Rows[n]["Time"].ToString(), myFont, (rightBound - pos - DRAW_OFFSET) * 59 / 100 - 10);
                stringDuration = ellipsisText(gOffScreen, tempDataTable.Rows[n]["Duration"].ToString(), myFont, (rightBound - pos - DRAW_OFFSET) * 25 / 100 - 10);

                // Draw the selected item to appear highlighted
                if (n == this.SelectedIndex)
                {
                    gOffScreen.FillRectangle(new SolidBrush(SystemColors.Highlight),
                        0,
                        itemTop,
                        rightBound,
                        this.ItemHeight);
                    fontColor = Color.White;

                    //Draw the data
                    //gOffScreen.DrawString(tempDataTable.Rows[n]["Number"].ToString(), myFont, new SolidBrush(fontColor), pos + DRAW_OFFSET, itemTop + m_fontHeight * 25 / 100);
                    //gOffScreen.DrawString(tempDataTable.Rows[n]["Name"].ToString(), myFont, new SolidBrush(fontColor), pos + DRAW_OFFSET, itemTop + m_fontHeight * 150 / 100);
                    //gOffScreen.DrawString(tempDataTable.Rows[n]["Time"].ToString(), myFont, new SolidBrush(fontColor), rightBound - (int)(e.Graphics.MeasureString(tempDataTable.Rows[n]["Time"].ToString(), myFont).Width) - 5, itemTop + m_fontHeight * 25 / 100);
                    //gOffScreen.DrawString(tempDataTable.Rows[n]["Duration"].ToString(), myFont, new SolidBrush(fontColor), rightBound - (int)(e.Graphics.MeasureString(tempDataTable.Rows[n]["Duration"].ToString(), myFont).Width) - 5, itemTop + m_fontHeight * 150 / 100);
                    gOffScreen.DrawString(stringNumber, myFont, new SolidBrush(fontColor), pos + DRAW_OFFSET, itemTop + m_fontHeight * 25 / 100);
                    gOffScreen.DrawString(stringName, myFont, new SolidBrush(fontColor), pos + DRAW_OFFSET, itemTop + m_fontHeight * 150 / 100);
                    gOffScreen.DrawString(stringTime, myFont, new SolidBrush(fontColor), rightBound - gOffScreen.MeasureString(stringTime, myFont).Width - 5, itemTop + m_fontHeight * 25 / 100);
                    gOffScreen.DrawString(stringDuration, myFont, new SolidBrush(fontColor), rightBound - gOffScreen.MeasureString(stringDuration, myFont).Width - 5, itemTop + m_fontHeight * 150 / 100);
                }
                else
                {
                    fontColor = this.ForeColor;
                    //Draw the data
                    //gOffScreen.DrawString(tempDataTable.Rows[n]["Number"].ToString(), myFont, new SolidBrush(fontColor), pos + DRAW_OFFSET, itemTop + m_fontHeight * 25 / 100);
                    //gOffScreen.DrawString(tempDataTable.Rows[n]["Name"].ToString(), myFont, new SolidBrush(fontColor), pos + DRAW_OFFSET, itemTop + m_fontHeight * 150 / 100);
                    //gOffScreen.DrawString(tempDataTable.Rows[n]["Time"].ToString(), myFont, new SolidBrush(fontColor), rightBound - (int)(e.Graphics.MeasureString(tempDataTable.Rows[n]["Time"].ToString(), myFont).Width) - 5, itemTop + m_fontHeight * 25 / 100);
                    //gOffScreen.DrawString(tempDataTable.Rows[n]["Duration"].ToString(), myFont, new SolidBrush(fontColor), rightBound - (int)(e.Graphics.MeasureString(tempDataTable.Rows[n]["Duration"].ToString(), myFont).Width) - 5, itemTop + m_fontHeight * 150 / 100);
                    gOffScreen.DrawString(stringNumber, myFont, new SolidBrush(fontColor), pos + DRAW_OFFSET, itemTop + m_fontHeight * 25 / 100);
                    gOffScreen.DrawString(stringName, myFont, new SolidBrush(fontColor), pos + DRAW_OFFSET, itemTop + m_fontHeight * 150 / 100);
                    gOffScreen.DrawString(stringTime, myFont, new SolidBrush(fontColor), rightBound - gOffScreen.MeasureString(stringTime,myFont).Width - 5, itemTop + m_fontHeight * 25 / 100);
                    gOffScreen.DrawString(stringDuration, myFont, new SolidBrush(fontColor), rightBound - gOffScreen.MeasureString(stringDuration, myFont).Width - 5, itemTop + m_fontHeight * 150 / 100);
                }
                // Draw the item
 //               gOffScreen.DrawString((string)this.Items[n], myFont, new SolidBrush(fontColor), DRAW_OFFSET, itemTop);

                // Begin bitmap section
                string pngName;
                if (tempDataTable.Rows[n]["Type"].ToString() == "Incoming")
                    pngName = "Historia.call_type_incoming.png";
                else if (tempDataTable.Rows[n]["Type"].ToString() == "Outgoing")
                    pngName = "Historia.call_type_outgoing.png";
                else
                    pngName = "Historia.call_type_error.png";

                Bitmap iconCallType = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream(pngName));

                // TODO: cache these instead of calculating every time
                int pngWidth = iconCallType.Width;
                ImageAttributes transparentAttributes = new ImageAttributes();
                Color transparentColor = iconCallType.GetPixel(iconCallType.Width - 1, iconCallType.Height - 1);
                transparentAttributes.SetColorKey(transparentColor, transparentColor);
                Rectangle destinationRectangle = new Rectangle(DRAW_OFFSET,itemTop + (int)(m_fontHeight * 75 / 100), iconCallType.Width, iconCallType.Height);
                gOffScreen.DrawImage(iconCallType, destinationRectangle, 0, 0, iconCallType.Width, iconCallType.Height, GraphicsUnit.Pixel, transparentAttributes);

                // End bitmap section

                itemTop += this.ItemHeight;
                gOffScreen.DrawLine(grayPen, 0, itemTop, rightBound, itemTop);
            }

            // Draw the list box
            e.Graphics.DrawImage(this.OffScreen, 0, 0);

            gOffScreen.Dispose();
        }

 //        Draws the external border around the control.

        protected override void OnPaintBackground(PaintEventArgs e)
        {
         //   e.Graphics.DrawRectangle(new Pen(Color.Black), 0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }

        private void FontListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        public string ellipsisText(Graphics g, string text, Font myFont, float width)
        {
            if (text != string.Empty)
            {

                string shorterendText = "";
                int i;
                if (g.MeasureString(text, myFont).Width > width)
                {
                    for (i = 0; g.MeasureString(shorterendText, myFont).Width < width; i++)
                    {
                        shorterendText += text[i];
                    }
                    if (shorterendText.Length >= 2)
                    {
                        shorterendText = shorterendText.Remove(shorterendText.Length - 2, 2);
                        float test = g.MeasureString(shorterendText, myFont).Width;
                        shorterendText += '…';
                    }
                    return shorterendText;
                }               
            }
            return text;
        }

    }

}
