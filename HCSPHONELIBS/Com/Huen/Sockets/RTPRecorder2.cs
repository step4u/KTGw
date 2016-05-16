using System;
using System.Collections.Generic;
using System.Linq;

using Com.Huen.Libs;
using Com.Huen.DataModel;
using Com.Huen.Sql;

using System.IO;
using System.Data;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using NAudio.Wave;

using System.Diagnostics;
using FirebirdSql.Data.FirebirdClient;

namespace Com.Huen.Sockets
{
    public class RTPRecorder2 : IDisposable
    {
        private string inipath = string.Format(@"{0}\{1}.ini", Options.usersdefaultpath, Options.appname);

        private UdpClient client = null;
        private Socket sockRTPSrv = null;
        private Socket sockCmdSrv = null;

        private WaveFormat pcmFormat = new WaveFormat(8000, 16, 1);

        private List<RtpRecordInfo> RecordIngList;

        private int threadcount = 1;
        private int port = 21010;
        private IPEndPoint localep;
        private IPEndPoint remoteep;
        private EndPoint localepRedirect;
        private EndPoint remoteepRedirect;
        private EndPoint localepCmd;
        private EndPoint remoteepCmd;

        private bool IsSockInterceptorStarted = false;
        private bool IsSockCmdSrvStarted = false;

        private Thread threadUdpClient;
        private Thread threadIntercept;

        // private List<InterceptorClient> cmdClientList = new List<InterceptorClient>();
        private List<InterceptorClient> rtpRedirectClientList = new List<InterceptorClient>();
        // private List<IPEndPoint> fileClientList = new List<IPEndPoint>();

        public RTPRecorder2() : this (21010, 1)
        {
        }

        public RTPRecorder2(int port) : this (port, 1)
        {
        }

        public RTPRecorder2(int port, int threadcount)
        {
            this.threadcount = threadcount;
            this.port = port;

            this.ReadIni();

            RecordIngList = new List<RtpRecordInfo>();

            remoteep = new IPEndPoint(IPAddress.Any, 0);
            localep = new IPEndPoint(IPAddress.Any, this.port);
            client = new UdpClient(localep);

            for(int i = 0; i < this.threadcount; i++)
            {
                threadUdpClient = new Thread(new ThreadStart(SendReceiveMsg));
                threadUdpClient.IsBackground = true;
                threadUdpClient.Start();
            }
        }

        private void ReadIni()
        {
            Ini ini = new Ini(inipath);

            Options.filetype = string.IsNullOrEmpty(ini.IniReadValue("RECORDER", "filetype").ToLower()) == false ? ini.IniReadValue("RECORDER", "filetype").ToLower() : "wav";
            Options.savedir = string.IsNullOrEmpty(ini.IniReadValue("RECORDER", "savedir")) == false ? ini.IniReadValue("RECORDER", "savedir") : string.Format(@"{0}\RecFiles", Options.usersdefaultpath);
            Options.dbserverip = string.IsNullOrEmpty(ini.IniReadValue("RECORDER", "dbserverip")) == false ? ini.IniReadValue("RECORDER", "dbserverip") : "127.0.0.1";
        }

        /*
        private void ReadIni(Object state)
        {
            Ini ini = new Ini(inipath);

            Options.filetype = string.IsNullOrEmpty(ini.IniReadValue("RECORDER", "filetype").ToLower()) == false ? ini.IniReadValue("RECORDER", "filetype").ToLower() : "wav";
            Options.savedir = string.IsNullOrEmpty(ini.IniReadValue("RECORDER", "savedir")) == false ? ini.IniReadValue("RECORDER", "savedir") : string.Format(@"{0}\RecFiles", Options.usersdefaultpath);
            Options.dbserverip = string.IsNullOrEmpty(ini.IniReadValue("RECORDER", "dbserverip")) == false ? ini.IniReadValue("RECORDER", "dbserverip") : "127.0.0.1";

            Thread.Sleep(3000);
            ThreadPool.QueueUserWorkItem(new WaitCallback(ReadIni));
        }
        */

        #region RTP Redirect Server s
        public void StartRtpRedirectSrv()
        {
            localepRedirect = new IPEndPoint(IPAddress.Any, 21020);
            remoteepRedirect = new IPEndPoint(IPAddress.Any, 0);

            sockRTPSrv = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sockRTPSrv.Bind(localepRedirect);

            IsSockInterceptorStarted = true;

            threadIntercept = new Thread(new ThreadStart(RtpRedirectSendReceiver));
            threadIntercept.IsBackground = true;
            threadIntercept.Start();
        }

        public void StopRtpRedirectSrv()
        {
            sockRTPSrv.Close();
            sockRTPSrv = null;
        }

        private void RtpRedirectSendReceiver()
        {
            try
            {
                InterceptRequest req;
                int count = 0;

                while (IsSockInterceptorStarted)
                {
                    req = new InterceptRequest() { cmd = 0, extnum = "0000" };
                    byte[] bytes = util.ObjectToByteArray(req);

                    count = 0;
                    count = sockRTPSrv.ReceiveFrom(bytes, SocketFlags.None, ref remoteepRedirect);

                    if (count == 0) return;

                    req = util.ByteArrayToObject<InterceptRequest>(bytes);

                    InterceptorClient tmpObj = null;
                    int idx = 0;
                    switch (req.cmd)
                    {
                        case 2:
                            // RTP 클라이언트 등록 요청
                            var tmpList = rtpRedirectClientList.Where(x => x.ClientIPEP.Address == ((IPEndPoint)remoteep).Address);

                            if (tmpList.Count() == 0)
                            {
                                lock (rtpRedirectClientList)
                                {
                                    rtpRedirectClientList.Add(new InterceptorClient() { ClientIPEP = (IPEndPoint)remoteep, ClientRegdate = DateTime.Now, ReqtelNum = req.extnum });
                                }
                            }
                            else
                            {
                                lock (rtpRedirectClientList)
                                {
                                    tmpObj = rtpRedirectClientList.FirstOrDefault(x => x.ClientIPEP.Address == ((IPEndPoint)remoteep).Address);
                                    idx = rtpRedirectClientList.IndexOf(tmpObj);
                                    rtpRedirectClientList[idx].ClientRegdate = DateTime.Now;
                                    rtpRedirectClientList[idx].ReqtelNum = req.extnum;
                                }
                            }
                            break;
                        case 3:
                            // RTP 클라이언트 등록 해제 요청
                            tmpObj = rtpRedirectClientList.FirstOrDefault(x => x.ClientIPEP.Address == ((IPEndPoint)remoteep).Address);
                            lock (rtpRedirectClientList)
                            {
                                rtpRedirectClientList.Remove(tmpObj);
                            }
                            break;
                    }
                }
            }
            catch (SocketException e)
            {
                throw e;
            }
        }
        #endregion RTP Redirect Server e

        #region Command Server s
        public void StartCmdSrv()
        {
            localepCmd = new IPEndPoint(IPAddress.Any, 21021);
            remoteepCmd = new IPEndPoint(IPAddress.Any, 0);

            sockCmdSrv = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sockCmdSrv.Bind(localepCmd);

            IsSockCmdSrvStarted = true;

            threadIntercept = new Thread(new ThreadStart(CmdSendReceiver));
            threadIntercept.IsBackground = true;
            threadIntercept.Start();
        }

        public void StopCmdSrv()
        {
            sockCmdSrv.Close();
            sockCmdSrv = null;
        }

        private void CmdSendReceiver()
        {
            try
            {
                InterceptRequest req = new InterceptRequest() { cmd = 0, extnum = string.Empty };
                InterceptResponse res;
                int count = 0;

                while (IsSockCmdSrvStarted)
                {
                    byte[] bytes = util.ObjectToByteArray(req);

                    count = 0;
                    count = sockCmdSrv.ReceiveFrom(bytes, SocketFlags.None, ref remoteepCmd);

                    if (count == 0) return;

                    req = util.ByteArrayToObject<InterceptRequest>(bytes);

                    switch (req.cmd)
                    {
#if false
                        case 1:
                            // 내선 전화 상태 요청
                            __res = new InterceptResponse() { cmd = __req.cmd, result = 1, innertels = _innertelstatus };
                            //__bytes = util.GetBytes(__res);
                            __bytes = util.ObjectToByteArray(__res);
                            __count = _sockCmdSrv.SendTo(__bytes, _remoteepCmd);
                            break;
#endif
                        case 2:
                            // RTP Redirect 요청
                            res = new InterceptResponse() { cmd = req.cmd, result = 1 };
                            bytes = util.ObjectToByteArray(res);
                            count = sockCmdSrv.SendTo(bytes, remoteepCmd);
                            break;
                        case 3:
                            // RTP Redirect 해제 요청
                            res = new InterceptResponse() { cmd = req.cmd, result = 1 };
                            bytes = util.ObjectToByteArray(res);
                            count = sockCmdSrv.SendTo(bytes, remoteepCmd);
                            break;
                        case 4:
                            // 파일 Stream 요청
                            res = new InterceptResponse() { cmd = req.cmd, result = 1 };
                            bytes = util.ObjectToByteArray(res);
                            count = sockCmdSrv.SendTo(bytes, remoteep);
                            break;
                        case 5:
                            // 파일 Down 요청
                            res = new InterceptResponse() { cmd = req.cmd, result = 1 };
                            bytes = util.ObjectToByteArray(res);
                            count = sockCmdSrv.SendTo(bytes, remoteep);
                            break;
                    }
                }
            }
            catch (SocketException se)
            {
                throw se;
            }
        }
        #endregion Command Server e

        private void SendReceiveMsg()
        {
            try
            {
                while (true)
                {
                    byte[] buffer = client.Receive(ref remoteep);

                    if (buffer.Length > 0)
                    {
                        RtpReceive(buffer);
                    }
                }
            }
            catch (System.Net.Sockets.SocketException e)
            {
                util.WriteLogTest2(e.ErrorCode + " : " + e.Message);
                threadUdpClient.Abort();
            }
        }

        private void RtpReceive(byte[] buffer)
        {
            RecordInfo_t recInfo = util.GetObject<RecordInfo_t>(buffer);
            int nDataSize = recInfo.size - 12;
            if (nDataSize != 80 && nDataSize != 160 && nDataSize != 240 && nDataSize != -12) return;

            // util.WriteLog(string.Format("seq:{0}, ext:{1}, peer:{2}, isExtension:{3}, size:{4}, bytesLength:{5}", recInfo.seq, recInfo.peer_number, recInfo.extension, recInfo.isExtension, recInfo.size - 12, recInfo.voice.Length));
            // util.WriteLogTest("");

#if false
            // 녹취 할 수 있는 내선 리스트 추가 및 상태 체크
            var __tmpCollection = _innertelstatus.Where(x => x.Telnum == recInfo.extension);
            if (__tmpCollection.Count() < 1)
            {
                lock (_innertelstatus)
                {
                    _innertelstatus.Add(new InnerTel() { Telnum = recInfo.extension, TellerName = string.Empty, PeerNum = recInfo.peer_number });
                }
            }
            else
            {
                InnerTel __tmpinntel = _innertelstatus.FirstOrDefault(x => x.Telnum == recInfo.extension);
                int __idx = _innertelstatus.IndexOf(__tmpinntel);
                _innertelstatus[__idx].PeerNum = recInfo.peer_number;
            }
            // 내선 리스트 추가 및 상태 체크
#endif
#if false
            // RTP Redirect
            foreach (InterceptorClient __ic in _rtpRedirectClientList)
            {
                if (recInfo.extension == __ic.ReqtelNum)
                    _sockRTPSrv.SendTo(buffer, 0, buffer.Length, SocketFlags.None, __ic.ClientIPEP);
            }
#endif
            this.StackRtp2Instance(recInfo);
        }

        private void StackRtp2Instance(RecordInfo_t recInfo)
        {
            var ingInstance = RecordIngList.FirstOrDefault(x => x.ext == recInfo.extension && x.peer == recInfo.peer_number);
            if (ingInstance == null)
            {
                WaveFormat wavformat;

                switch (recInfo.codec)
                {
                    case 0:
                        // wavformat = WaveFormat.CreateMuLawFormat(8000, 1);
                        // break;
                    case 8:
                        wavformat = WaveFormat.CreateALawFormat(8000, 1);
                        break;
                    case 4:
                        wavformat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.G723, 8000, 1, 8000, 1, 8);
                        break;
                    case 18:
                        wavformat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.G729, 8000, 1, 8000, 1, 8);
                        break;
                    default:
                        wavformat = WaveFormat.CreateMuLawFormat(8000, 1);
                        break;
                }

                DateTime now = DateTime.Now;
                TimeSpan ts = now - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);

                string header = string.Format("{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}{6:000}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
                string datepath = string.Format("{0:0000}-{1:00}-{2:00}", now.Year, now.Month, now.Day);
                string fileName = string.Format("{0}_{1}_{2}.wav", header, recInfo.extension, recInfo.peer_number);

                string path = string.Format(@"{0}\{1}", Options.savedir, datepath);

                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                RtpRecordInfo RecInstance = new RtpRecordInfo(wavformat, path, fileName) { ext = recInfo.extension, peer = recInfo.peer_number, codec = wavformat, idx = ts.TotalMilliseconds, savepath = path, filename = fileName };

                RecInstance.EndOfRtpStreamEven += RecInstance_EndOfRtpStreamEvent;

                RecInstance.Add(recInfo);
                lock (RecordIngList)
                {
                    RecordIngList.Add(RecInstance);
                }
            }
            else
            {
                ingInstance.Add(recInfo);
            }
        }

        void RecInstance_EndOfRtpStreamEvent(object sender, EventArgs e)
        {
            RtpRecordInfo ingdata = (RtpRecordInfo)sender;
            if (ingdata != null)
            {
                string _savepath = ingdata.savepath;
                string _savefn = ingdata.filename;
                string _ext = ingdata.ext;
                string _peer = ingdata.peer;

#if false
                var __tmpCollection = _innertelstatus.FirstOrDefault(x => x.Telnum == ingdata.ext);
                if (__tmpCollection != null)
                {
                    lock (_innertelstatus)
                    {
                        _innertelstatus.Remove(__tmpCollection);
                    }
                }
#endif

                lock (RecordIngList)
                {
                    RecordIngList.Remove(ingdata);
                }

                ingdata.Dispose();
                ingdata = null;

                if (Options.filetype.Equals("mp3"))
                {
                    _savefn = this.LameWavToMp3(_savepath, _savefn);
                }

                this.FileName2DB(_savefn, _ext, _peer);
            }
        }

        private string LameWavToMp3(string savepath, string wavFN)
        {
            string mp3fn = string.Empty;
            string fullwavfn = string.Empty;
            string fullmp3fn = string.Empty;

            try
            {
                fullwavfn = string.Format("{0}\\{1}", savepath, wavFN);
                mp3fn = wavFN.Replace(".wav", ".mp3");
                fullmp3fn = string.Format("{0}\\{1}", savepath, mp3fn);

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = @".\lame.exe";
                psi.Arguments = "-h -b 32 " + fullwavfn + " " + fullmp3fn;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                Process p = Process.Start(psi);
                p.WaitForExit();

                if (File.Exists(fullwavfn)) File.Delete(fullwavfn);
            }
            catch (Exception e)
            {
                util.WriteLog(e.Message);
                //throw e;
            }

            return mp3fn;
        }

        private void FileName2DB(string fn, string ext, string peernum)
        {
            try
            {
                using (FirebirdDBHelper db = new FirebirdDBHelper(util.strFBDBConn))
                {
                    db.SetParameters("@EXTENTION", FbDbType.VarChar, ext);
                    db.SetParameters("@PEERNUMBER", FbDbType.VarChar, peernum);
                    db.SetParameters("@FN", FbDbType.VarChar, fn);

                    try
                    {
                        db.BeginTran();
                        db.ExcuteSP("INS_RECINFO");
                        db.Commit();
                    }
                    catch (FbException e)
                    {
                        util.WriteLog(string.Format("SQL INS ERROR (INS_RECINF)\r\nMessage : {0}", e.Message));
                        db.Rollback();
                    }
                }
            }
            catch (FbException e)
            {
                util.WriteLog(string.Format("SQL INS ERROR (INS_RECINF)\r\nMessage : {0}", e.Message));
            }
        }

        public void Dispose()
        {
            this.sockRTPSrv.Close();
            this.sockCmdSrv.Close();

            this.sockRTPSrv.Dispose();
            this.sockCmdSrv.Dispose();

            this.sockRTPSrv = null;
            this.sockCmdSrv = null;
        }
    }
}
