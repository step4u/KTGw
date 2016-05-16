using System.Collections.Generic;

namespace Com.Huen.DataModel
{
    public class Department
    {
        public List<Department> _department = new List<Department>();

        public List<Department> Children
        {
            get { return _department; }
            set { _department = value; }
        }

        public string Name { get; set; }
        public string Idx { get; set; }
    }
}
