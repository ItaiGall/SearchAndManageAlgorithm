using System;
using System.Collections.Generic;
using System.Text;

namespace DSProject_ItaiG.ClassLibrary
{
    class TimeData
    {
        public DateTime timeStamp;
        public BoxBase xValue;
        public BoxHeight yValue;

        internal TimeData(BoxBase xValue, BoxHeight yValue)
        {
            this.xValue = xValue;
            this.yValue = yValue;
            timeStamp = DateTime.Now;
        }
    }
}
