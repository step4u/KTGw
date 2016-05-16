using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Huen.DataModel
{
    public class Sms
    {
        public string SmsIdx { get; set; }
        public string YYMMDD { get; set; }
        public string HHMMSS { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Memo { get; set; }
        public DateTime Regdate { get; set; }
    }
}
