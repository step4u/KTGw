using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Com.Huen.DataModel;
using System.Globalization;


namespace Com.Huen.Libs
{
    public class CustGroupComparer : IComparer<CustGroup>
    {
        public int Compare(CustGroup x, CustGroup y)
        {
            return CultureInfo.CurrentCulture.CompareInfo.Compare(x.Name, y.Name, CompareOptions.StringSort);
        }
    }
}
