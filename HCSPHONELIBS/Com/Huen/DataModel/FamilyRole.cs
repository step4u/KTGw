using System.Collections.Generic;
using System.Collections.ObjectModel;

using Com.Huen.Libs;
using Com.Huen.Sql;
using System.Data;

namespace Com.Huen.DataModel
{
    public class FamilyRole
    {
        private string _fr_idx = string.Empty;
        private string _fr_name = string.Empty;
        private string _fr_sort = string.Empty;
        private string _fr_use = string.Empty;

        public string Fr_Idx
        {
            get { return _fr_idx; }
            set { _fr_idx = value; }
        }
        public string Fr_Name
        {
            get { return _fr_name; }
            set { _fr_name = value; }
        }
        public string Fr_Sort
        {
            get { return _fr_sort; }
            set { _fr_sort = value; }
        }
        public string Fr_Use
        {
            get { return _fr_use; }
            set { _fr_use = value; }
        }
    }

    public class FamilyRoles
    {
        private List<FamilyRole> _roles = new List<FamilyRole>();

        public List<FamilyRole> GetFamilyRole
        {
            get { return _roles; }
        }

        public FamilyRoles()
        {
            DataTable dt = null;

            using (FirebirdDBHelper db = new FirebirdDBHelper(util.strDBConn))
            {
                try
                {
                    dt = db.GetDataTableSP("GET_FAMILY_ROLE");
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
            }

            FamilyRole fr = new FamilyRole() {
                Fr_Idx = "0"
                , Fr_Name = util.LoadProjectResource("TEXT_CB_FIRSTFIELD", "COMMONRES", "").ToString()
                , Fr_Sort = "0"
                , Fr_Use = "1"
            };

            _roles.Add(fr);

            foreach (DataRow myRow in dt.Rows)
            {
                fr = new FamilyRole()
                {
                    Fr_Idx = myRow["o_fr_idx"].ToString()
                    ,
                    Fr_Name = myRow["o_fr_name"].ToString()
                    ,
                    Fr_Sort = myRow["o_fr_sort"].ToString()
                    ,
                    Fr_Use = myRow["o_fr_use"].ToString()
                };

                _roles.Add(fr);
            }
        }
    }
}
