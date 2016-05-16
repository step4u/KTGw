using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Globalization;

namespace Com.Huen.Libs
{
    class StrComparer : IComparer<String>
    {
        public int Compare(string x, string y)
        {
            return CultureInfo.CurrentCulture.CompareInfo.Compare(x, y, CompareOptions.StringSort);
        }
    }
}
