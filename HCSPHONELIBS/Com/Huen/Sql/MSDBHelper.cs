using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.SqlClient;

using Com.Huen.Libs;

namespace Com.Huen.Sql
{
    public class MSDBHelper : IDisposable
    {
        SqlConnection conn = null;
        SqlCommand cmd = null;
        SqlDataAdapter adapter = null;

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
                //cmd.CommandType = _commandType;
            }
        }
        #endregion 속성

        public MSDBHelper() { }

        public MSDBHelper(string strconn)
        {
            this.strConn = strconn;

            conn = new SqlConnection(strConn);
            conn.Open();

            cmd = new SqlCommand();
            cmd.Connection = conn;
        }

        public MSDBHelper(string __sql, string strconn)
        {
            strSql = __sql;
            strConn = strconn;

            conn = new SqlConnection(strConn);
            conn.Open();

            cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = strSql;
            cmd.Connection = conn;
        }

        public MSDBHelper(string str, string strconn, CommandType cmdtype)
        {
            strSql = str;
            strConn = strconn;
            CmdType = cmdtype;

            conn = new SqlConnection(strConn);
            conn.Open();

            cmd = new SqlCommand();
            cmd.CommandType = CmdType;
            cmd.CommandText = strSql;
            cmd.Connection = conn;
        }

        public DataSet GetDataSet()
        {
            ds = new DataSet();
            try
            {
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        public DataTable GetDataTable()
        {
            dt = new DataTable();
            try
            {
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        public object GetData()
        {
            try
            {
                objResult = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return objResult;
        }

        public int GetEffectedCount()
        {
            try
            {
                iResult = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }

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
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procname;

            foreach (DataRow dr in variables.Rows)
            {
                SqlParameter sqlParam = new SqlParameter(dr[0].ToString(), dr[1].ToString());
                cmd.Parameters.Add(sqlParam);
            }

            cmd.ExecuteNonQuery();
        }

        // 프로시저 실행 결과 select 리턴
        public DataTable GetDataTableSP(string procname, DataTable variables)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = procname;

            foreach (DataRow dr in variables.Rows)
            {
                SqlParameter sqlParam = new SqlParameter(dr[0].ToString(), dr[1].ToString());
                cmd.Parameters.Add(sqlParam);
            }

            dt = this.GetDataTable();

            return dt;
        }

        // 프로시저 실행 결과 select 리턴
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

            foreach (DataRow dr in variables.Rows)
            {
                SqlParameter sqlParam = new SqlParameter(dr[0].ToString(), dr[1].ToString());
                cmd.Parameters.Add(sqlParam);
            }

            return this.GetData();
        }

        //// 프로시저 실행을 위한 값 셋팅 테이블
        //protected DataTable MakeDataTable2Proc()
        //{
        //    DataTable dt = new DataTable();

        //    DataColumn paramname = new DataColumn();
        //    paramname.DataType = System.Type.GetType("System.String");
        //    paramname.ColumnName = "DataName";
        //    dt.Columns.Add(paramname);

        //    DataColumn datatype = new DataColumn();
        //    datatype.DataType = System.Type.GetType("System.Object");
        //    datatype.ColumnName = "DataType";
        //    dt.Columns.Add(datatype);

        //    DataColumn datasize = new DataColumn();
        //    datasize.DataType = System.Type.GetType("System.Int32");
        //    datasize.ColumnName = "DataSize";
        //    dt.Columns.Add(datasize);

        //    DataColumn datavalue = new DataColumn();
        //    datavalue.DataType = System.Type.GetType("System.String");
        //    datavalue.ColumnName = "DataValue";
        //    dt.Columns.Add(datavalue);

        //    DataColumn datadirection = new DataColumn();
        //    datadirection.DataType = System.Type.GetType("System.String");
        //    datadirection.ColumnName = "DataDirection";
        //    dt.Columns.Add(datadirection);

        //    return dt;
        //}
    }
}
