using System.Collections.Generic;

namespace Com.Huen.DataModel
{
    public class Customers
    {
        public string Cst_Idx { get; set; }
        public string Cstg_Idx { get; set; }
        public string Cst_Name { get; set; }
        public string Cst_Email { get; set; }
        public string Cst_Addr { get; set; }
        public string Cst_Homepage { get; set; }
        public string Cst_Memo { get; set; }
    }

    public class CustomersTel
    {
        public string Cust_Idx { get; set; }
        public string Cust_Tel { get; set; }
        public string Cust_Name { get; set; }
        public string Cust_Duty { get; set; }
    }
}
