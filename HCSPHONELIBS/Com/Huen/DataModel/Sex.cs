using System.Collections.Generic;
using System.Collections.ObjectModel;

using Com.Huen.Libs;
using Com.Huen.Sql;
using System.Data;

namespace Com.Huen.DataModel
{
    public class Sex
    {
        private string _s_idx = string.Empty;
        private string _s_name = string.Empty;

        public string S_Idx
        {
            get { return _s_idx; }
            set { _s_idx = value; }
        }
        public string S_Name
        {
            get { return _s_name; }
            set { _s_name = value; }
        }
    }

    public class Sexes
    {
        private List<Sex> _combolist = new List<Sex>();

        public List<Sex> GetComboSex
        {
            get { return _combolist; }
        }

        public Sexes()
        {
            //FamilyRole fr = new FamilyRole() {
            //    Fr_Idx = "0"
            //    , Fr_Name = util.LoadProjectResource("TEXT_CB_FIRSTFIELD", "COMMONRES", "").ToString()
            //};

            _combolist.Add(
                    new Sex()
                    {
                        S_Idx = "1"
                        , S_Name = "남"
                    }
                );

            _combolist.Add(
                    new Sex() {
                        S_Idx = "0"
                        , S_Name = "여"
                    }
                );
        }
    }
}
