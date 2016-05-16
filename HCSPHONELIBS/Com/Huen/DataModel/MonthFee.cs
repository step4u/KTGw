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
    public class MonthFee
    {
        //public string FEE_IDX { get; set; }
        public string P_Idx { get; set; }
        public string P_Name { get; set; }
        public string P_Tel { get; set; }
        public string P_Info
        {
            get
            {
                return string.Format("{0} ({1})", P_Name, P_Tel);
            }
        }
        public string CH_Idx { get; set; }
        public string CH_Name { get; set; }
        public string FEE_DelayMonth { get; set; }
        public string FEE_STATUS { get; set; }
    }

    public class MonthFees
    {
        private ObservableCollection<MonthFee> _monthfees;

        public MonthFees()
        {
            /*
            _monthfees.Add(new MonthFee() {
                CH_Idx = "6"
                , CH_Name = "어린이6"
                , FEE_DelayMonth = "1"
                , P_Idx = "19"
                , P_Name = "부모19"
                , P_Tel = "070-4694-6730"
            });

            _monthfees.Add(new MonthFee()
            {
                CH_Idx = "7"
                ,
                CH_Name = "어린이7"
                ,
                FEE_DelayMonth = "1"
                ,
                P_Idx = "19"
                ,
                P_Name = "부모19"
                ,
                P_Tel = "070-4694-6730"
            });
            */

            using (FirebirdDBHelper db = new FirebirdDBHelper(util.strDBConn))
            {
                DataTable dt = util.MakeDataTable2Proc();

                DataRow dr = dt.NewRow();
                dr["DataName"] = "@i_com_idx";
                dr["DataValue"] = util.Userinfo.COM_IDX;
                dt.Rows.Add(dr);

                dr = dt.NewRow();
                dr["DataName"] = "@i_day";
                dr["DataValue"] = util.i_day;
                dt.Rows.Add(dr);

                try
                {
                    dt = db.GetDataTableSP("GET_MONTHFEE_LIST", dt);
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException fe)
                {
                    throw fe;
                }

                _monthfees = new ObservableCollection<MonthFee>(
                                    (from myRow in dt.AsEnumerable()
                                     select new MonthFee()
                                     {
                                         P_Idx = myRow["o_p_idx"].ToString()
                                         , P_Name = myRow["o_p_name"].ToString()
                                         , P_Tel = myRow["o_p_tel"].ToString()
                                         , CH_Idx = myRow["o_ch_idx"].ToString()
                                         , CH_Name = myRow["o_ch_name"].ToString()
                                         , FEE_DelayMonth = myRow["o_fee_delay"].ToString()
                                         , FEE_STATUS = myRow["o_curstatus"].ToString()
                                     }).ToList<MonthFee>());
            }
        }

        public ObservableCollection<MonthFee> GetMonthFees
        {
            get { return _monthfees; }
        }
    }
}
