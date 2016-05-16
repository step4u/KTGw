using System.Collections.Generic;

namespace Com.Huen.DataModel
{
    public class Student
    {
        private bool _ischecked = false;

        public bool IsChecked
        {
            get { return _ischecked; }
            set { _ischecked = value; }
        }
        public string CH_Idx { get; set; }
        public string CH_Name { get; set; }
        public string P_Idx { get; set; }
        public string P_Name { get; set; }
        public string P_Addr { get; set; }
        public string P_Tel { get; set; }
        public string P_Role { get; set; }
        public string P_Memo { get; set; }
        public string Cstg_Idx { get; set; }
        public string Cstg_Name { get; set; }
        public string Field_Parent
        {
            get
            {
                return string.Format("{0} ({1})", P_Name, P_Role);
            }
        }
    }
}
