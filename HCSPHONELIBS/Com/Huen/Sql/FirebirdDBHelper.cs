using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using FirebirdSql.Data.FirebirdClient;
using Com.Huen.Libs;

namespace Com.Huen.Sql
{
    public class FirebirdDBHelper : IDisposable
    {
        FbConnection conn = null;
        FbCommand cmd = null;
        FbDataAdapter adapter = null;

        DataSet ds = null;
        DataTable dt = null;

        object objResult = null;
        int iResult = 0;

        #region 속성
        private string _strSql = string.Empty;
        public string strSql
        {
            get
            {
                return _strSql;
            }
            set
            {
                _strSql = value;
                //cmd.CommandText = _strSql;
            }
        }

        private string _strConn = string.Empty;
        public string strConn
        {
            get
            {
                return _strConn;
            }
            set
            {
                _strConn = value;
            }
        }

        private CommandType _commandType = CommandType.Text;
        public CommandType CmdType
        {
            get
            {
                return _commandType;
            }
            set
            {
                _commandType = value;
                cmd.CommandType = _commandType;
            }
        }
        #endregion 속성

        public FirebirdDBHelper() { }

        public FirebirdDBHelper(string strconn)
        {
            this.strConn = strconn;

            conn = new FbConnection(strConn);
            conn.Open();

            cmd = new FbCommand();
            cmd.Connection = conn;
        }

        public FirebirdDBHelper(string str, string strconn)
        {
            strSql = str;
            strConn = strconn;

            conn = new FbConnection(strConn);
            conn.Open();

            cmd = new FbCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = strSql;
            cmd.Connection = conn;
        }

        public FirebirdDBHelper(string str, string strconn, CommandType cmdtype)
        {
            strSql = str;
            strConn = strconn;
            CmdType = cmdtype;

            conn = new FbConnection(strConn);
            conn.Open();

            cmd = new FbCommand();
            cmd.CommandType = CmdType;
            cmd.CommandText = strSql;
            cmd.Connection = conn;
        }

        public void SetParameters(string column, FbDbType fbdbtype, object value)
        {
            cmd.Parameters.Add(column, fbdbtype).Value = value;
        }

        public DataSet GetDataSet()
        {
            ds = new DataSet();
            adapter = new FbDataAdapter(cmd);
            adapter.Fill(ds);
            return ds;
        }

        public DataTable GetDataTable()
        {
            dt = new DataTable();
            adapter = new FbDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        public object GetData()
        {
            objResult = cmd.ExecuteScalar();
            return objResult;
        }

        public int GetEffectedCount()
        {
            iResult = cmd.ExecuteNonQuery();
            return iResult;
        }

        public void BeginTran()
        {
            cmd.Transaction = conn.BeginTransaction();
        }

        public void Commit()
        {
            cmd.Transaction.Commit();
        }

        public void Rollback()
        {
            cmd.Transaction.Rollback();
        }

        public void Dispose()
        {
            if (conn != null)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            conn = null;
            cmd = null;
            ds = null;
            adapter = null;
            objResult = null;
            iResult = 0;
        }

        // 프로시저 실행 결과 리턴 (0:실패, 1:성공), 트랜잭션은 프로시저에서 처리
        //protected int ExcuteSP(string procname, DataTable variables)
        //{
        //    foreach (DataRow dr in variables.Rows)
        //    {
        //        SqlParameter sqlParam = new SqlParameter(dr[0].ToString(), dr[3].ToString());
        //        if (dr[4].ToString() == "ReturnValue")
        //        {
        //            sqlParam.Direction = ParameterDirection.ReturnValue;
        //        }
        //        cmd.Parameters.Add(sqlParam);
        //    }

        //    cmd.ExecuteNonQuery();

        //    int chk = (int)cmd.Parameters["@CHK"].Value;

        //    return chk;
        //}

        public void ExcuteSP(string procname, DataTable variables)
        {
            cmd.CommandText = procname;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            foreach (DataRow dr in variables.Rows)
            {
                FbParameter sqlParam = new FbParameter(dr[0].ToString(), dr[1].ToString());
                cmd.Parameters.Add(sqlParam);
            }

            cmd.ExecuteNonQuery();
        }

        public void ExcuteSP(string procname)
        {
            cmd.CommandText = procname;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
        }

        // 프로시저 실행 결과 리턴, 매개변수 Datatable
        public DataTable GetDataTableSP(string procname, DataTable variables)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procname;
            cmd.Parameters.Clear();

            foreach (DataRow dr in variables.Rows)
            {
                FbParameter sqlParam = new FbParameter(dr[0].ToString(), dr[1].ToString());
                cmd.Parameters.Add(sqlParam);
            }

            dt = this.GetDataTable();

            return dt;
        }

        // 프로시저 실행 결과 리턴, 매개변수 없음
        public DataTable GetDataTableSP(string procname)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procname;

            dt = this.GetDataTable();
            return dt;
        }

        // 프로시저 실행 결과 select 리턴
        public Object GetDataSP(string procname)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procname;

            return this.GetData();
        }

        // 프로시저 실행 결과 select 리턴
        public Object GetDataSP(string procname, DataTable variables)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procname;
            cmd.Parameters.Clear();

            foreach (DataRow dr in variables.Rows)
            {
                FbParameter sqlParam = new FbParameter(dr[0].ToString(), dr[1].ToString());
                cmd.Parameters.Add(sqlParam);
            }

            return this.GetData();
        }
    }
}
