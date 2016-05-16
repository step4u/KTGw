using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Com.Huen.DataModel
{
    public class CustGroup
    {
        private List<CustGroup> _custgroup = new List<CustGroup>();

        public List<CustGroup> Children
        {
            get { return _custgroup; }
            set { _custgroup = value; }
        }

        public string Name { get; set; }
        public string Idx { get; set; }
    }
}
