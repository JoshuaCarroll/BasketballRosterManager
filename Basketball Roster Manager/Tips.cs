using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Basketball_Roster_Manager
{
    class Tips
    {
        public ToolTip toolTip;
        public bool showCountTip;
        public string tip;

        public Tips()
        {
            toolTip = new ToolTip();
            showCountTip = true;
            tip = "Tip: Double-click this field to add one to the current value.";
        }
    }
}
