using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;

using Com.Huen.Sql;
using Com.Huen.Libs;

namespace Com.Huen.Controldata
{
    public class Statistics
    {
        private string _strconn = string.Empty;
        public string strConn
        {
            get
            {
                if (string.IsNullOrEmpty(_strconn))
                {
                    _strconn = (string)util.LoadProjectResource("DBCONSTR_MSSQL", "COMMONRES", "");
                }
                return _strconn;
            }
        }

        public Statistics() { }

        public string GetCurYear
        {
            get
            {
                return DateTime.Now.Year.ToString();
            }
        }

        public string GetCurMonth
        {
            get
            {
                string month = DateTime.Now.Month.ToString();
                if (month.Length == 1)
                    month = string.Format("{0}{1}", "0", month);
                
                return month;
            }
        }

        public string GetCurDay
        {
            get
            {
                string day = DateTime.Now.Day.ToString();
                if (day.Length == 1)
                    day = string.Format("{0}{1}", "0", day);

                return day;
            }
        }

        public DataTable GetJobYear
        {
            get
            {
                DataTable dt;

                using (MSDBHelper db = new MSDBHelper(strConn))
                {
                    try
                    {
                        db.CmdType = CommandType.Text;
                        db.strSql = " select year(regdate) as yyyy, ltrim(str(year(regdate))+'년') as yy from ipcc_call_list group by year(regdate) order by year(regdate) desc ";
                        dt = db.GetDataTable();
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                }

                return dt;
            }
        }

        public DataTable GetJobMonth
        {
            get
            {
                DataTable dt = new DataTable();

                dt.Columns.Add("txtMonth", typeof(string));
                dt.Columns.Add("valMonth", typeof(string));

                for (int i = 0; i < 12; i++)
                {
                    string month = (i+1).ToString();

                    if (month.Length == 1)
                        month = string.Format("{0}{1}", "0", month);

                    dt.Rows.Add(month+"월", month);
                }

                return dt;
            }
        }

        public static DataTable GetJobDay
        {
            get
            {
                DateTime date = DateTime.Now;
                date = date.AddMonths(1);
                date = date.AddDays(-date.Day);
                int monthdays = date.Day;

                DataTable dt = new DataTable();
                dt.Columns.Add("txtDay", typeof(string));
                dt.Columns.Add("valDay", typeof(string));

                for (int i = 1; i <= monthdays; i++)
                {
                    string day = i.ToString();

                    if (day.Length == 1)
                        day = string.Format("{0}{1}", "0", day);

                    dt.Rows.Add(day + "일", day);
                }

                return dt;
            }
        }

        public static DataTable GetJobDays(int yy, int mm)
        {
            DateTime date = new DateTime(yy, mm, DateTime.Now.Day);
            date = date.AddMonths(1);
            date = date.AddDays(-date.Day);
            int monthdays = date.Day;

            DataTable dt = new DataTable();
            dt.Columns.Add("txtDay", typeof(string));
            dt.Columns.Add("valDay", typeof(string));

            for (int i = 1; i <= monthdays; i++)
            {
                string day = i.ToString();

                if (day.Length == 1)
                    day = string.Format("{0}{1}", "0", day);

                dt.Rows.Add(day + "일", day);
            }

            return dt;
        }

        public DataTable GetTelerNum
        {
            get
            {
                DataTable dt;

                using (MSDBHelper db = new MSDBHelper(strConn))
                {
                    try
                    {
                        db.CmdType = CommandType.Text;
                        db.strSql = " select calleenum from ipcc_call_list group by calleenum ";
                        dt = db.GetDataTable();
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                }

                return dt;
            }
        }

        public DataTable GetCallStaticByDay(int yy, int mm, int dd, int gubun)
        {
            DataTable data;

            StringBuilder sb = new StringBuilder();

            if (gubun == 0 || gubun == 1)
            {
                sb.Append(" select isnull(count(idx), 0) callcount ");
                sb.Append(" , isnull(sum(call_time), 0) calltimesec ");
                sb.Append(" , convert(float,  ");
                sb.Append(" convert(varchar(10), isnull(sum(call_time), 0)/60) ");
                sb.Append(" + '.' ");
                sb.Append(" + case ");
                sb.Append(" when len(isnull(sum(call_time), 0)%60)=1 then '0'+convert(char(1),isnull(sum(call_time), 0)%60) ");
                sb.Append(" else convert(char(2),isnull(sum(call_time), 0)%60) ");
                sb.Append(" end ");
                sb.Append(" ) calltimemin ");
                sb.Append(" , b.telnum ");
                sb.Append(" , b.tellername ");
                sb.Append(" from ipcc_call_list a right join ipcc_tbl_teller b ");
                sb.Append(" on a.calleenum=b.telnum ");
                sb.AppendFormat(" and year(regdate)={0} ", yy);
                sb.AppendFormat(" and month(regdate)={0} ", mm);
                sb.AppendFormat(" and day(regdate)={0} ", dd);
                sb.Append(" group by b.telnum, b.tellername ");
                sb.Append(" order by b.telnum desc ");
            }
            else if (gubun == 2)
            {
                sb.Append(" select isnull(count(idx), 0) callcount ");
                sb.Append(" , isnull(sum(call_time), 0) calltimesec ");
                sb.Append(" , convert(float,  ");
                sb.Append(" convert(varchar(10), isnull(sum(call_time), 0)/60) ");
                sb.Append(" + '.' ");
                sb.Append(" + case ");
                sb.Append(" when len(isnull(sum(call_time), 0)%60)=1 then '0'+convert(char(1),isnull(sum(call_time), 0)%60) ");
                sb.Append(" else convert(char(2),isnull(sum(call_time), 0)%60) ");
                sb.Append(" end ");
                sb.Append(" ) calltimemin ");
                sb.Append(" , b.telnum ");
                sb.Append(" , b.tellername ");
                sb.Append(" from ipcc_call_list a right join ipcc_tbl_teller b ");
                sb.Append(" on a.calleenum=b.telnum ");
                sb.AppendFormat(" and datepart(yy, a.regdate)={0} ", yy);
                sb.AppendFormat(" and datepart(mm, a.regdate)={0} ", mm);
                sb.Append(" group by b.telnum, b.tellername ");
                sb.Append(" order by b.telnum desc ");
            }

            using (MSDBHelper db = new MSDBHelper(strConn))
            {
                try
                {
                    db.CmdType = CommandType.Text;
                    db.strSql = sb.ToString();
                    data = db.GetDataTable();
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
            }

            return data;
        }

        public DataTable GetCallStaticByTime(int yy, int mm, int dd, int gubun)
        {
            DataTable data;

            StringBuilder sb = new StringBuilder();

            if (gubun == 0 || gubun == 1)
            {
                sb.Append(" select isnull(count(idx), 0) callcount ");
                sb.Append(" , isnull(sum(call_time), 0) calltimesec ");
                sb.Append(" , convert(float,  ");
                sb.Append(" convert(varchar(10), isnull(sum(call_time), 0)/60) ");
                sb.Append(" + '.' ");
                sb.Append(" + case ");
                sb.Append(" when len(isnull(sum(call_time), 0)%60)=1 then '0'+convert(char(1),isnull(sum(call_time), 0)%60) ");
                sb.Append(" else convert(char(2),isnull(sum(call_time), 0)%60) ");
                sb.Append(" end ");
                sb.Append(" ) calltimemin ");
                sb.Append(" , b.wtime worktime ");
                sb.Append(" , ltrim(str(b.wtime)) + '시' worktimetxt ");
                sb.Append(" from ipcc_call_list a right join ipcc_tbl_worktime b ");
                sb.Append(" on datepart(hh, a.regdate)=b.wtime ");
                sb.AppendFormat(" and datepart(yy, a.regdate)={0} ", yy);
                sb.AppendFormat(" and datepart(mm, a.regdate)={0} ", mm);
                sb.AppendFormat(" and datepart(dd, a.regdate)={0} ", dd);
                sb.AppendFormat(" group by b.wtime ");
                sb.AppendFormat(" order by b.wtime desc; ");
            }
            else if (gubun == 2)
            {
                sb.Append(" select isnull(count(idx), 0) callcount ");
                sb.Append(" , isnull(sum(call_time), 0) calltimesec ");
                sb.Append(" , convert(float,  ");
                sb.Append(" convert(varchar(10), isnull(sum(call_time), 0)/60) ");
                sb.Append(" + '.' ");
                sb.Append(" + case ");
                sb.Append(" when len(isnull(sum(call_time), 0)%60)=1 then '0'+convert(char(1),isnull(sum(call_time), 0)%60) ");
                sb.Append(" else convert(char(2),isnull(sum(call_time), 0)%60) ");
                sb.Append(" end ");
                sb.Append(" ) calltimemin ");
                sb.Append(" , b.wtime worktime ");
                sb.Append(" , ltrim(str(b.wtime)) + '시' worktimetxt ");
                sb.Append(" from ipcc_call_list a right join ipcc_tbl_worktime b ");
                sb.Append(" on datepart(hh, a.regdate)=b.wtime ");
                sb.AppendFormat(" and datepart(yy, a.regdate)={0} ", yy);
                sb.AppendFormat(" and datepart(mm, a.regdate)={0} ", mm);
                //sb.AppendFormat(" --and datepart(dd, a.regdate)={0} ", dd);
                sb.AppendFormat(" group by b.wtime ");
                sb.AppendFormat(" order by b.wtime desc; ");
            }

            using (MSDBHelper db = new MSDBHelper(strConn))
            {
                try
                {
                    db.CmdType = CommandType.Text;
                    db.strSql = sb.ToString();
                    data = db.GetDataTable();
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
            }

            return data;
        }
    }
}
