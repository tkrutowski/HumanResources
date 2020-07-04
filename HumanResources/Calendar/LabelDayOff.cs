using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HumanResources.Calendar
{
    public class LabelDayOff : Label
    {
        public LabelDayOff()
        {
            //base.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left)| System.Windows.Forms.AnchorStyles.Right));
            base.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            base.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            base.ForeColor = System.Drawing.Color.White;
            base.Size = new System.Drawing.Size(144, 17);
            base.AutoSize = true;
        }

        public LabelDayOff AddText(string s)
        {
            base.Text = s;
            //base.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            //| System.Windows.Forms.AnchorStyles.Right)));
            return this;
        }
    }
}
