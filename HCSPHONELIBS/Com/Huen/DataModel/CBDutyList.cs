using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Com.Huen.Libs;
using Com.Huen.Sql;
using System.Data;

namespace Com.Huen.DataModel
{
    public class CBDutyList
    {
        public string Duty_Idx
        {
            get;
            set;
        }

        public string Duty_Name
        {
            get;
            set;
        }
    }

    public class CBDutyLists : List<CBDutyList>
    {
        public CBDutyLists()
        {
            DataTable dt = null;

            using (FirebirdDBHelper db = new FirebirdDBHelper(util.strDBConn))
            {
                try
                {
                    dt = db.GetDataTableSP("GET_DUTY_INFO");
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
            }

            CBDutyList gl = new CBDutyList()
            {
                Duty_Idx = "0"
                , Duty_Name = util.LoadProjectResource("TEXT_CB_FIRSTFIELD", "COMMONRES", "").ToString()
            };

            this.Add(gl);

            foreach (DataRow myRow in dt.Rows)
            {
                gl = new CBDutyList()
                {
                    Duty_Idx = myRow["o_duty_idx"].ToString()
                    , Duty_Name = myRow["o_duty_name"].ToString()
                };

                this.Add(gl);
            }
        }
    }
}
