using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AGV
{
    class Pallet
    {
        public string palletName;
        public int palletValue;

        public Pallet(string palletName)
        {
            if (string.Equals("小托盘", palletName))
            {
                this.palletValue = 1;
            }
            else if (string.Equals("小托盘", palletName))
            {
                this.palletValue = 0;
            }
        }

        public string Name
        {
            get { return palletName; }
            set { this.palletName = value; }
        }

        public int Value
        {
            get { return palletValue; }
        }
    }
}
