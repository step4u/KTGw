using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Huen.DataModel
{
    public class Parent
    {
        private bool _ischecked = false;

        public bool IsChecked
        {
            get { return _ischecked; }
            set { _ischecked = value; }
        }
        public string P_Idx { get; set; }
        public string P_Name { get; set; }
        public string P_Addr { get; set; }
        public string P_Tel { get; set; }
        public string P_Role { get; set; }
        public string P_Memo { get; set; }
        public string Cstg_Idx { get; set; }
        public string Cstg_Name { get; set; }
        public string CH_Idx { get; set; }
        public string CH_Name { get; set; }
    }
}
