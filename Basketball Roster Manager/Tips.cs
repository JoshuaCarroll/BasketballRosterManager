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
        public bool showFoulCountTip;

        public Tips()
        {
            toolTip = new ToolTip();
            showFoulCountTip = true;
        }
    }
}
