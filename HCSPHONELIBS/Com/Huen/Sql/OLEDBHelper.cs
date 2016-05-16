using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.OleDb;

using Com.Huen.Libs;

namespace Com.Huen.Sql
{
    public class OLEDBHelper : IDisposable
    {
        OleDbConnection conn = null;
        OleDbCommand comm = null;
        DataSet ds = null;
        DataTable dt = null;
        //DataRow dr = null;
        OleDbDataAdapter adapter = null;
        object objResult = null;
        int iResult = 0;

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

        public OLEDBHelper() { }

        public OLEDBHelper(string str, string strconn)
        {
            strSql = str;
            strConn = strconn;

            conn = new OleDbConnection(strConn);
            conn.Open();

            comm = new OleDbCommand();
            comm.CommandType = CommandType.Text;
            comm.CommandText = strSql;
            comm.Connection = conn;
        }

        public DataSet GetResultDataSet()
        {
            ds = new DataSet();
            try
            {
                adapter = new OleDbDataAdapter(comm);
                adapter.Fill(ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        public DataTable GetResultDataTable()
        {
            dt = new DataTable();
            try
            {
                adapter = new OleDbDataAdapter(comm);
                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        public object GetResultOneData()
        {
            try
            {
                //conn.Open();
                objResult = comm.ExecuteScalar();
                //conn.Close();
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
                //conn.Open();
                iResult = comm.ExecuteNonQuery();
                //conn.Close();
            }
            catch (OleDbException ex)
            {
                throw ex;
            }

            return iResult;
        }

        public void BeginTran()
        {
            comm.Transaction = conn.BeginTransaction();
        }

        public void Commit()
        {
            comm.Transaction.Commit();
        }

        public void Rollback()
        {
            comm.Transaction.Rollback();
        }

        public void Dispose()
        {
            if (conn != null)
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            conn = null;
            comm = null;
            ds = null;
            adapter = null;
            objResult = null;
            iResult = 0;
        }

        public void SetQuery(string query)
        {
            comm.CommandText = query;
        }

        public bool CreateDatabase(string fullFilename)
        {
            bool succeeded = false;
            try
            {
                string newDB = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", fullFilename);
                Type objClassType = Type.GetTypeFromProgID("ADOX.Catalog");
                if (objClassType != null)
                {
                    object obj = Activator.CreateInstance(objClassType);
                    // Create MDB file 
                    obj.GetType().InvokeMember(
                                "Create", System.Reflection.BindingFlags.InvokeMethod
                                , null, obj
                                , new object[] { string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}", newDB) }
                                );
                    succeeded = true;
                    // Clean up
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Could not create database file: " + fullFilename + "\n\n" + ex.Message, "Database Creation Error");
                throw ex;
            }
            return succeeded;
        }

    }
}
