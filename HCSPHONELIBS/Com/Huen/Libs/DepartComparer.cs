using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Com.Huen.DataModel;
using System.Globalization;

namespace Com.Huen.Libs
{
    public class DepartComparer : IComparer<Department>
    {
        public int Compare(Department x, Department y)
        {
            return CultureInfo.CurrentCulture.CompareInfo.Compare(x.Name, y.Name, CompareOptions.StringSort);
        }
    }
}
