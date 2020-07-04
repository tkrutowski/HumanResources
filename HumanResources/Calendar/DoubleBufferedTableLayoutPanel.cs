using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HumanResources.Calendar
{
    class DoubleBufferedTableLayoutPanel: TableLayoutPanel
    {
        public DoubleBufferedTableLayoutPanel()
        {
            DoubleBuffered = true;
        }
    }
}
