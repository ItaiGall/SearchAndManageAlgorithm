using System;
using System.Collections.Generic;
using System.Text;
using DSProject_ItaiG.ClassLibrary;

namespace DSProject_ItaiG.ClassLibrary
{
    internal class BoxBase : IComparable<BoxBase>
    {
        internal double XValue;
        public BinarySearchTree<BoxHeight> BoxHeightTree;


        public BoxBase(double xvalue)
        {            
            XValue = xvalue;
            BoxHeightTree = new BinarySearchTree<BoxHeight>();
        }

        public int CompareTo(BoxBase other)
        {
            if (XValue > other.XValue)
                return 1;
            else if (XValue == other.XValue)
                return 0;
            else
                return -1;
        }
        

    }
}
