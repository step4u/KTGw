using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Huen.DataModel
{
    public class UserInfo
    {
        private string _com_idx = string.Empty;

        public string EMP_IDX { get; set; }
        public string EMP_ID { get; set; }
        public string EMP_NAME { get; set; }
        public string EMP_TEL { get; set; }
        public string COM_IDX
        {
            get
            {
                if (string.IsNullOrEmpty(_com_idx)) _com_idx = "1";
                
                return _com_idx;
            }
            set
            {
                _com_idx = value;
            }
        }
        public string RIGHT_IDX { get; set; }
    }
}
