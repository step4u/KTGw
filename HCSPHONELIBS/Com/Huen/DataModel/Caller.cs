using System.Collections.Generic;

namespace Com.Huen.DataModel
{
    public class Caller
    {
        private string _cst_idx = string.Empty;

        public string Cl_Idx { get; set; }
        public string Cl_Status { get; set; }
        public string Cl_Forward { get; set; }
        public string Cst_Idx
        {
            get
            {
                return _cst_idx;
            }
            set
            {
                if (value == "")
                    _cst_idx = "0";
                else
                    _cst_idx = value;
            }
        }
        public string Cst_Name { get; set; }
        public string Cust_Name { get; set; }
        public string Cust_Role { get; set; }
        public string Caller_Tel { get; set; }
        public string Callee_Tel { get; set; }
        public string REGDATE { get; set; }
        public string Callingtime { get; set; }
        public string Cl_Memo { get; set; }
        public string Cl_Caller
        {
            get
            {
                return string.Format("{0} ({1})", Caller_Tel, Cst_Name);
            }
        }
        //public List<ChildInfo> Cl_Children
        //{
        //    get
        //    {
        //        List<ChildInfo> __list = new List<ChildInfo>();
        //        __list.Add(new ChildInfo() {
        //            CH_IDX = "1"
        //        });

        //        return __list;
        //    }
        //}
    }
}
