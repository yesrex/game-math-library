using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.SlotUtility
{
    internal class Coordiante
    {
        internal int x;
        internal int y;
        internal int value;
        internal Coordiante(int x, int y, int value)
        {
            this.x = x;
            this.y = y;
            this.value = value;
        }
        internal Coordiante(int x, int y) 
        {
            this.x = x;
            this.y = y;
            this.value = 0;
        }

    }
}
