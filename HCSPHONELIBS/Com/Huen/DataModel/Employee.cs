using System.Collections.Generic;

namespace Com.Huen.DataModel
{
    public class Employee
    {
        private string _callstatus = "0";
        private bool _ischecked = false;
        private string _part_idx = string.Empty;
        private string _duty_idx = string.Empty;
        private string _cstg_idx = string.Empty;

        public string CallStatus
        {
            get { return _callstatus; }
            set { _callstatus = value; }
        }
        public bool IsChecked
        {
            get { return _ischecked; }
            set { _ischecked = value; }
        }
        public string Part_Idx
        {
            get
            {
                if (string.IsNullOrEmpty(_part_idx))
                    _part_idx = "0";

                return _part_idx;
            }
            set
            {
                _part_idx = value;
            }
        }
        public string Part_Name { get; set; }
        public string Emp_Idx { get; set; }
        public string Emp_Name { get; set; }
        public string Emp_Id { get; set; }
        public string Duty_Idx
        {
            get
            {
                if (string.IsNullOrEmpty(_duty_idx))
                    _duty_idx = "0";

                return _duty_idx;
            }
            set
            {
                _duty_idx = value;
            }
        }
        public string Duty_Name { get; set; }
        public string Cstg_Idx
        {
            get
            {
                if (string.IsNullOrEmpty(_cstg_idx))
                    _cstg_idx = "0";

                return _cstg_idx;
            }
            set
            {
                _cstg_idx = value;
            }
        }
        public string Emp_Email { get; set; }
        public string Emp_Phone { get; set; }
        public string Emp_Addr { get; set; }
        public string Emp_Tel { get; set; }
        public string Emp_Memo { get; set; }
        public string Right_Idx { get; set; }
    }
}
