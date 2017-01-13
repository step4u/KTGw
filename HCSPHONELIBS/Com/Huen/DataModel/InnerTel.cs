using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;
using System.Data;
using System.ComponentModel;

using System.Windows;

using Com.Huen.Libs;
using Com.Huen.Sql;
using FirebirdSql.Data.FirebirdClient;

namespace Com.Huen.DataModel
{
    [Serializable()]
    public class InnerTel : INotifyPropertyChanged
    {
        private string _tellername = string.Empty;
        private string _peernum = string.Empty;

        public int Seq { get; set; }
        public string Telnum { get; set; }
        public string TellerName
        {
            get
            {
                return _tellername;
            }
            set
            {
                _tellername = value;
                this.OnPropertyChanged("TellerName");
            }
        }
        public string PeerNum
        {
            get
            {
                return _peernum;
            }
            set
            {
                _peernum = value;
                this.OnPropertyChanged("PeerNum");
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class InnerTels
    {
        private ObservableCollection<InnerTel> _list;
        public ObservableCollection<InnerTel> GetList
        {
            get { return _list; }
        }

        public InnerTels()
        {
            DataTable dt = null;

            try
            {
                using (FirebirdDBHelper db = new FirebirdDBHelper(util.strFBDBConn))
                {
                    try
                    {
                        dt = db.GetDataTableSP("GET_INNERTELS");
                    }
                    catch (FirebirdSql.Data.FirebirdClient.FbException fe1)
                    {
                        //throw fe;
                        MessageBox.Show("test");
                    }
                }

                _list = new ObservableCollection<InnerTel>(
                        (from __row in dt.AsEnumerable()
                         select new InnerTel()
                         {
                             Seq = int.Parse(__row[0].ToString())
                             ,
                             Telnum = "   " + __row[1].ToString()
                             ,
                             TellerName = __row[2].ToString()
                         }
                        ).ToList<InnerTel>()
                    );

                InnerTel _tmptel = new InnerTel() { Seq = 0, Telnum = "전체" };
                _list.Insert(0, _tmptel);
            }
            catch (FirebirdSql.Data.FirebirdClient.FbException fe0)
            {
                _list = new ObservableCollection<InnerTel>();
                util.WriteLog(fe0.Message);
                MessageBox.Show("Database 접속에 문제가 발생하였습니다.\r\n \"도구 → 환경설정 → 서버주소\"을 확인 후 다시 실행해 주세요.");
            }

        }

        public void Add(InnerTel _itel)
        {
            using (FirebirdDBHelper db = new FirebirdDBHelper(util.strFBDBConn))
            {
                db.SetParameters("@telnum", FbDbType.VarChar, _itel.Telnum);
                db.SetParameters("@tellername", FbDbType.VarChar, _itel.TellerName);

                try
                {
                    db.BeginTran();
                    //db.ExcuteSP("INS_INNERTELS", dt);
                    db.ExcuteSP("INS_INNERTELS");
                    db.Commit();

                    _list.Add(_itel);
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException fe)
                {
                    db.Rollback();
                }
            }
        }

        public void Modify(InnerTel _itel)
        {
            using (FirebirdDBHelper db = new FirebirdDBHelper(util.strFBDBConn))
            {
                db.SetParameters("@seq", FbDbType.Integer, _itel.Seq);
                db.SetParameters("@telnum", FbDbType.VarChar, _itel.Telnum);
                db.SetParameters("@tellername", FbDbType.VarChar, _itel.TellerName);

                try
                {
                    db.BeginTran();
                    //db.ExcuteSP("UDT_INNERTELS", dt);
                    db.ExcuteSP("UDT_INNERTELS");
                    db.Commit();

                    InnerTel __obj = _list.FirstOrDefault(x => x.Telnum == _itel.Telnum);
                    __obj.TellerName = _itel.TellerName;
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException fe)
                {
                    db.Rollback();
                }
            }
        }

        public void Remove(InnerTel _itel)
        {
            using (FirebirdDBHelper db = new FirebirdDBHelper(util.strFBDBConn))
            {
                db.SetParameters("@seq", FbDbType.Integer, _itel.Seq);

                try
                {
                    db.BeginTran();
                    db.ExcuteSP("RMV_INNERTELS");
                    db.Commit();

                    InnerTel __obj = _list.FirstOrDefault(x => x.Seq == _itel.Seq);
                    _list.Remove(__obj);
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException fe)
                {
                    db.Rollback();
                    MessageBox.Show(util.GetErrorMessage(fe));
                }
            }
        }
    }
}
