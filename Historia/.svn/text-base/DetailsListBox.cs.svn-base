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
    public class DetailsListBox : OwnerDrawnListBox
    {
        const int DRAW_OFFSET = 5;
        private int m_fontHeight;
        public DetailsListBox()
        {            
            Graphics g = this.CreateGraphics();
            m_fontHeight = (int)g.MeasureString("123", Font).Height;
            this.ItemHeight = (int)(m_fontHeight * 3);
            g.Dispose();
        }

        // Determine what the text color should be
        // for the selected item drawn as highlighted
        protected override void OnPaint(PaintEventArgs e)
        {
            int pos = 25;	// starting position of strings
            // The base class contains a bitmap, offScreen, for constructing
            // the control and is rendered when all items are populated.
            // This technique prevents flicker.
            Graphics gOffScreen = Graphics.FromImage(this.OffScreen);
            gOffScreen.FillRectangle(new SolidBrush(this.BackColor), this.ClientRectangle);

            int itemTop = 0;
            int rightBound;
            Pen grayPen = new Pen(Color.LightGray);
            String stringText;
            // Draw the calls in the list.

            for (int n = this.VScrollBar.Value; n < Math.Min(this.VScrollBar.Value + DrawCount + 1, this.Items.Count); n++)
            {
                // If the scroll bar is visible, subtract the scrollbar width
                // otherwise subtract 2 for the width of the rectangle
                rightBound = this.ClientSize.Width - (this.VScrollBar.Visible ? this.VScrollBar.Width : 2);

                stringText = ellipsisText(gOffScreen, this.Items[n].ToString(), Font, (rightBound - pos - DRAW_OFFSET) - 5);

                gOffScreen.DrawString(stringText, Font, new SolidBrush(Color.Black), pos + DRAW_OFFSET, itemTop + m_fontHeight);

                // Begin bitmap section
                string pngName;
                switch (n)
                {
                    case 0:

                        if (this.Items[n].ToString() == "Incoming")
                            pngName = "Historia.call_type_incoming.png";
                        else if (this.Items[n].ToString() == "Outgoing")
                            pngName = "Historia.call_type_outgoing.png";
                        else
                            pngName = "Historia.call_type_error.png";
                        break;

                    case 1:
                        pngName = "Historia.call_details_date_and_time.png";
                        break;
                    case 2:
                        pngName = "Historia.call_details_duration.png";
                        break;
                    default:
                        pngName = "Historia.call_details_contact_card.png";
                        break;
                }
                         

                Bitmap iconCallType = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream(pngName));

                // TODO: cache these instead of calculating every time
                int pngWidth = iconCallType.Width;
                ImageAttributes transparentAttributes = new ImageAttributes();
                Color transparentColor = iconCallType.GetPixel(iconCallType.Width - 1, iconCallType.Height - 1);
                transparentAttributes.SetColorKey(transparentColor, transparentColor);
                Rectangle destinationRectangle = new Rectangle(DRAW_OFFSET, itemTop + m_fontHeight, iconCallType.Width, iconCallType.Height);
                gOffScreen.DrawImage(iconCallType, destinationRectangle, 0, 0, iconCallType.Width, iconCallType.Height, GraphicsUnit.Pixel, transparentAttributes);

                // End bitmap section

                itemTop += this.ItemHeight;
            }

            // Draw the list box
            e.Graphics.DrawImage(this.OffScreen, 0, 0);

            gOffScreen.Dispose();
        }

        //        Draws the external border around the control.

        protected override void OnPaintBackground(PaintEventArgs e)
        {
 
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
