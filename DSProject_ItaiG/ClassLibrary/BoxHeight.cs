using System;
using System.Collections.Generic;
using System.Text;

namespace DSProject_ItaiG.ClassLibrary
{
    internal class BoxHeight : IComparable<BoxHeight>
    {
        internal double YValue;
        internal int Stock { get; set; }
        internal TimeManager<TimeData>.TimeNode timeNodeReference;

        public BoxHeight(double yvalue, int stock = -1)
        {
            YValue = yvalue;
            Stock = stock;
        }


        public int CompareTo(BoxHeight other)
        {
            if (YValue > other.YValue)
                return 1;
            else if (YValue == other.YValue)
                return 0;
            else
                return -1;
        }


    }


}
