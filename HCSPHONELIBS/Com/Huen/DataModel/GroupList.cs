using System.Collections.Generic;
using System.Collections.ObjectModel;

using Com.Huen.Libs;
using Com.Huen.Sql;
using System.Data;

namespace Com.Huen.DataModel
{
    public class GroupList
    {
        private string _cstg_idx = string.Empty;
        private string _cstg_name = string.Empty;
        private string _cstg_depth = string.Empty;
        //private string _cstg_parent = string.Empty;

        public string Cstg_Idx
        {
            get { return _cstg_idx; }
            set { _cstg_idx = value; }
        }

        public string Cstg_Name
        {
            get { return _cstg_name; }
            set { _cstg_name = value; }
        }

        public string Cstg_Depth
        {
            get { return _cstg_depth; }
            set { _cstg_depth = value; }
        }

        //public string Cstg_Parent
        //{
        //    get { return _cstg_parent; }
        //    set { _cstg_parent = value; }
        //}
    }

    public class GroupLists : List<GroupList>
    {
        public GroupLists()
        {
            DataTable dt = util.MakeDataTable2Proc();
            DataRow dr = dt.NewRow();
            dr["DataName"] = "@i_com_idx";
            dr["DataValue"] = util.Userinfo.COM_IDX;
            dt.Rows.Add(dr);

            using (FirebirdDBHelper db = new FirebirdDBHelper(util.strDBConn))
            {
                try
                {
                    dt = db.GetDataTableSP("GET_CUSTGROUP_LIST", dt);
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
            }

            GroupList gl = new GroupList() {
                Cstg_Idx = "0"
                , Cstg_Name = util.LoadProjectResource("TEXT_CB_FIRSTFIELD", "COMMONRES", "").ToString()
                , Cstg_Depth = "1"
            };

            this.Add(gl);

            foreach (DataRow myRow in dt.Rows)
            {
                gl = new GroupList() {
                    Cstg_Idx = myRow["cstg_idx"].ToString()
                    , Cstg_Name = myRow["cstg_name"].ToString()
                    , Cstg_Depth = myRow["cstg_depth"].ToString()
                };

                this.Add(gl);
            }
        }
    }
}
