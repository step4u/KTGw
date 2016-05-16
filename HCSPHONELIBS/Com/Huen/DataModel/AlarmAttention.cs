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
    public class AlarmAttention
    {
        public string Ch_Idx { get; set; }
        public string Ch_Name { get; set; }
        public string Evt_Title { get; set; }
        public string Evt_Memo { get; set; }
        public string Evt_Sdate { get; set; }
    }

    public class AlarmAttentions
    {
        private ObservableCollection<AlarmAttention> _obslist;

        public AlarmAttentions()
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
                    dt = db.GetDataTableSP("GET_ALARM_ATTENTION", dt);
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException fe)
                {
                    throw fe;
                }

                _obslist = new ObservableCollection<AlarmAttention>(
                                    (from myRow in dt.AsEnumerable()
                                     select new AlarmAttention()
                                     {
                                         Ch_Idx = myRow["o_ch_idx"].ToString()
                                         , Ch_Name = myRow["o_ch_name"].ToString()
                                         , Evt_Title = myRow["o_evt_title"].ToString()
                                         , Evt_Memo = myRow["o_evt_memo"].ToString()
                                         , Evt_Sdate = myRow["o_evt_sdate"].ToString()
                                     }).ToList<AlarmAttention>());
            }
        }

        public ObservableCollection<AlarmAttention> GetItems
        {
            get { return _obslist; }
        }

        public int GetCount
        {
            get { return _obslist.Count; }
        }
    }
}
