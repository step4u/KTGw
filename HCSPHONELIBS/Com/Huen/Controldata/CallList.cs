using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Com.Huen.Libs;
using Com.Huen.Sql;
using System.Data;

namespace Com.Huen.Controldata
{
    public class CallList
    {
        private string _strConn = string.Empty;
        public string strConn
        {
            get
            {
                return (string)util.LoadProjectResource("DBCONSTR_MSSQL", "COMMONRES", "");
            }
        }

        public CallList() { }

        public DataTable GetList()
        {
            DataTable dt;
            StringBuilder sb = new StringBuilder();

            sb.Append(" select idx, callfor, callernum, calleenum ");
            sb.Append(" , convert(varchar(10), CALL_SDATE, 120) as syymmdd ");
            sb.Append(" , convert(varchar(10), CALL_EDATE, 120) as eyymmdd ");
            sb.Append(" , convert(varchar(8), CALL_SDATE, 114) as shhmmss ");
            sb.Append(" , convert(varchar(8), CALL_EDATE, 114) as ehhmmss ");
            sb.Append(" , convert(varchar(10), REGDATE, 120) as regyymmdd ");
            sb.Append(" , convert(varchar(8), REGDATE, 114) as reghhmmss ");
            sb.Append(" , call_time ");
            sb.Append(" , case ");
            sb.Append(" when call_status = 0 then '대기중' ");
            sb.Append(" when call_status = 1 then '부재중' ");
            sb.Append(" when call_status = 2 then '통화중' ");
            sb.Append(" when call_status = 3 then '통화완료' ");
            sb.Append(" end callstatus ");
            sb.Append(" from IPCC_CALL_LIST ");
            sb.Append(" where year(regdate)=year(getdate()) ");
            sb.Append(" and month(regdate)=month(getdate()) ");
            sb.Append(" and day(regdate)=day(getdate()) ");
            sb.Append(" order by regdate desc; ");

            using (MSDBHelper db = new MSDBHelper(strConn))
            {
                try
                {
                    db.strSql = sb.ToString();
                    dt = db.GetDataTable();
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
            }

            return dt;
        }

        public DataTable GetList(string sdate, string edate, bool? isallList)
        {
            DataTable dt;

            dt = util.CreateDT2SP();
            dt.Rows.Add("@TOKEN", isallList);
            dt.Rows.Add("@SDATE", sdate);
            dt.Rows.Add("@EDATE", edate);

            using (MSDBHelper db = new MSDBHelper(strConn))
            {
                try
                {
                    dt = db.GetDataTableSP("IPCC_GET_CALL_LIST", dt);
                }
                catch(System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
            }

            return dt;
        }

        public bool UpdateList(string idx, CallListData data)
        {
            bool rtnBool = false;



            return rtnBool;
        }

        public bool DeleteList(string curcallidx)
        {
            bool rtnBool = false;
            StringBuilder sb = new StringBuilder();

            sb.Append(" delete from ipcc_call_list ");
            sb.AppendFormat(" where idx={0} ", curcallidx);

            int chk = 0;
            using (MSDBHelper db = new MSDBHelper(strConn))
            {
                try
                {
                    db.strSql = sb.ToString();

                    db.BeginTran();
                    chk = db.GetEffectedCount();

                    db.Commit();
                    rtnBool = true;
                }
                catch(System.Data.SqlClient.SqlException e)
                {
                    db.Rollback();
                    rtnBool = false;
                }
            }

            return rtnBool;
        }
    }

}
