using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Collections.ObjectModel;

using Com.Huen.Libs;
using Com.Huen.Sql;

namespace Com.Huen.DataModel
{
    public class AlarmBirth
    {
        public string Idx { get; set; }
        public string Name { get; set; }
        public string Birth { get; set; }
        public string Birth_Luna { get; set; }
        public string Cstg_Name { get; set; }
        public string Remain_Birth { get; set; }
    }

    public class AlarmBirths
    {
        private ObservableCollection<AlarmBirth> _obslist;

        public AlarmBirths()
        {
            using (FirebirdDBHelper db = new FirebirdDBHelper(util.strDBConn))
            {
                DataTable dt = util.MakeDataTable2Proc();

                DataRow dr = dt.NewRow();
                dr["DataName"] = "@i_com_idx";
                dr["DataValue"] = util.Userinfo.COM_IDX;
                dt.Rows.Add(dr);

                try
                {
                    dt = db.GetDataTableSP("GET_ALARM_BIRTH", dt);
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException fe)
                {
                    throw fe;
                }

                _obslist = new ObservableCollection<AlarmBirth>(
                                    (from myRow in dt.AsEnumerable()
                                     select new AlarmBirth()
                                     {
                                         Idx = myRow["o_idx"].ToString()
                                         , Name = myRow["o_name"].ToString()
                                         , Birth = myRow["o_birth"].ToString()
                                         , Birth_Luna = myRow["o_birth_luna"].ToString()
                                         , Cstg_Name = myRow["o_cstg_name"].ToString()
                                         , Remain_Birth = myRow["o_remain_birth"].ToString()
                                     }).ToList<AlarmBirth>());
            }
        }

        public ObservableCollection<AlarmBirth> GetItems
        {
            get { return _obslist; }
        }

        public int GetCount
        {
            get { return _obslist.Count; }
        }
    }
}
