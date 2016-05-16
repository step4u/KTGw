using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using Com.Huen.Libs;

namespace Com.Huen.Data
{
    public class EmployeeList
    {
        public EmployeeList() { }

        private string _strConn = string.Empty;
        public string strConn
        {
            get
            {
                return (string)util.LoadProjectResource("DBCONSTR_MDB", "CommonRes", "");
            }
        }
    }
}
