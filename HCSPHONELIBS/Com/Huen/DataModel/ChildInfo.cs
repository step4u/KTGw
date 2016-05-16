using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;

namespace Com.Huen.DataModel
{
    public class ChildInfo
    {
        public string P_IDX { get; set; }
        public string CH_IDX { get; set; }
        public string CH_NAME { get; set; }
        public string CH_SEX { get; set; }
        public DateTime CH_BIRTH { get; set; }
        public DateTime CH_ENTERANCE { get; set; }
        public string CH_GRADUATE { get; set; }
        public ObservableCollection<EventSchedule> CH_ATTENTIONS { get; set; }
    }
}
