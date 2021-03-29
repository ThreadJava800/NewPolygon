using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CircleDraw
{
    public class RadiusEventArgs : EventArgs
    {
        public int radius, prevRadius;
        public RadiusEventArgs(int radius, int prevRadius)
        {
            this.radius = radius;
            this.prevRadius = prevRadius;
        }
    }
}
