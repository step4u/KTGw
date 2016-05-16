using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Data;
using NAudio.Wave;

using Com.Huen.DataModel;
using Com.Huen.Libs;
using Com.Huen.Sockets;
using Com.Huen.Sql;

namespace Com.Huen.Views
{
    /// <summary>
    /// Interaction logic for SearchNintercept.xaml
    /// </summary>
    public partial class SearchNintercept : Window
    {
        public SearchNintercept()
        {
            InitializeComponent();

            this.InitilaizeWindow();

            this.Loaded += SearchNintercept_Loaded;
            this.Closed += SearchNintercept_Closed;
        }

        // private bool _isregistered = false;
        // private int _shareremain = 30;
        public CRInterceptOption _option;

        //private ObservableCollection<Company> _companies;
        private ObservableCollection<InnerTel> _innertels;
        private ObservableCollection<Interview> _interviewes;

        //private CollectionViewSource _companiesViewSrc;
        private CollectionViewSource _innertelViewSrc;
        private CollectionViewSource _interviewsViewSrc;

        private int _comTotalPage = 0;
        // private int _comItemPerPage = 25;
        private int _comCurrentPageIdx = 0;

        private int _inerviewTotalPage = 0;
        private int _inerviewItemPerPage = 200;
        private int _inerviewCurrentPageIdx = 0;

        void SearchNintercept_Loaded(object sender, RoutedEventArgs e)
        {
            //this.InitializeCompanyLV();
            this.InitializeInnertelLV();
            //this.InitailizeInnerTelSatusLV();
            
            //this.StartCmdSocket();
            //this.StartRtpSocket();

            //if (_IsCmdSocketStarted)
            //    ThreadPool.QueueUserWorkItem(new WaitCallback(SendInnertelStatusReq));
        }

        void SearchNintercept_Closed(object sender, EventArgs e)
        {
            this.StopFileClient();
            this.SaveIni();
        }

        public Ini ReadIni()
        {
            Ini ini = new Ini(string.Format(@"{0}\{1}\{2}.ini", Options.usersdatapath, Options.appname));
            return ini;
        }

        public void SaveIni()
        {
            Ini ini = new Ini(string.Format(@"{0}\{1}\{2}", Options.usersdatapath, Options.appname));

            ini.IniWriteValue("POSITION", "top", this.Top.ToString());
            ini.IniWriteValue("POSITION", "left", this.Left.ToString());

            ini.IniWriteValue("SIZE", "width", this.Width.ToString());
            ini.IniWriteValue("SIZE", "height", this.Height.ToString());
            ini.IniWriteValue("SIZE", "col0", grid0.ColumnDefinitions[0].Width.Value.ToString());
            //_ini.IniWriteValue("SIZE", "col1", grid0.ColumnDefinitions[1].Width.Value.ToString());

            ini.IniWriteValue("SERVER", "rec", Options.recserverip);
            ini.IniWriteValue("SERVER", "db", Options.dbserverip);
            ini.IniWriteValue("SERVER", "pbx", Options.pbxip);
            ini.IniWriteValue("ETC", "savedir", Options.savedir);
        }

        private void InitilaizeWindow()
        {
            Ini ini = ReadIni();
            this.Top = string.IsNullOrEmpty(ini.IniReadValue("POSITION", "top")) == true ? 10.0d : double.Parse(ini.IniReadValue("POSITION", "top"));
            this.Left = string.IsNullOrEmpty(ini.IniReadValue("POSITION", "left")) == true ? 10.0d : double.Parse(ini.IniReadValue("POSITION", "left"));

            this.Width = string.IsNullOrEmpty(ini.IniReadValue("SIZE", "width")) == true ? 800.0d : double.Parse(ini.IniReadValue("SIZE", "width"));
            this.Height = string.IsNullOrEmpty(ini.IniReadValue("SIZE", "height")) == true ? 600.0d : double.Parse(ini.IniReadValue("SIZE", "height"));
            this.grid0.ColumnDefinitions[0].Width = new GridLength(string.IsNullOrEmpty(ini.IniReadValue("SIZE", "col0")) == true ? 250.0d : double.Parse(ini.IniReadValue("SIZE", "col0")));
            //this.grid0.ColumnDefinitions[1].Width = new GridLength(string.IsNullOrEmpty(_ini.IniReadValue("SIZE", "col1")) == true ? 250.0d : double.Parse(ini.IniReadValue("SIZE", "col1")));

            Options.recserverip = string.IsNullOrEmpty(ini.IniReadValue("SERVER", "rec")) == true ? "127.0.0.1" : ini.IniReadValue("SERVER", "rec");
            Options.dbserverip = string.IsNullOrEmpty(ini.IniReadValue("SERVER", "db")) == true ? "127.0.0.1" : ini.IniReadValue("SERVER", "db");
            Options.pbxip = string.IsNullOrEmpty(ini.IniReadValue("SERVER", "pbx")) == true ? "127.0.0.1" : ini.IniReadValue("SERVER", "pbx");
            Options.savedir = string.IsNullOrEmpty(ini.IniReadValue("ETC", "savedir")) == false ? ini.IniReadValue("ETC", "savedir") : string.Format(@"{0}\{1}", Options.usersdatapath, "RecDown");
#if false
            if (!_isregistered)
            {
                string __fn = ".\\resrces.dll";
                FileStream __fs = null;
                byte[] __buff = new byte[1024];
                CheckShareWare __chk = null;

                if (File.Exists(__fn))
                {
                    try
                    {
                        __fs = File.Open(__fn, FileMode.Open, FileAccess.ReadWrite);
                        __fs.Read(__buff, 0, (int)__fs.Length);
                        __fs.Close();

                        __chk = (CheckShareWare)util.ByteArrayToObject(__buff);

                        DateTime __now = DateTime.Now;
                        DateTime __tmpDate = new DateTime(1, 1, 1, 0, 0, 0, 0);

                        if (__chk.installDate.Equals(__tmpDate))
                        {
                            util.WriteLog2("first run");

                            __chk.installDate = __now;
                            __chk.currentDate = __now;

                            // write file
                            __buff = util.ObjectToByteArray(__chk);
                            File.WriteAllBytes(__fn, __buff);
                        }
                        else
                        {
                            _shareremain = (int)util.DateDiff("dd", __now, __chk.currentDate);
                            if (_shareremain > 30)
                            {
                                MessageBox.Show("This shareware date is expired.");
                                this.Close();
                            }
                            else if (_shareremain <= 30 && _shareremain > 27)
                            {
                                if (MessageBox.Show(string.Format("This shareware date remain {0}days.\r\nWill you regist this software?", 30 - _shareremain), "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                {
                                    // show register windows open
                                }

                                // currentdate update and write file
                                __chk.currentDate = __now;

                                __buff = util.ObjectToByteArray(__chk);
                                File.WriteAllBytes(__fn, __buff);
                            }
                            else
                            {
                                // currentdate update and write file
                                __chk.currentDate = __now;

                                __buff = util.ObjectToByteArray(__chk);
                                File.WriteAllBytes(__fn, __buff);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        util.WriteLog2(e.Message);
                    }
                }
                else
                {
                    /*
                    DateTime _now = DateTime.Now;
                    byte[] _buff;

                    //CheckShareWare __chkshare = new CheckShareWare() { currentDate = _now, installDate = _now };
                    CheckShareWare __chkshare = new CheckShareWare();
                    _buff = util.ObjectToByteArray(__chkshare);

                    __fs = File.OpenWrite(__fn);
                    __fs.Write(_buff, 0, _buff.Length);
                    __fs.Close();
                    */
                    this.Close();
                }
            }
#endif
        }

#if false // InitializeCompanyLV
        private void InitializeCompanyLV()
        {
            Companies __coms = new Companies();
            _companies = __coms.GetList;
            _companiesViewSrc = new CollectionViewSource();
            _companiesViewSrc.Filter += _companiesViewSrc_Filter;
            _companiesViewSrc.Source = _companies;

            // Calculate the total pages
            _comTotalPage = _companies.Count / _comItemPerPage;
            if (_companies.Count % _comItemPerPage != 0)
            {
                _comTotalPage += 1;
            }

            this.SetComLVCurPage();
        }

        void _companiesViewSrc_Filter(object sender, FilterEventArgs e)
        {
            Company __obj = (Company)e.Item;
            int __idx = _companies.IndexOf(__obj);

            if (__idx >= _comItemPerPage * _comCurrentPageIdx && __idx < _comItemPerPage * (_comCurrentPageIdx + 1))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }

            string __txtSearch = txtSearch0.Text.Trim();

            if (!string.IsNullOrEmpty(__txtSearch))
            {
                if (__obj.comname.Contains(__txtSearch) || __obj.comhg.Contains(__txtSearch))
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
        }

        private void SetComLVCurPage()
        {
            txtT0.Text = _comTotalPage.ToString();
            txtF0.Text = (_comCurrentPageIdx + 1).ToString();
        }
#endif

#if true // InitializeInnertelLV
        private int _innertelTotalPage = 0;
        private int _innertelItemPerPage = 50;
        private int _innertelCurrentPageIdx = 0;

        private void InitializeInnertelLV()
        {
            __innertels = new InnerTels();
            _innertels = __innertels.GetList;
            _innertelViewSrc = new CollectionViewSource();
            _innertelViewSrc.Filter += _innertelViewSrc_Filter;
            _innertelViewSrc.Source = _innertels;

            lvInnertels.DataContext = _innertelViewSrc;
            lvInnertels.SelectedIndex = 0;

            // Calculate the total pages
            _innertelTotalPage = _innertels.Count / _innertelItemPerPage;
            if (_innertels.Count % _innertelItemPerPage != 0)
            {
                _innertelTotalPage += 1;
            }

            this.SetinnertelLVCurPage();
        }

        void _innertelViewSrc_Filter(object sender, FilterEventArgs e)
        {
            InnerTel __obj = (InnerTel)e.Item;
            var _tmpcollection = _innertels.ToList<InnerTel>();
            int __idx = _tmpcollection.FindIndex(x => x.Telnum == __obj.Telnum);

            //int __idx = _innertels.IndexOf(__obj);

            if (__idx >= _innertelItemPerPage * _innertelCurrentPageIdx && __idx < _innertelItemPerPage * (_innertelCurrentPageIdx + 1))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }

            string __txtSearch = txtSearch0.Text.Trim();

            if (!string.IsNullOrEmpty(__txtSearch))
            {
                if (__obj.Telnum.Contains(__txtSearch))
                {
                    e.Accepted = true;
                }
                else
                {
                    e.Accepted = false;
                }
            }
        }

        private void SetinnertelLVCurPage()
        {
            txtT0.Text = _innertelTotalPage.ToString();
            txtF0.Text = (_innertelCurrentPageIdx + 1).ToString();
        }
#endif

        private void InitializeInterviewLV(int __seq)
        {
            string _format = "HH:mm:ss";
            SearchConditions cons = new SearchConditions() {
                idx = __seq,
                sdate = DateTime.Parse(string.Format("{0} {1}", txtFromYY.Text, ((DateTime)txtFromTT.Value).ToString(_format))),
                edate = DateTime.Parse(string.Format("{0} {1}", txtToYY.Text, ((DateTime)txtToTT.Value).ToString(_format)))
        };

            Interviewes __interviewlists = new Interviewes(cons);
            _interviewes = __interviewlists.GetList;
            _interviewsViewSrc = new CollectionViewSource();
            _interviewsViewSrc.Filter += _interviewsViewSrc_Filter;
            _interviewsViewSrc.Source = _interviewes;

            lvInterview.DataContext = _interviewsViewSrc;
            lvInterview.SelectedIndex = 0;

            // Calculate the total pages
            _inerviewTotalPage = _interviewes.Count / _inerviewItemPerPage;
            if (_interviewes.Count % _inerviewItemPerPage != 0)
            {
                _inerviewTotalPage += 1;
            }

            this.SetInterviewLVCurPage();
        }

        private bool __search1 = false;
        void _interviewsViewSrc_Filter(object sender, FilterEventArgs e)
        {
            Interview __obj = (Interview)e.Item;
            int __idx = _interviewes.IndexOf(__obj);

            if (__idx >= _inerviewItemPerPage * _inerviewCurrentPageIdx && __idx < _inerviewItemPerPage * (_inerviewCurrentPageIdx + 1))
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = false;
            }

            if (__search1)
            {
                string __format = "HH:mm:ss";
                DateTime __from = DateTime.Parse(string.Format("{0} {1}", txtFromYY.Text, ((DateTime)txtFromTT.Value).ToString(__format)));
                DateTime __to = DateTime.Parse(string.Format("{0} {1}", txtToYY.Text, ((DateTime)txtToTT.Value).ToString(__format)));

                if (string.IsNullOrEmpty(txt_peernum.Text.Trim()))
                {
                    if (__obj.regdate >= __from && __obj.regdate <= __to)
                    {
                        e.Accepted = true;
                    }
                    else
                    {
                        e.Accepted = false;
                    }
                }
                else
                {
                    if ((__obj.regdate >= __from && __obj.regdate <= __to) && (__obj.extention.Contains(txt_peernum.Text.Trim()) || __obj.peernum.Contains(txt_peernum.Text.Trim())))
                    {
                        e.Accepted = true;
                    }
                    else
                    {
                        e.Accepted = false;
                    }
                }
            }
        }

        private void SetInterviewLVCurPage()
        {
            txtT1.Text = _inerviewTotalPage.ToString();
            txtF1.Text = (_inerviewCurrentPageIdx + 1).ToString();
        }

        private InnerTels __innertels;
        private void InitailizeInnerTelSatusLV()
        {
            __innertels = new InnerTels();
            _innertels = __innertels.GetList;
            lvInnertels.ItemsSource = _innertels;
        }

        private void topMenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void topMenuItemListen_Checked(object sender, RoutedEventArgs e)
        {
            MenuItem __item = (MenuItem)e.Source;

            _option.UsingInnertelCol = __item.IsChecked;

            if (_option.UsingInnertelCol)
            {
                if (_option.GridColDef2.Value == 0.0d)
                {
                    grid0.ColumnDefinitions[1].Width = new GridLength(1d, GridUnitType.Star);
                }
                else
                {
                    grid0.ColumnDefinitions[1].Width = _option.GridColDef1;
                }
            }
            else
            {
                grid0.ColumnDefinitions[1].Width = new GridLength(1d, GridUnitType.Star);
            }
        }

        private void topMenuItemProps_Click(object sender, RoutedEventArgs e)
        {
            SearchProperties __pop = new SearchProperties();
            __pop.Owner = this;
            __pop.Show();
        }

        private void btnRefresh0_Click(object sender, RoutedEventArgs e)
        {
            txtSearch0.Text = string.Empty;
            InitializeInnertelLV();

            //_companiesViewSrc.View.Refresh();

            //InitializeCompanyLV();
        }

        private void btnPrev0_Click(object sender, RoutedEventArgs e)
        {
            if (_comCurrentPageIdx > 0)
            {
                _comCurrentPageIdx--;
                //_companiesViewSrc.View.Refresh();
            }
            //this.SetComLVCurPage();
        }

        private void btnNext0_Click(object sender, RoutedEventArgs e)
        {
            if (_comCurrentPageIdx < _comTotalPage - 1)
            {
                _comCurrentPageIdx++;
                //_companiesViewSrc.View.Refresh();
            }
            //this.SetComLVCurPage();
        }

        private void btnRefresh1_Click(object sender, RoutedEventArgs e)
        {
            __search1 = false;
            lvInnertels.SelectedIndex = 0;
            InitializeInterviewLV(__seltel.Seq);
            txt_peernum.Text = string.Empty;
        }

        private void btnSearch0_Click(object sender, RoutedEventArgs e)
        {
            //_interviewsViewSrc.View.Refresh();
            _innertelViewSrc.View.Refresh();
        }

        private void btnSearch1_Click(object sender, RoutedEventArgs e)
        {
            __search1 = true;
            _interviewsViewSrc.View.Refresh();
        }

        private InnerTel __seltel;
        private void lvInnertels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView __lv = (ListView)e.Source;

            if (__lv == null) return;

            __seltel = (InnerTel)__lv.SelectedItem;

            if (__seltel == null) return;

            //__search1 = false;
            __search1 = true;

            this.InitializeInterviewLV(__seltel.Seq);

            __search1 = false;
        }

        private void btnPrev1_Click(object sender, RoutedEventArgs e)
        {
            if (_inerviewCurrentPageIdx > 0)
            {
                _inerviewCurrentPageIdx--;
                _interviewsViewSrc.View.Refresh();
            }

            this.SetInterviewLVCurPage();
            chkheader.IsChecked = false;
        }

        private void btnNext1_Click(object sender, RoutedEventArgs e)
        {
            if (_inerviewCurrentPageIdx < _inerviewTotalPage - 1)
            {
                _inerviewCurrentPageIdx++;
                _interviewsViewSrc.View.Refresh();
            }

            this.SetInterviewLVCurPage();
            chkheader.IsChecked = false;
        }

        #region 소켓 관련 소스 s

        private EndPoint _localep_cmd;
        private EndPoint _serverep_cmd;
        private EndPoint _localep_rtp;
        private EndPoint _serverep_rtp;
        private Thread _threadRtpSocket;
        private Thread _threadCmdSocket;

        private Socket _sockCmd;
        private Socket _sockRtp;
        //private Socket _sockFile;

        private bool _IsRtpSocketStarted = false;
        private bool _IsCmdSocketStarted = false;
        //private bool _IsSocketFileStarted = false;

        //private InterceptorStatus _interceptorStatus = InterceptorStatus.None;
        private InterceptRequest _req;
        private InterceptResponse _res;
        private RecordInfo_t _rtp;

        #region Cmd Socket s
        private void StartCmdSocket()
        {
            _localep_cmd = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                _serverep_cmd = new IPEndPoint(IPAddress.Parse(Options.recserverip), 21021);

                _sockCmd = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _sockCmd.Bind(_localep_cmd);

                //this.SetStatusMessage("서버에 연결되었습니다.");
                ThreadPool.QueueUserWorkItem(new WaitCallback(SetStatusMessage), "서버에 연결되었습니다.");
            }
            catch (SocketException se)
            {
                //throw se;
                util.WriteLog(se.Message);

                Thread.Sleep(5000);
                this.StartCmdSocket();
                
                //this.SetStatusMessage("녹취 서버에 연결하지 못했습니다. 서버를 확인해주세요.");
                ThreadPool.QueueUserWorkItem(new WaitCallback(SetStatusMessage), "녹취 서버에 연결하지 못했습니다. 서버를 확인해주세요.");
            }

            _IsCmdSocketStarted = true;

            _threadCmdSocket = new Thread(new ThreadStart(CmdReceiver));
            _threadCmdSocket.IsBackground = true;
            _threadCmdSocket.Start();
        }

        private void CmdReceiver()
        {
            int __count = 0;

            while (_IsCmdSocketStarted)
            {
                //_res = new InterceptResponse() { cmd = 0, result = 0, innertels = new ObservableCollection<InnerTel>() };

                byte[] __buffer = new byte[12288];
                __count = 0;

                try
                {
                    __count = _sockCmd.ReceiveFrom(__buffer, SocketFlags.None, ref _localep_cmd);
                }
                catch (SocketException se)
                {
                    //throw se;
                    string __msg = string.Format("socket error : {0}", se.ErrorCode);
                    util.WriteLog(__msg);
                }

                if (__count == 0) return;

                byte[] __databuffer = new byte[__count];
                Buffer.BlockCopy(__buffer, 0, __databuffer, 0, __count);

                _res = util.ByteArrayToObject<InterceptResponse>(__databuffer);

                if (_res.cmd == 1)
                {
                    // 내선 상태 요청 > 응답 처리
                    foreach (InnerTel _item in _res.innertels)
                    {
                        InnerTel _t = _innertels.FirstOrDefault(x => x.Telnum.Trim() == _item.Telnum.Trim());
                        if (_t != null)
                        {
                            if (string.IsNullOrEmpty(_item.PeerNum))
                            {
                                _t.PeerNum = _item.PeerNum;
                            }
                            else
                            {
                                _t.PeerNum = _item.PeerNum + "(녹취중)";
                            }
                        }
                    }
                }
                else if (_res.cmd == 2)
                {
                    // 응답이 오면 플레이어 실행
                    if (playbackState == StreamingPlaybackState.Stopped)
                    {
                        playbackState = StreamingPlaybackState.Buffering;
                        this.bufferedWaveProvider = null;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(_playTimer_Tick));
                    }
                    else if (playbackState == StreamingPlaybackState.Paused)
                    {
                        playbackState = StreamingPlaybackState.Buffering;
                    }

                    //ThreadPool.QueueUserWorkItem(new WaitCallback(this.SendRtpReq), __requestTelNumber);
                    this.SendRtpReq(__requestTelNumber);
                    this.SetStatusMessage("실시간 청취 중입니다.");
                }
                else if (_res.cmd == 3)
                {
                    this.StopPlayback();

                    //lock (rcvqueList)
                    //{
                    //    rcvqueList.Remove(ingData);
                    //}

                    //ThreadPool.QueueUserWorkItem(new WaitCallback(this.SendRtpUnReq), __requestTelNumber);
                    //this.SendRtpUnReq();

                    this.SetStatusMessage("실시간 청취가 중지되었습니다.");
                }
                else if (_res.cmd == 4)
                {

                }
                else if (_res.cmd == 5)
                {

                }
            }
        }
        #endregion Cmd Socket e

        #region RTP Socket s
        private void StartRtpSocket()
        {
            _localep_rtp = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                _serverep_rtp = new IPEndPoint(IPAddress.Parse(Options.recserverip), 21020);
                _sockRtp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //_sockRtp.Connect(_serverep_rtp);
                _sockRtp.Bind(_localep_rtp);
            }
            catch (SocketException se)
            {
                util.WriteLog(se.Message);
                this.StartRtpSocket();
            }

            _IsRtpSocketStarted = true;

            _threadRtpSocket = new Thread(new ThreadStart(RtpReceiver));
            _threadRtpSocket.IsBackground = true;
            _threadRtpSocket.Start();
        }

        private void RtpReceiver()
        {
            int __count = 0;

            while (_IsRtpSocketStarted)
            {
                byte[] __buffer = null;

                _rtp = new RecordInfo_t();
                __buffer = util.GetBytes(_rtp);

                __count = 0;

                try
                {
                    __count = _sockRtp.ReceiveFrom(__buffer, SocketFlags.None, ref _localep_rtp);
                }
                catch (SocketException se)
                {
                    //throw se;
                }

                if (__count == 0) return;

                _rtp = util.GetObject<RecordInfo_t>(__buffer);
                int nDataSize = _rtp.size - 12;
                if (nDataSize != 80 && nDataSize != 160 && nDataSize != 240 && nDataSize != -12) return;
                this.Rtp2Wav(_rtp);
            }
        }

        private void StopSocket()
        {
            _IsRtpSocketStarted = false;
            _threadRtpSocket.Join();
            _sockRtp.Close();
        }

        //private void SendRegReq()
        //{
        //    //_req = new RTPInterceptReq() { cmd = 1 };
        //    _req = new RTPInterceptReq() { cmd = 1, extnum = string.Empty };
        //    byte[] __bytes = util.GetBytes(_req);
        //    //_s.SendTo(__bytes, SocketFlags.None, _serverep);
        //    _s.Send(__bytes, 0, __bytes.Length, SocketFlags.None);
        //}

        //private void SendUnregReq()
        //{
        //    //_req = new RTPInterceptReq() { cmd = 2 };
        //    _req = new RTPInterceptReq() { cmd = 2, extnum = string.Empty };
        //    byte[] __bytes = util.GetBytes(_req);
        //    //_s.SendTo(__bytes, SocketFlags.None, _serverep);
        //    _s.Send(__bytes, 0, __bytes.Length, SocketFlags.None);
        //}
        #endregion RTP Socket e

        #region FileTransfer Socket s

        //private FileInfo _fileinfo;
        private Socket _sockFileClient;
        //private EndPoint _localep;
        //private EndPoint _remoteep;
        private Thread _sockFileThread;
        private bool _sockFileClientStarted = false;
        //private bool hasData = false;

        private void StartFileClient(List<Interview> _ques)
        {
            if (_sockFileClientStarted) return;

            try
            {
                //EndPoint _localep = new IPEndPoint(IPAddress.Any, 0);
                //_remoteep = new IPEndPoint(IPAddress.Parse("192.168.1.2"), 21022);
                //EndPoint _remoteep = new IPEndPoint(IPAddress.Parse(_option.ServerIP), 21022);
                //_sockFileClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //_sockFileClient.Bind(_localep);
                //_sockFileClient.Connect(_remoteep);
                //_sockFileClient.Listen(20);

                _sockFileThread = new Thread(new ThreadStart(this.FileSendReceiver));
                //_sockFileThread = new Thread(() => this.FileSendReceiver(_ques));
                //_sockFileThread = new Thread(new ParameterizedThreadStart(FileSendReceiver));
                _sockFileThread.IsBackground = true;
                //_sockFileThread.Start(_ques);
                _sockFileThread.Start();

                //ThreadPool.QueueUserWorkItem(new WaitCallback(this.SetStatusMessage), "파일 전송이 시작되었습니다.");

                _sockFileClientStarted = true;
            }
            catch(Exception e)
            {
                util.WriteLog(string.Format("StartFileClient : {0}", e.Message));
            }
        }

        private void StopFileClient()
        {
            //hasData = false;
            if (_sockFileClient != null)
            {
                try
                {
                    _sockFileClient.Disconnect(false);
                }
                catch (ObjectDisposedException ex)
                {
                    _sockFileClient.Close();
                }
                catch (SocketException se)
                {
                    _sockFileClient.Close();
                }
            }

            _sockFileClientStarted = false;

            //_sockFileClient.Dispose();
            //_sockFileClient = null;

            //_sockFileThread.Join();
            //_sockFileThread = null;
        }

        //private void FileSendReceiver(Object _ques)
        private void FileSendReceiver()
        {
            EndPoint _remoteep = new IPEndPoint(IPAddress.Parse(Options.recserverip), 21022);
            //_sockFileClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //_sockFileClient.Connect(_remoteep);

            //List<Interview> __ques = (List<Interview>)_ques;
            
            //foreach (Interview _item in __ques)
            foreach (Interview _item in _QuereqDownload)
            {
                try
                {
                    _sockFileClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    _sockFileClient.Connect(_remoteep);

                    Header _header = new Header()
                    {
                        Cmd = 1
                        ,
                        FileName = _item.recfile
                        ,
                        FileSize = 0
                    };

                    byte[] headerBuffer = null;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        _header.Serialize(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        headerBuffer = ms.ToArray();
                    }

                    // send the header  
                    int _sent = _sockFileClient.Send(headerBuffer, 0, headerBuffer.Length, SocketFlags.None);

                    byte[] buffer = new byte[1024];
                    //using (Socket sock = _sockFileClient.Accept())
                    using (NetworkStream ns = new NetworkStream(_sockFileClient))
                    {
                        // first get the header. Header has the file size.
                        byte[] headerbuffer = util.GetBytes(new FileTransferHeader());
                        Header header = Header.Deserialize(ns);

                        switch (header.Cmd)
                        {
                            case 3:
                                // 파일 download 응답 > Save
                                long remain = header.FileSize;
                                int read = 0;
                                MemoryStream ms = new MemoryStream();
                                while (remain > 0)
                                {
                                    long minbuffsize = buffer.LongLength;
                                    if (minbuffsize > remain)
                                        minbuffsize = remain;

                                    read = ns.Read(buffer, 0, (int)minbuffsize);

                                    if (read == 0)
                                        break;

                                    ms.Write(buffer, 0, read);
                                    remain -= read;
                                }

                                using (FileStream fs = File.OpenWrite(string.Format(@"{0}\{1}", Options.savedir, header.FileName)))
                                {
                                    ms.WriteTo(fs);
                                    ms.Close();
                                }

                                ThreadPool.QueueUserWorkItem(new WaitCallback(this.SetStatusMessage), string.Format("파일 전송이 완료 되었습니다. ({0})", header.FileName));
                                _item.CHK = false;
                                break;
                        }
                    }

                    _sockFileClient.Disconnect(false);
                    _sockFileClient.Close();
                    _sockFileClient.Dispose();
                    _sockFileClient = null;

                    //_sockFileThread.Abort();

                    //this.StopFileClient();
                }
                catch (SocketException ex)
                {
                    util.WriteLog("File Transfer error : " + ex.Message);

                    ThreadPool.QueueUserWorkItem(new WaitCallback(this.SetStatusMessage), "파일 전송에 문제가 발생했습니다. 녹취 서버를 확인해 주세요.");
                }
            }

            lock (_QuereqDownload)
            {
                _QuereqDownload.Clear();
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    chkheader.IsChecked = false;
                }));
            }

            _sockFileClientStarted = false;
            _sockFileThread.Abort();
        }

        #endregion FileTransfer Socket e

        // 내선 상태 요청
        private void SendInnertelStatusReq(object state)
        {
            _req = new InterceptRequest() { cmd = 1, extnum = string.Empty };
            //byte[] __bytes = util.GetBytes(_req);
            byte[] __bytes = util.ObjectToByteArray(_req);
            //_sockCmd.Send(__bytes, 0, __bytes.Length, SocketFlags.None);
            _sockCmd.SendTo(__bytes, SocketFlags.None, _serverep_cmd);
            

            //_interceptorStatus = InterceptorStatus.InnertelStatusReq;

            Thread.Sleep(1000);
            ThreadPool.QueueUserWorkItem(new WaitCallback(SendInnertelStatusReq));
        }

        // Send Command
        private void SendCmdReq(object state)
        {
            int __cmd = (int)state;

            _req = new InterceptRequest() { cmd = __cmd, extnum = string.Empty };
            //byte[] __bytes = util.GetBytes(_req);
            byte[] __bytes = util.ObjectToByteArray(_req);
            //_sockCmd.Send(__bytes, 0, __bytes.Length, SocketFlags.None);
            _sockCmd.SendTo(__bytes, SocketFlags.None, _serverep_cmd);
        }

        private void SendCmdReq(int __cmd)
        {
            //int __cmd = (int)state;

            _req = new InterceptRequest() { cmd = __cmd, extnum = string.Empty };
            //byte[] __bytes = util.GetBytes(_req);
            byte[] __bytes = util.ObjectToByteArray(_req);
            //_sockCmd.Send(__bytes, 0, __bytes.Length, SocketFlags.None);
            _sockCmd.SendTo(__bytes, SocketFlags.None, _serverep_cmd);
        }

        // RTP 요청
        private void SendRtpReq(object state)
        {
            string __telnum = state.ToString();

            _req = new InterceptRequest() { cmd = 2, extnum = __telnum };
            //byte[] __bytes = util.GetBytes(_req);
            byte[] __bytes = util.ObjectToByteArray(_req);
            //_sockCmd.Send(__bytes, 0, __bytes.Length, SocketFlags.None);
            _sockRtp.SendTo(__bytes, SocketFlags.None, _serverep_rtp);

            //_interceptorStatus = InterceptorStatus.RtpReq;
        }

        private void SendRtpReq(string __telnum)
        {
            //string __telnum = state.ToString();

            _req = new InterceptRequest() { cmd = 2, extnum = __telnum };
            //byte[] __bytes = util.GetBytes(_req);
            byte[] __bytes = util.ObjectToByteArray(_req);
            //_sockCmd.Send(__bytes, 0, __bytes.Length, SocketFlags.None);
            _sockRtp.SendTo(__bytes, SocketFlags.None, _serverep_rtp);

            //_interceptorStatus = InterceptorStatus.RtpReq;
        }

        // RTP 해제 요청
        private void SendRtpUnReq(object state)
        {
            _req = new InterceptRequest() { cmd = 3, extnum = "0000" };
            //byte[] __bytes = util.GetBytes(_req);
            byte[] __bytes = util.ObjectToByteArray(_req);
            //_sockCmd.Send(__bytes, 0, __bytes.Length, SocketFlags.None);
            _sockRtp.SendTo(__bytes, SocketFlags.None, _serverep_rtp);

            //_interceptorStatus = InterceptorStatus.RtpReq;
        }

        private void SendRtpUnReq()
        {
            _req = new InterceptRequest() { cmd = 3, extnum = "0000" };
            //byte[] __bytes = util.GetBytes(_req);
            byte[] __bytes = util.ObjectToByteArray(_req);
            //_sockCmd.Send(__bytes, 0, __bytes.Length, SocketFlags.None);
            _sockRtp.SendTo(__bytes, SocketFlags.None, _serverep_rtp);

            //_interceptorStatus = InterceptorStatus.RtpReq;
        }

        // 파일 청취 요청
        private void SendFileListenReq(object state)
        {
            _req = new InterceptRequest() { cmd = 4, extnum = string.Empty };
            //byte[] __bytes = util.GetBytes(_req);
            byte[] __bytes = util.ObjectToByteArray(_req);
            _sockCmd.Send(__bytes, 0, __bytes.Length, SocketFlags.None);

            //_interceptorStatus = InterceptorStatus.FileListenReq;
        }

        // 파일 다운로드 요청
        private void SendFileDownReq(object state)
        {
            _req = new InterceptRequest() { cmd = 5, extnum = string.Empty };
            byte[] __bytes = util.GetBytes(_req);
            _sockCmd.Send(__bytes, 0, __bytes.Length, SocketFlags.None);

            //_interceptorStatus = InterceptorStatus.FileTransferReq;
        }

        #endregion 소켓 관련 소스 e

#if true
        #region 소팅, 믹싱, 플레이
        enum StreamingPlaybackState
        {
            Stopped,
            Playing,
            Buffering,
            Paused
        }

        //private WaveFormat mulawFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.MuLaw, 8000, 1, 8000, 1, 8);
        private WaveFormat pcmFormat = new WaveFormat(8000, 16, 1);

        private List<RcvData> isExtensionIN = new List<RcvData>();
        private List<RcvData> isExtensionOUT = new List<RcvData>();
        private List<RecIngData> recIngList = new List<RecIngData>();

        public void Rtp2Wav(RecordInfo_t recordInfo)
        {
            DateTime __now = DateTime.Now;
            TimeSpan __ts = __now - new DateTime(1970, 1, 1, 0, 0, 0, 0);

            //this.MixEndFlush(__ts);

            RecIngData __ingData = recIngList.FirstOrDefault(x => x.extension == recordInfo.extension && x.peernumber == recordInfo.peer_number);

            if (__ingData == null)
            {
                WaveFormat __tmpwavformat = null;

                switch (recordInfo.codec)
                {
                    case 0:
                        //__tmpwavformat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.MuLaw, 8000, 1, 8000, 1, 8);
                        __tmpwavformat = WaveFormat.CreateMuLawFormat(8000, 1);
                        break;
                    case 8:
                        //__tmpwavformat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.ALaw, 8000, 1, 8000, 1, 8);
                        __tmpwavformat = WaveFormat.CreateALawFormat(8000, 1);
                        break;
                    case 4:
                        __tmpwavformat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.G723, 8000, 1, 8000, 1, 8);
                        break;
                    case 18:
                        __tmpwavformat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.G729, 8000, 1, 8000, 1, 8);
                        break;
                }

                string __header = string.Format("{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}{6:000}", __now.Year, __now.Month, __now.Day, __now.Hour, __now.Minute, __now.Second, __now.Millisecond);
                string __datepath = string.Format("{0:0000}-{1:00}-{2:00}", __now.Year, __now.Month, __now.Day);
                string __fileName = string.Format("{0}_{1}_{2}.wav", __header, recordInfo.extension, recordInfo.peer_number);
                string __wavFileName = string.Format(@"{0}\{1}\{2}", Options.savedir, __datepath, __fileName);

                __ingData = new RecIngData()
                {
                    seqidx = (long)__ts.TotalMilliseconds,
                    extension = recordInfo.extension,
                    peernumber = recordInfo.peer_number,
                    codec = recordInfo.codec,
                    savepath = __datepath,
                    wavefilename = __fileName,
                    wavformat = __tmpwavformat,
                    endcount = 0,
                    endtimer = (long)__ts.TotalMilliseconds
                };

                lock (recIngList)
                {
                    recIngList.Add(__ingData);
                }
            }

            this.StackRtpPayload(recordInfo);

            if (recordInfo.size == 0)
            {
                if (__ingData.endcount > 0)
                {
                    // mixing, writing
                    this.MixFlush(__ingData, 1);

                    lock (isExtensionIN)
                    {
                        isExtensionIN.RemoveAll(x => x.extension == __ingData.extension && x.peernumber == __ingData.peernumber);
                        //isExtensionIN.RemoveAll(x => x.seqidx == __ingData.seqidx);
                    }
                    lock (isExtensionOUT)
                    {
                        isExtensionOUT.RemoveAll(x => x.extension == __ingData.extension && x.peernumber == __ingData.peernumber);
                        //isExtensionOUT.RemoveAll(x => x.seqidx == __ingData.seqidx);
                    }

                    lock (recIngList)
                    {
                        recIngList.Remove(__ingData);
                        __ingData.Dispose(); __ingData = null;
                    }

                    return;
                }

                __ingData.endcount++;

                return;
            }

            this.MixFlush(__ingData, 1);
        }

#if true
        public void StackRtpPayload(RecordInfo_t _recInfo)
        {
            if (_recInfo.size == 0) return;

            RcvData _data = new RcvData()
            {
                //seqidx = _seqidx,
                isExtension = _recInfo.isExtension,
                extension = _recInfo.extension,
                peernumber = _recInfo.peer_number,
                seqnum = _recInfo.seq,
                size = _recInfo.size,
                buffers = _recInfo.voice
            };

            if (_data.isExtension == 1)
            {
                lock (isExtensionIN)
                {
                    isExtensionIN.Add(_data);
                }
            }
            else if (_data.isExtension == 0)
            {
                lock (isExtensionOUT)
                {
                    isExtensionOUT.Add(_data);
                }
            }
        }
#endif

        private void MixFlush(RecIngData __ingData, short __token)
        {
            List<RcvData> __rcvdatumIN;
            List<RcvData> __rcvdatumOUT;
            lock (isExtensionIN)
            {
                __rcvdatumIN = isExtensionIN.FindAll(x => x.extension == __ingData.extension && x.peernumber == __ingData.peernumber);
            }
            lock (isExtensionOUT)
            {
                __rcvdatumOUT = isExtensionOUT.FindAll(x => x.extension == __ingData.extension && x.peernumber == __ingData.peernumber);
            }

            CompareRtpSeq __comSort = new CompareRtpSeq();
            __rcvdatumIN.Sort(__comSort);
            __rcvdatumOUT.Sort(__comSort);

            int __count = 0;
            if (__rcvdatumIN.Count - __rcvdatumOUT.Count() < 0)
            {
                __count = __rcvdatumIN.Count();
            }
            else
            {
                __count = __rcvdatumOUT.Count();
            }

            if (__count == 0) return;

            if (__token == 0)
            {
                if (__count < 300) return;
            }

            for (int i = 0; i < __count; i++)
            {
                RcvData __basedata;
                RcvData __rcvdataIn = __rcvdatumIN.FirstOrDefault();
                RcvData __rcvdataOut = __rcvdatumOUT.FirstOrDefault();
                //RcvData __rcvdataOut = __rcvdatumOUT.FirstOrDefault(x => x.seqnum == __rcvdataIn.seqnum);

                if (__rcvdataIn.seqnum - __rcvdataOut.seqnum < 0)
                {
                    __basedata = __rcvdataIn;
                }
                else if (__rcvdataIn.seqnum - __rcvdataOut.seqnum > 0)
                {
                    __basedata = __rcvdataOut;
                }
                else
                {
                    __basedata = __rcvdataIn;
                }

                if (__basedata.isExtension == 1)
                {
                    __rcvdataOut = __rcvdatumOUT.FirstOrDefault(x => x.seqnum == __basedata.seqnum);
                }
                else
                {
                    __rcvdataIn = __rcvdatumIN.FirstOrDefault(x => x.seqnum == __basedata.seqnum);
                }
#if true
                if (__rcvdataIn == null || __rcvdataOut == null)
                {
                    // 삭제
                    if (__rcvdataIn != null)
                    {
                        lock (isExtensionIN)
                        {
                            isExtensionIN.Remove(__rcvdataIn);
                        }

                        __rcvdataIn.Dispose(); __rcvdataIn = null;
                    }

                    if (__rcvdataOut != null)
                    {
                        lock (isExtensionOUT)
                        {
                            isExtensionOUT.Remove(__rcvdataOut);
                        }

                        __rcvdataOut.Dispose(); __rcvdataOut = null;
                    }

                    continue;
                }
#endif

                if (__rcvdataIn.size == 0 || __rcvdataOut.size == 0)
                    continue;

                byte[] writingBuffer = new byte[(__rcvdataIn.size - 12) * 2];

                byte[] wavSrcIn = new byte[__rcvdataIn.size - 12];
                byte[] wavSrcOut = new byte[__rcvdataOut.size - 12];

                Array.Copy(__rcvdataIn.buffers, 12, wavSrcIn, 0, wavSrcIn.Length);
                Array.Copy(__rcvdataOut.buffers, 12, wavSrcOut, 0, wavSrcOut.Length);

                WaveMixerStream32 mixer = new WaveMixerStream32();
                //mixer.AutoStop = true;

                WaveChannel32 channelStm0 = null;
                WaveChannel32 channelStm1 = null;
                MemoryStream memStm0 = null;
                MemoryStream memStm1 = null;
                RawSourceWaveStream rawSrcStm0 = null;
                RawSourceWaveStream rawSrcStm1 = null;
                WaveFormatConversionStream conversionStm0 = null;
                WaveFormatConversionStream conversionStm1 = null;

                for (int j = 0; j < 2; j++)
                {
                    if (j == 0)
                    {
                        memStm0 = new MemoryStream(wavSrcIn);
                        rawSrcStm0 = new RawSourceWaveStream(memStm0, __ingData.wavformat);
                        conversionStm0 = new WaveFormatConversionStream(pcmFormat, rawSrcStm0);
                        channelStm0 = new WaveChannel32(conversionStm0);
                        mixer.AddInputStream(channelStm0);
                    }
                    else if (j == 1)
                    {
                        memStm1 = new MemoryStream(wavSrcOut);
                        rawSrcStm1 = new RawSourceWaveStream(memStm1, __ingData.wavformat);
                        conversionStm1 = new WaveFormatConversionStream(pcmFormat, rawSrcStm1);
                        channelStm1 = new WaveChannel32(conversionStm1);
                        mixer.AddInputStream(channelStm1);
                    }
                }

                mixer.Position = 0;

                Wave32To16Stream to16 = new Wave32To16Stream(mixer);
                var convStm = new WaveFormatConversionStream(pcmFormat, to16);
                byte[] tobyte = new byte[(int)convStm.Length];
                int chk = convStm.Read(tobyte, 0, (int)convStm.Length);
                Buffer.BlockCopy(tobyte, 0, writingBuffer, 0, tobyte.Length);

                channelStm0.Close(); channelStm0.Dispose(); channelStm0 = null;
                conversionStm0.Close(); conversionStm0.Dispose(); conversionStm0 = null;
                rawSrcStm0.Close(); rawSrcStm0.Dispose(); rawSrcStm0 = null;
                memStm0.Close(); memStm0.Dispose(); memStm0 = null;

                channelStm1.Close(); channelStm1.Dispose(); channelStm1 = null;
                conversionStm1.Close(); conversionStm1.Dispose(); conversionStm1 = null;
                rawSrcStm1.Close(); rawSrcStm1.Dispose(); rawSrcStm1 = null;
                memStm1.Close(); memStm1.Dispose(); memStm1 = null;

                convStm.Close(); convStm.Dispose(); convStm = null;
                to16.Close(); to16.Dispose(); to16 = null;
                mixer.Close(); mixer.Dispose(); mixer = null;

                // 삭제
                lock (isExtensionIN)
                {
                    isExtensionIN.Remove(__rcvdataIn);
                }

                lock (isExtensionOUT)
                {
                    isExtensionOUT.Remove(__rcvdataOut);
                }

                __rcvdataIn.Dispose(); __rcvdataIn = null;
                __rcvdataOut.Dispose(); __rcvdataOut = null;

                if (playbackState != StreamingPlaybackState.Stopped)
                {
                    this.StreamMedia(writingBuffer);
                }
            }

            __rcvdatumIN = null;
            __rcvdatumOUT = null;
        }

        private BufferedWaveProvider bufferedWaveProvider;
        private IWavePlayer waveOut;
        private volatile StreamingPlaybackState playbackState;
        private volatile bool fullyDownloaded;
        private VolumeWaveProvider16 volumeProvider;
        private WaveStream _playWavestream;

        private void StreamMedia(byte[] _stream)
        {
            if (playbackState == StreamingPlaybackState.Stopped) return;

            this.fullyDownloaded = false;

            if (_stream.Length == 0)
            {
                this.fullyDownloaded = true;
                return;
            }

            MemoryStream __memstream = new MemoryStream(_stream);

            //byte[] buffer = new byte[16384 * 4]; // needs to be big enough to hold a decompressed frame
            byte[] buffer = new byte[__memstream.Length];

            //var readFullyStream = new ReadFullyStream(__memstream);
            //do
            //{
            if (bufferedWaveProvider != null && bufferedWaveProvider.BufferLength - bufferedWaveProvider.BufferedBytes < bufferedWaveProvider.WaveFormat.AverageBytesPerSecond / 4)
            {
                Debug.WriteLine("Buffer getting full, taking a break");
                Thread.Sleep(500);
            }
            else
            {
                if (bufferedWaveProvider == null)
                {
                    this.bufferedWaveProvider = new BufferedWaveProvider(pcmFormat);
                    this.bufferedWaveProvider.BufferDuration = TimeSpan.FromSeconds(10); // allow us to get well ahead of ourselves
                }

                _playWavestream = new RawSourceWaveStream(__memstream, pcmFormat);
                //_playWavestream = new RawSourceWaveStream(readFullyStream, pcmFormat);
                _playWavestream.Read(buffer, 0, buffer.Length);
                bufferedWaveProvider.AddSamples(buffer, 0, buffer.Length);
            }
            //} while (playbackState != StreamingPlaybackState.Stopped);
        }

        void _playTimer_Tick(object state)
        {
            if (playbackState != StreamingPlaybackState.Stopped)
            {
                if (this.waveOut == null && this.bufferedWaveProvider != null)
                {
                    Debug.WriteLine("Creating WaveOut Device");
                    this.waveOut = CreateWaveOut();
                    waveOut.PlaybackStopped += waveOut_PlaybackStopped;
                    this.volumeProvider = new VolumeWaveProvider16(bufferedWaveProvider);
                    //this.volumeProvider.Volume = this.volumeSlider1.Volume;
                    waveOut.Init(volumeProvider);
                    //progressBarBuffer.Maximum = (int)bufferedWaveProvider.BufferDuration.TotalMilliseconds;
                }
                else if (bufferedWaveProvider != null)
                {
                    var bufferedSeconds = bufferedWaveProvider.BufferedDuration.TotalSeconds;
                    //ShowBufferState(bufferedSeconds);
                    // make it stutter less if we buffer up a decent amount before playing
                    if (bufferedSeconds < 0.5 && this.playbackState == StreamingPlaybackState.Playing && !this.fullyDownloaded)
                    {
                        this.playbackState = StreamingPlaybackState.Buffering;
                        waveOut.Pause();
                        Debug.WriteLine(String.Format("Paused to buffer, waveOut.PlaybackState={0}", waveOut.PlaybackState));
                    }
                    else if (bufferedSeconds > 4 && this.playbackState == StreamingPlaybackState.Buffering)
                    {
                        waveOut.Play();
                        Debug.WriteLine(String.Format("Started playing, waveOut.PlaybackState={0}", waveOut.PlaybackState));
                        this.playbackState = StreamingPlaybackState.Playing;
                    }
                    else if (this.fullyDownloaded && bufferedSeconds == 0)
                    {
                        Debug.WriteLine("Reached end of stream");
                        //ThreadPool.QueueUserWorkItem(new WaitCallback(this.SendRtpUnReq));
                        this.SendRtpUnReq();
                        StopPlayback();
                    }
                    //else if (bufferedSeconds == 0)
                    //{
                    //    Debug.WriteLine("Reached end of stream");
                    //    ThreadPool.QueueUserWorkItem(new WaitCallback(this.SendRtpUnReq));
                    //    StopPlayback();
                    //}
                }
            }

            Thread.Sleep(250);
            ThreadPool.QueueUserWorkItem(new WaitCallback(_playTimer_Tick));
        }

        private IWavePlayer CreateWaveOut()
        {
            return new WaveOut();
            //return new DirectSoundOut();
        }

        private void StopPlayback()
        {
            if (playbackState != StreamingPlaybackState.Stopped)
            {
                if (!fullyDownloaded)
                {
                    //webRequest.Abort();
                }
                this.playbackState = StreamingPlaybackState.Stopped;
                if (waveOut != null)
                {
                    waveOut.Stop();
                    waveOut.Dispose();
                    waveOut = null;
                }
                //_playTimer.Enabled = false;
                // n.b. streaming thread may not yet have exited
                Thread.Sleep(500);
                //ShowBufferState(0);
            }
        }

        private void waveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            Debug.WriteLine("Playback Stopped");
            if (e.Exception != null)
            {
                MessageBox.Show(String.Format("Playback Error {0}", e.Exception.Message));
            }
        }
        #endregion 소팅, 믹싱, 플레이 e
#endif

        #region List ContextMenu s
        private string __requestTelNumber = string.Empty;
        private void lvInnertels_realtimelisten_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.Source;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            ListView __lv = (ListView)contextMenu.PlacementTarget;

            InnerTel __obj = (InnerTel)__lv.SelectedItem;

            // 실시간 청취 요청
            __requestTelNumber = __obj.Telnum;
            //ThreadPool.QueueUserWorkItem(new WaitCallback(SendCmdReq), 2);
            this.SendCmdReq(2);
        }

        private void lvInnertels_realtimelistenCancel_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.Source;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            ListView __lv = (ListView)contextMenu.PlacementTarget;
            InnerTel __obj = (InnerTel)__lv.SelectedItem;

            // 실시간 청취 요청 중지
            __requestTelNumber = __obj.Telnum;
            //ThreadPool.QueueUserWorkItem(new WaitCallback(SendCmdReq), 3);
            this.SendCmdReq(3);
        }

        private List<Interview> _QuereqDownload = new List<Interview>();
        private void menu_download_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.Source;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            ListView _lv = (ListView)contextMenu.PlacementTarget;
            Interview _rowdata = (Interview)_lv.SelectedItem;
            //_rowdata.CHK = true;

            foreach (Interview _item in lvInterview.ItemsSource)
            {
                if (_item.CHK)
                {
                    lock (_QuereqDownload)
                    {
                        _QuereqDownload.Add(_item);
                    }
                }
            }

            this.StartFileClient(_QuereqDownload);
            //if (!_sockFileClient.Connected) return;

#if false
            try
            {
                //using (NetworkStream _ns = new NetworkStream(_sockFileClient))
                //{
                    Header _header = new Header()
                    {
                        Cmd = 1
                        ,
                        FileName = __rowdata.recfile
                        ,
                        FileSize = 0
                    };

                    byte[] headerBuffer = null;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        _header.Serialize(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        headerBuffer = ms.ToArray();
                    }

                    // send the header  
                    _sockFileClient.Send(headerBuffer, 0, headerBuffer.Length, SocketFlags.None);
                //}
            }
            catch (Exception ex)
            {
                util.WriteLog2(ex.Message);
            }
#endif
        }

        private void lvInterview_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ListView lv = (ListView)e.Source;
            Interview __rowdata = (Interview)lv.SelectedItem;

            if (__rowdata == null) return;

            ContextMenu cm = lv.ContextMenu;

            foreach (MenuItem mi in cm.Items)
            {
                mi.IsEnabled = true;
            }

            int __locSelectedIndex = lv.SelectedIndex;

            for (int i = 0; i < cm.Items.Count; i++)
            {
                MenuItem mi = (MenuItem)cm.Items[i];

                if (__locSelectedIndex == -1)
                {
                    mi.IsEnabled = false;
                }

                if (string.IsNullOrEmpty(__rowdata.recfile))
                {
                    mi.IsEnabled = false;
                }
            }
        }
        #endregion List ContextMenu e

        #region

        private void HideInnerTelList(Ini _ini)
        {
            _ini.IniWriteValue("SIZE", "col0", this.grid0.ColumnDefinitions[0].Width.Value.ToString());
            _ini.IniWriteValue("SIZE", "col1", this.grid0.ColumnDefinitions[1].Width.Value.ToString());

            this.grid0.ColumnDefinitions[0].Width = new GridLength(0.0d, GridUnitType.Pixel);
            //this.grid0.ColumnDefinitions[1].Width = new GridLength(100.0d, GridUnitType.Star);
        }

        private void ShowInnerTelList(Ini _ini)
        {
            double _col0 = string.IsNullOrEmpty(_ini.IniReadValue("SIZE", "col0")) == true ? 240.0d : double.Parse(_ini.IniReadValue("SIZE", "col0"));
            //double _col1 = string.IsNullOrEmpty(_ini.IniReadValue("SIZE", "col1")) == true ? 120.0d : double.Parse(_ini.IniReadValue("SIZE", "col1"));

            this.grid0.ColumnDefinitions[0].Width = new GridLength(_col0, GridUnitType.Pixel);
            //this.grid0.ColumnDefinitions[1].Width = new GridLength(100.0d, GridUnitType.Star);
        }
        #endregion 

        protected void SetStatusMessage(object message)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                txtStatus.Text = message.ToString();
            }));


            Thread.Sleep(10000);
            ThreadPool.QueueUserWorkItem(new WaitCallback(SetStatusMessage), "");
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (Interview _item in lvInterview.ItemsSource)
            {
                _item.CHK = true;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (Interview _item in lvInterview.ItemsSource)
            {
                _item.CHK = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();
            saveDialog.Filter = "Excel files (*.xls)|*.xls";
            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DataSet _ds = new DataSet();
                DataTable _dt = new DataTable();
                _dt.Columns.Add("통화일자", typeof(string));
                _dt.Columns.Add("통화시각", typeof(string));
                _dt.Columns.Add("전화번호", typeof(string));
                _dt.Columns.Add("녹취파일", typeof(string));
                
                foreach (Interview item in lvInterview.Items)
                {
                    _dt.Rows.Add(item.regyymmdd, item.reghhmmss, item.peernum, item.recfile);
                }
                
                _ds.Tables.Add(_dt);

                ExcelHelper.SaveExcelDB(saveDialog.FileName, _ds, true);
            }
        }
    }
}
