using System.Collections.Generic;

namespace Com.Huen.DataModel
{
    public class Customer
    {
        private bool _ischecked = false;

        public bool IsChecked
        {
            get { return _ischecked; }
            set { _ischecked = value; }
        }
        public string Cst_Idx { get; set; }
        public string Cstg_Idx { get; set; }
        public string Cst_Name { get; set; }
        public string Cst_Addr { get; set; }
        public string Cust_Name { get; set; }
        public string Cust_Role { get; set; }
        public string Cust_Tel { get; set; }
        public string Cst_Memo { get; set; }
    }
}
