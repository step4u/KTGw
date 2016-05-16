using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Com.Huen.DataModel;

namespace Com.Huen.Libs
{
    public class IsExtensionComparer : IComparer<RecInfos>
    {
        public int Compare(RecInfos x, RecInfos y)
        {
            if (x.seq > y.seq)
                return 1;
            else if (x.seq < y.seq)
                return -1;
            else
                return 0;
        }
    }
}
