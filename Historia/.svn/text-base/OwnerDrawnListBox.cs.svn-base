using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

namespace OwnerDrawnListBox
{
    // Base custom control for DrawFontList
    public class OwnerDrawnListBox : Control
    {
        const int SCROLL_WIDTH = 15;
        int itemHeight = -1;
        int selectedIndex = -1;

        Bitmap offScreen;
        VScrollBar vs;
        ArrayList items;

        public OwnerDrawnListBox()
        {
            this.vs = new VScrollBar();
            this.vs.Parent = this;
            this.vs.Visible = false;
            this.vs.SmallChange = 1;
            this.vs.ValueChanged += new EventHandler(this.ScrollValueChanged);

            this.items = new ArrayList();
        }
        public void refreshScroll()
        {
            OnResize(new EventArgs());
        }
        public ArrayList Items
        {
            get
            {
                return this.items;
            }
        }

        protected Bitmap OffScreen
        {
            get
            {
                return this.offScreen;
            }
        }

        protected VScrollBar VScrollBar
        {
            get
            {
                return this.vs;
            }
        }

        public event EventHandler SelectedIndexChanged;

        // Raise the SelectedIndexChanged event
        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            if (this.SelectedIndexChanged != null)
                this.SelectedIndexChanged(this, e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.SelectedIndex = this.vs.Value + (e.Y / this.ItemHeight);

            // Invalidate the control so we can draw the item as selected.
            this.Refresh();
        }
        public void IndexFromPoint(Point pPoint)
        {
            this.SelectedIndex = this.vs.Value + (pPoint.Y / this.ItemHeight);
            this.Refresh();
        }
        // Get or set index of selected item.
        public int SelectedIndex
        {
            get
            {
                return this.selectedIndex;
            }

            set
            {
                this.selectedIndex = value;

                if (this.SelectedIndexChanged != null)
                    this.SelectedIndexChanged(this, EventArgs.Empty);

            }
        }

        protected void ScrollValueChanged(object o, EventArgs e)
        {
            this.Refresh();
        }

        protected virtual int ItemHeight
        {
            get
            {
                return this.itemHeight;
            }

            set
            {
                this.itemHeight = value;
            }
        }

        // If the requested index is before the first visible index then set the
        // first item to be the requested index. If it is after the last visible
        // index, then set the last visible index to be the requested index.
        public void EnsureVisible(int index)
        {
            if (index < this.vs.Value)
            {
                this.vs.Value = index;
                this.Refresh();
            }
            else if (index >= this.vs.Value + this.DrawCount)
            {
                this.vs.Value = index - this.DrawCount + 1;
                this.Refresh();
            }
        }


        // Need to set focus to the control when it
        // is clicked so that keyboard events occur.
        protected override void OnClick(EventArgs e)
        {
            this.Focus();
            base.OnClick(e);
        }

        // Selected item moves when you use the keyboard up/down keys.
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    if (this.SelectedIndex < this.vs.Maximum)
                    {
                        EnsureVisible(++this.SelectedIndex);
                        this.Refresh();
                    }
                    break;
                case Keys.Up:
                    if (this.SelectedIndex > this.vs.Minimum)
                    {
                        EnsureVisible(--this.SelectedIndex);
                        this.Refresh();
                    }
                    break;
                case Keys.PageDown:
                    this.SelectedIndex = Math.Min(this.vs.Maximum, this.SelectedIndex + this.DrawCount);
                    EnsureVisible(this.SelectedIndex);
                    this.Refresh();
                    break;
                case Keys.PageUp:
                    this.SelectedIndex = Math.Max(this.vs.Minimum, this.SelectedIndex - this.DrawCount);
                    EnsureVisible(this.SelectedIndex);
                    this.Refresh();
                    break;
                case Keys.Home:
                    this.SelectedIndex = 0;
                    EnsureVisible(this.SelectedIndex);
                    this.Refresh();
                    break;
                case Keys.End:
                    this.SelectedIndex = this.items.Count - 1;
                    EnsureVisible(this.SelectedIndex);
                    this.Refresh();
                    break;
            }

            base.OnKeyDown(e);
        }

        // Calculate how many items we can draw given the height of the control.
        protected int DrawCount
        {
            get
            {
                if (this.vs.Value + this.vs.LargeChange > this.vs.Maximum)
                    return this.vs.Maximum - this.vs.Value + 1;
                else
                    return this.vs.LargeChange;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            int viewableItemCount = this.ClientSize.Height / this.ItemHeight;

            this.vs.Bounds = new Rectangle(this.ClientSize.Width - SCROLL_WIDTH,
                0,
                SCROLL_WIDTH,
                this.ClientSize.Height);


            // Determine if scrollbars are needed
            if (this.items.Count > viewableItemCount)
            {
                this.vs.Visible = true;
                this.vs.LargeChange = viewableItemCount;
                this.offScreen = new Bitmap(this.ClientSize.Width - SCROLL_WIDTH - 1, this.ClientSize.Height - 2);
            }
            else
            {
                this.vs.Visible = false;
                this.vs.LargeChange = this.items.Count;
                this.offScreen = new Bitmap(this.ClientSize.Width - 1, this.ClientSize.Height - 2);
            }

            this.vs.Maximum = this.items.Count - 1;
        }
    }

}
