using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basketball_Roster_Manager
{
    class ComboBoxItem
    {
        public string Name;
        public string Value;
        public ComboBoxItem(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
        public ComboBoxItem(object Name, object Value)
        {
            this.Name = Name.ToString();
            this.Value = Value.ToString();
        }
        public ComboBoxItem()
        {

        }

        // override ToString() function
        public override string ToString()
        {
            return this.Name;
        }
    }
}
