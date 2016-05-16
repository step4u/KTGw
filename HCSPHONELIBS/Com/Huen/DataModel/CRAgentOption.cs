using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

namespace Com.Huen.DataModel
{
    [Serializable()]
    public class CRAgentOption
    {
        public string SaveFileType { get; set; }
        public string SaveDirectory { get; set; }
        public bool FtpAutoTrans { get; set; }
        public string FtpAddr { get; set; }
        public string FtpAccount { get; set; }
        public string FtpPassword { get; set; }
        public string FtpTransMode { get; set; }
        public bool AgentAutoStart { get; set; }
    }
}
