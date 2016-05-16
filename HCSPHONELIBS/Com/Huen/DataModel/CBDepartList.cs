using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Com.Huen.Libs;
using Com.Huen.Sql;
using System.Data;

namespace Com.Huen.DataModel
{
    public class CBDepartList
    {
        public string Part_Idx
        {
            get;
            set;
        }

        public string Part_Name
        {
            get;
            set;
        }
    }

    public class CBDeparLists : List<CBDepartList>
    {
        public CBDeparLists()
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
                    dt = db.GetDataTableSP("GET_DEPARTMENT_LIST", dt);
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
            }

            CBDepartList gl = new CBDepartList()
            {
                Part_Idx = "0"
                , Part_Name = util.LoadProjectResource("TEXT_CB_FIRSTFIELD", "COMMONRES", "").ToString()
            };

            this.Add(gl);

            foreach (DataRow myRow in dt.Rows)
            {
                gl = new CBDepartList()
                {
                    Part_Idx = myRow["part_idx"].ToString()
                    , Part_Name = myRow["part_name"].ToString()
                };

                this.Add(gl);
            }
        }
    }
}
