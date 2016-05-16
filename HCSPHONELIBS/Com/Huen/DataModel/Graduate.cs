using System.Collections.Generic;
using System.Collections.ObjectModel;

using Com.Huen.Libs;
using Com.Huen.Sql;
using System.Data;

namespace Com.Huen.DataModel
{
    public class Graduate
    {
        private string _idx = string.Empty;
        private string _name = string.Empty;

        public string Idx
        {
            get { return _idx; }
            set { _idx = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }

    public class GraduateCombo
    {
        private List<Graduate> _combolist = new List<Graduate>();

        public List<Graduate> GetComboList
        {
            get { return _combolist; }
        }

        public GraduateCombo()
        {
            _combolist.Add(
                    new Graduate()
                    {
                        Idx = "0"
                        , Name = "재학"
                    }
                );

            _combolist.Add(
                    new Graduate()
                    {
                        Idx = "1"
                        ,
                        Name = "졸업"
                    }
                );
        }
    }
}
