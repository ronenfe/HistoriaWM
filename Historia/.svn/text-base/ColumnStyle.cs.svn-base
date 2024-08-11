
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace Historia
{
    public class ColumnStyle : DataGridTextBoxColumn
    {
        public event CheckCellEventHandler CheckCellEquals; // event raised when cell is equal something

        private int _col;   // column local variable

        public ColumnStyle(int column)  // set local column variable
        {
            _col = column;
        }

        protected override void Paint(Graphics g, Rectangle Bounds, CurrencyManager Source, int RowNum, Brush BackBrush, Brush ForeBrush, bool AlignToRight)    // overrided paint method to color the cells
        {


            if (CheckCellEquals != null)    // if cell fits criteria
            {
                DataGridEnableEventArgs e = new DataGridEnableEventArgs(RowNum, _col);  // create new event arguments
                CheckCellEquals(this, e);   // check cell criteria
                if (e.MeetsCriteria == meetsCriteriaEnum.outgoing)  // set backcolor of cell according to criteria
                    BackBrush = new SolidBrush(Color.Green);
                else if (e.MeetsCriteria == meetsCriteriaEnum.incoming)
                    BackBrush = new SolidBrush(Color.Red);
                else
                    BackBrush = new SolidBrush(Color.Yellow);
            }
            base.Paint(g, Bounds, Source, RowNum, BackBrush, ForeBrush, AlignToRight);  // call paint with custom values
        }

    }
    public delegate void CheckCellEventHandler(object sender, DataGridEnableEventArgs e);   // create a delegate for checking cell criteria

    public enum meetsCriteriaEnum { incoming, outgoing, missed };   // enumerates acording to the call type

    public class DataGridEnableEventArgs : EventArgs    // create event arguments
    {
        private int _column;
        private int _row;
        private meetsCriteriaEnum _meetsCriteria;
        public DataGridEnableEventArgs(int row, int col)    // set local row and column
        {
            _row = row;
            _column = col;
        }

        public int Column
        {
            get { return _column; }
            set { _column = value; }
        }

        public int Row
        {
            get { return _row; }
            set { _row = value; }
        }

        public meetsCriteriaEnum MeetsCriteria
        {
            get { return _meetsCriteria; }
            set { _meetsCriteria = value; }
        }
    }

    
}
