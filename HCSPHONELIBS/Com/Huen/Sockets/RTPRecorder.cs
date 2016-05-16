using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Com.Huen.Libs;
using Com.Huen.DataModel;
using Com.Huen.Sockets;
using Com.Huen.Sql;

using System.IO;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading;
using System.Windows.Threading;
using System.Net;
using System.Net.Sockets;
using NAudio.Wave;

using System.Diagnostics;
using System.ComponentModel;

namespace Com.Huen.Sockets
{
    public class RTPRecorder : IDisposable
    {
        private ModifyRegistry _reg = null;
        private CRAgentOption _option = null;

        private HUDPClient client = null;
        private Socket _sockRTPSrv = null;
        //private Socket _sockFileSrv = null;
        private Socket _sockCmdSrv = null;

        private WaveFormat pcmFormat = new WaveFormat(8000, 16, 1);

        private string seqnum = string.Empty;

        private List<RcvData> isExtensionIN = new List<RcvData>();
        private List<RcvData> isExtensionOUT = new List<RcvData>();
        private List<RecIngData> recIngList = new List<RecIngData>();

        private EndPoint _localep;
        private EndPoint _remoteep;
        private EndPoint _localepCmd;
        private EndPoint _remoteepCmd;
        //private EndPoint _localepFile;
        //private EndPoint _remoteepFile;

        private bool _IsSockInterceptorStarted = false;
        private bool _IsSockCmdSrvStarted = false;
        //private bool _IsSockFileSrvStarted = false;

        private Thread _threadIntercept;
        //private Thread _threadCommand;
        //private Thread _threadFile;

        private List<InterceptorClient> _cmdClientList = new List<InterceptorClient>();
        private List<InterceptorClient> _rtpRedirectClientList = new List<InterceptorClient>();
        private List<IPEndPoint> _fileClientList = new List<IPEndPoint>();

        private ObservableCollection<InnerTel> _innertelstatus = new ObservableCollection<InnerTel>();

        public RTPRecorder()
        {
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
            ThreadPool.QueueUserWorkItem(new WaitCallback(LoadRegistry));

            client = new HUDPClient();
            client.UDPClientEventReceiveMessage += client_UDPClientEventReceiveMessage;

            client.SocketMsgKinds = MsgKinds.RecordInfo;
            client.ServerPort = 21010;
            client.StartServer();
        }

        public RTPRecorder(MsgKinds __msgkinds, int __port)
        {
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
            ThreadPool.QueueUserWorkItem(new WaitCallback(LoadRegistry));

            client = new HUDPClient();
            client.UDPClientEventReceiveMessage += client_UDPClientEventReceiveMessage;

            client.SocketMsgKinds = __msgkinds;
            client.ServerPort = __port;
        }

        private void LoadRegistry(object state)
        {
            _reg = new ModifyRegistry(util.LoadProjectResource("REG_SUBKEY_CALLRECORDER", "COMMONRES", "").ToString());
            byte[] __bytes = (byte[])_reg.GetValue(RegKind.LocalMachine, "CR");
            _option = (CRAgentOption)util.ByteArrayToObject(__bytes);

            Thread.Sleep(2000);
            ThreadPool.QueueUserWorkItem(new WaitCallback(LoadRegistry));
        }

        #region RTP Redirect Server s
        public void StartRtpRedirectSrv()
        {
            _localep = new IPEndPoint(IPAddress.Any, 21020);
            _remoteep = new IPEndPoint(IPAddress.Any, 0);

            _sockRTPSrv = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _sockRTPSrv.Bind(_localep);

            _IsSockInterceptorStarted = true;

            _threadIntercept = new Thread(new ThreadStart(RtpRedirectSendReceiver));
            _threadIntercept.IsBackground = true;
            _threadIntercept.Start();
        }

        public void StopRtpRedirectSrv()
        {
            _sockRTPSrv.Close();
            _sockRTPSrv = null;
        }

        private void RtpRedirectSendReceiver()
        {
            try
            {
                InterceptRequest __req;
                //InterceptRes __res;
                int __count = 0;

                while (_IsSockInterceptorStarted)
                {
                    __req = new InterceptRequest() { cmd = 0, extnum = "0000" };
                    //byte[] __bytes = util.GetBytes(__req);
                    byte[] __bytes = util.ObjectToByteArray(__req);

                    __count = 0;
                    __count = _sockRTPSrv.ReceiveFrom(__bytes, SocketFlags.None, ref _remoteep);

                    if (__count == 0) return;

                    //__req = util.GetObject<InterceptReq>(__bytes);
                    __req = util.ByteArrayToObject<InterceptRequest>(__bytes);

                    InterceptorClient __tmpObj = null;
                    int __idx = 0;
                    switch (__req.cmd)
                    {
                        case 2:
                            // RTP 클라이언트 등록 요청
                            //__res = new RTPInterceptRes() { cmd = __req.cmd, result = 1 };
                            //__bytes = util.GetBytes(__res);
                            //__count = _sockRTPSrv.SendTo(__bytes, _remoteep);

                            var __tmpList = _rtpRedirectClientList.Where(x => x.ClientIPEP.Address == ((IPEndPoint)_remoteep).Address);

                            if (__tmpList.Count() == 0)
                            {
                                lock (_rtpRedirectClientList)
                                {
                                    _rtpRedirectClientList.Add(new InterceptorClient() { ClientIPEP = (IPEndPoint)_remoteep, ClientRegdate = DateTime.Now, ReqtelNum = __req.extnum });
                                }
                            }
                            else
                            {
                                lock (_rtpRedirectClientList)
                                {
                                    __tmpObj = _rtpRedirectClientList.FirstOrDefault(x => x.ClientIPEP.Address == ((IPEndPoint)_remoteep).Address);
                                    __idx = _rtpRedirectClientList.IndexOf(__tmpObj);
                                    _rtpRedirectClientList[__idx].ClientRegdate = DateTime.Now;
                                    _rtpRedirectClientList[__idx].ReqtelNum = __req.extnum;
                                }
                            }
                            break;
                        case 3:
                            // RTP 클라이언트 등록 해제 요청
                            //__res = new RTPInterceptRes() { cmd = __req.cmd, result = 1 };
                            //__bytes = util.GetBytes(__res);
                            //__count = _sockRTPSrv.Send(__bytes, __bytes.Length, SocketFlags.None);
                            //__count = _sockRTPSrv.SendTo(__bytes, _remoteep);

                            __tmpObj = _rtpRedirectClientList.FirstOrDefault(x => x.ClientIPEP.Address == ((IPEndPoint)_remoteep).Address);
                            lock (_rtpRedirectClientList)
                            {
                                _rtpRedirectClientList.Remove(__tmpObj);
                            }
                            break;
                    }
                }
            }
            catch (SocketException se)
            {
                throw se;
            }
        }
        #endregion RTP Redirect Server e

        #region Command Server s
        public void StartCmdSrv()
        {
            _localepCmd = new IPEndPoint(IPAddress.Any, 21021);
            _remoteepCmd = new IPEndPoint(IPAddress.Any, 0);

            _sockCmdSrv = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _sockCmdSrv.Bind(_localepCmd);

            _IsSockCmdSrvStarted = true;

            _threadIntercept = new Thread(new ThreadStart(CmdSendReceiver));
            _threadIntercept.IsBackground = true;
            _threadIntercept.Start();
        }

        public void StopCmdSrv()
        {
            _sockCmdSrv.Close();
            _sockCmdSrv = null;
        }

        private void CmdSendReceiver()
        {
            try
            {
                InterceptRequest __req = new InterceptRequest() { cmd = 0, extnum = string.Empty };
                InterceptResponse __res;
                int __count = 0;

                while (_IsSockCmdSrvStarted)
                {
                    //byte[] __bytes = util.GetBytes(__req);
                    byte[] __bytes = util.ObjectToByteArray(__req);

                    __count = 0;
                    __count = _sockCmdSrv.ReceiveFrom(__bytes, SocketFlags.None, ref _remoteepCmd);

                    if (__count == 0) return;

                    //__req = util.GetObject<InterceptReq>(__bytes);
                    __req = util.ByteArrayToObject<InterceptRequest>(__bytes);

                    switch (__req.cmd)
                    {
                        case 1:
                            // 내선 전화 상태 요청
                            __res = new InterceptResponse() { cmd = __req.cmd, result = 1, innertels = _innertelstatus };
                            //__bytes = util.GetBytes(__res);
                            __bytes = util.ObjectToByteArray(__res);
                            __count = _sockCmdSrv.SendTo(__bytes, _remoteepCmd);
                            break;
                        case 2:
                            // RTP Redirect 요청
                            __res = new InterceptResponse() { cmd = __req.cmd, result = 1 };
                            //__bytes = util.GetBytes(__res);
                            __bytes = util.ObjectToByteArray(__res);
                            //__count = _sockRTPSrv.Send(__bytes, __bytes.Length, SocketFlags.None);
                            __count = _sockCmdSrv.SendTo(__bytes, _remoteepCmd);
                            break;
                        case 3:
                            // RTP Redirect 해제 요청
                            __res = new InterceptResponse() { cmd = __req.cmd, result = 1 };
                            //__bytes = util.GetBytes(__res);
                            __bytes = util.ObjectToByteArray(__res);
                            __count = _sockCmdSrv.SendTo(__bytes, _remoteepCmd);
                            break;
                        case 4:
                            // 파일 Stream 요청
                            __res = new InterceptResponse() { cmd = __req.cmd, result = 1 };
                            //__bytes = util.GetBytes(__res);
                            __bytes = util.ObjectToByteArray(__res);
                            __count = _sockCmdSrv.SendTo(__bytes, _remoteep);
                            break;
                        case 5:
                            // 파일 Down 요청
                            __res = new InterceptResponse() { cmd = __req.cmd, result = 1 };
                            //__bytes = util.GetBytes(__res);
                            __bytes = util.ObjectToByteArray(__res);
                            __count = _sockCmdSrv.SendTo(__bytes, _remoteep);
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

        public void StartServer()
        {
            client.StartServer();
        }

        public void StopServer()
        {
            client.Stop();
        }

        void client_UDPClientEventReceiveMessage(object sender, byte[] buffer)
        {
            switch (client.SocketMsgKinds)
            {
                case MsgKinds.CdrRequest:
                    //CdrRequest_t cdr_req = (CdrRequest_t)e;
                    //CdrList cdrlist = (CdrList)util.GetObject<CdrList>(cdr_req.data);

                    //HUDPClient cl = (HUDPClient)sender;
                    //cl.Send(2, MsgKinds.CdrResponse, cdr_req);
                    break;
                case MsgKinds.RecordInfo:
                    RecordInfo_t recInfo = util.GetObject<RecordInfo_t>(buffer);

                    int nDataSize = recInfo.size - 12;

                    if (nDataSize != 80 && nDataSize != 160 && nDataSize != 240 && nDataSize != -12) return;

#if true
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

                    this.Rtp2Wav(recInfo);
#if true
                    // RTP Redirect
                    foreach (InterceptorClient __ic in _rtpRedirectClientList)
                    {
                        if (recInfo.extension == __ic.ReqtelNum)
                            _sockRTPSrv.SendTo(buffer, 0, buffer.Length, SocketFlags.None, __ic.ClientIPEP);
                    }
#endif
                    break;
            }
        }

        public void Rtp2Wav(RecordInfo_t recordInfo)
        {
            int __nDataSize = recordInfo.size;

            DateTime __now = DateTime.Now;
            TimeSpan __ts = __now - new DateTime(1970, 1, 1, 0, 0, 0, 0);

            this.MixEndFlush(__ts);

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
                string __wavFileName = string.Format(@"{0}\{1}\{2}", _option.SaveDirectory, __datepath, __fileName);

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

            if (__nDataSize == 0)
            {
                if (__ingData.endcount > 0)
                {
#if true
                    // 통화가 끝나면 내선의 peernumber 제거
                    InnerTel __tmpobj = _innertelstatus.FirstOrDefault(x => x.Telnum == recordInfo.extension);
                    if (__tmpobj != null)
                    {
                        int __idx = _innertelstatus.IndexOf(__tmpobj);
                        _innertelstatus[__idx].PeerNum = string.Empty;
                    }
                    // 통화가 끝나면 내선의 peernumber 제거
#endif

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

                    string __path = string.Format("{0}\\{1}", _option.SaveDirectory, __ingData.savepath);
                    if (!Directory.Exists(__path))
                    {
                        Directory.CreateDirectory(__path);
                    }

                    string _wavfilename = __ingData.wavefilename;
                    string _wavfilefullname = string.Format("{0}\\{1}\\{2}", _option.SaveDirectory, __ingData.savepath, __ingData.wavefilename);
                    //string _ext = __ingData.extension;
                    //string _peernum = __ingData.peernumber;

                    // change wave to mp3 & insert db
                    WriteFN2DB(_option.SaveFileType, _wavfilename, _wavfilefullname, __ingData.extension, __ingData.peernumber);

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

            this.MixFlush(__ingData, 0);
        }

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

        private void MixRtpPayload(RecIngData __ingData)
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

            RcvData __rcvdataIn = __rcvdatumIN.First();
            RcvData __rcvdataOut = __rcvdatumOUT.FirstOrDefault(x => x.seqnum == __rcvdataIn.seqnum);

            if (__rcvdataIn == null || __rcvdataOut == null)
                return;

            byte[] writingBuffer = new byte[(__rcvdataIn.size - 12) * 2];

            byte[] wavSrcIn = new byte[__rcvdataIn.size - 12];
            byte[] wavSrcOut = new byte[__rcvdataOut.size - 12];

            //Array.Copy(__rcvdataIn.buffers, 12, wavSrcIn, 0, wavSrcIn.Length);
            //Array.Copy(__rcvdataOut.buffers, 12, wavSrcOut, 0, wavSrcOut.Length);
            Buffer.BlockCopy(__rcvdataIn.buffers, 12, wavSrcIn, 0, wavSrcIn.Length);
            Buffer.BlockCopy(__rcvdataOut.buffers, 12, wavSrcOut, 0, wavSrcOut.Length);

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
                    //rawSrcStm0 = new RawSourceWaveStream(memStm0, pcmFormat);
                    conversionStm0 = new WaveFormatConversionStream(pcmFormat, rawSrcStm0);
                    channelStm0 = new WaveChannel32(conversionStm0);
                    //channelStm0 = new WaveChannel32(rawSrcStm0);
                    mixer.AddInputStream(channelStm0);
                }
                else if (j == 1)
                {
                    memStm1 = new MemoryStream(wavSrcOut);
                    rawSrcStm1 = new RawSourceWaveStream(memStm1, __ingData.wavformat);
                    //rawSrcStm1 = new RawSourceWaveStream(memStm1, pcmFormat);
                    conversionStm1 = new WaveFormatConversionStream(pcmFormat, rawSrcStm1);
                    channelStm1 = new WaveChannel32(conversionStm1);
                    //channelStm1 = new WaveChannel32(rawSrcStm1);
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
            //lock (isExtensionIN) isExtensionIN.Remove(__rcvdataIn);
            //lock (isExtensionOUT) isExtensionOUT.Remove(__rcvdataOut);
            __rcvdataIn.Dispose(); __rcvdataIn = null;
            __rcvdataOut.Dispose(); __rcvdataOut = null;
            __rcvdatumIN = null;
            __rcvdatumOUT = null;

            this.WaveFileWriting(writingBuffer, __ingData);
        }

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

            if ( __count == 0 ) return;

            if (__token == 0)
            {
                if (__count < 200) return;
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

                RcvData _rcvdataOut = __rcvdatumOUT.FirstOrDefault(x => x.seqnum == __basedata.seqnum);
                RcvData _rcvdataIn = __rcvdatumIN.FirstOrDefault(x => x.seqnum == __basedata.seqnum);

                if (__rcvdataIn == null || __rcvdataOut == null)
                {
                    continue;
                }

                //if (__basedata.isExtension == 1)
                //{
                //    __rcvdataOut = __rcvdatumOUT.FirstOrDefault(x => x.seqnum == __basedata.seqnum);
                //}
                //else
                //{
                //    __rcvdataIn = __rcvdatumIN.FirstOrDefault(x => x.seqnum == __basedata.seqnum);
                //}

                //if (__rcvdataIn == null || __rcvdataOut == null)
                //{
                //    // 삭제
                //    if (__rcvdataIn != null)
                //    {
                //        lock (isExtensionIN)
                //        {
                //            isExtensionIN.Remove(__rcvdataIn);
                //        }

                //        __rcvdataIn.Dispose(); __rcvdataIn = null;
                //    }

                //    if (__rcvdataOut != null)
                //    {
                //        lock (isExtensionOUT)
                //        {
                //            isExtensionOUT.Remove(__rcvdataOut);
                //        }

                //        __rcvdataOut.Dispose(); __rcvdataOut = null;
                //    }

                //    continue;
                //}

                if (__rcvdataIn.size == 0 || __rcvdataOut.size == 0)
                    continue;

                byte[] writingBuffer = new byte[(__rcvdataIn.size - 12) * 2];

                byte[] wavSrcIn = new byte[__rcvdataIn.size - 12];
                byte[] wavSrcOut = new byte[__rcvdataOut.size - 12];

                Array.Copy(__rcvdataIn.buffers, 12, wavSrcIn, 0, wavSrcIn.Length);
                Array.Copy(__rcvdataOut.buffers, 12, wavSrcOut, 0, wavSrcOut.Length);
                //Buffer.BlockCopy(__rcvdataIn.buffers, 12, wavSrcIn, 0, wavSrcIn.Length);
                //Buffer.BlockCopy(__rcvdataOut.buffers, 12, wavSrcOut, 0, wavSrcOut.Length);

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
                //Buffer.BlockCopy(tobyte, 0, writingBuffer, 0, tobyte.Length);

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

                //this.WaveFileWriting(writingBuffer, __ingData);
                this.WaveFileWriting(tobyte, __ingData);
            }

            __rcvdatumIN = null;
            __rcvdatumOUT = null;
        }

        private void MixEndFlush(TimeSpan __ts)
        {
            // __ingData endtimer 가 30초이 넘으면 MixFlush (정상 종료가 아닌 경우)
            List<RecIngData> __remainList = recIngList.FindAll(x => ((long)__ts.TotalMilliseconds / 1000 - x.endtimer / 1000) >= 30);

            if (__remainList == null) return;

            for (int i = 0; i < __remainList.Count(); i++)
            {
                RecIngData __remainData = __remainList.First();
                this.MixFlush(__remainData, 1);

                lock (isExtensionIN)
                {
                    isExtensionIN.RemoveAll(x => x.extension == __remainData.extension && x.peernumber == __remainData.peernumber);
                    //isExtensionIN.RemoveAll(x => x.seqidx == __remainData.seqidx);
                }
                lock (isExtensionOUT)
                {
                    isExtensionOUT.RemoveAll(x => x.extension == __remainData.extension && x.peernumber == __remainData.peernumber);
                    //isExtensionOUT.RemoveAll(x => x.seqidx == __remainData.seqidx);
                }

                string __path = string.Format("{0}\\{1}", _option.SaveDirectory, __remainData.savepath);
                if (!Directory.Exists(__path))
                {
                    Directory.CreateDirectory(__path);
                }

                string _wavfilename = __remainData.wavefilename;
                string _wavfilefullname = string.Format("{0}\\{1}\\{2}", _option.SaveDirectory, __remainData.savepath, __remainData.wavefilename);
                //string _ext = __remainData.extension;
                //string _peernum = __remainData.peernumber;

                // change wave to mp3 & insert db
                WriteFN2DB(_option.SaveFileType, _wavfilename, _wavfilefullname, __remainData.extension, __remainData.peernumber);

                lock (recIngList)
                {
                    recIngList.Remove(__remainData);
                    __remainData.Dispose(); __remainData = null;
                }
            }
        }

        private void MixCloseFlush()
        {
            DateTime __now = DateTime.Now;
            TimeSpan __ts = __now - new DateTime(1970, 1, 1, 0, 0, 0, 0);

            // __ingData endtimer 가 30초이 넘으면 MixFlush (정상 종료가 아닌 경우)
            List<RecIngData> __remainList = recIngList.FindAll(x => ((long)__ts.TotalMilliseconds / 1000 - x.endtimer / 1000) >= 30);
            for (int i = 0; i < __remainList.Count(); i++)
            {
                RecIngData __remainData = __remainList.First();
                this.MixFlush(__remainData, 1);

                lock (isExtensionIN)
                {
                    isExtensionIN.RemoveAll(x => x.extension == __remainData.extension && x.peernumber == __remainData.peernumber);
                    //isExtensionIN.RemoveAll(x => x.seqidx == __remainData.seqidx);
                }
                lock (isExtensionOUT)
                {
                    isExtensionOUT.RemoveAll(x => x.extension == __remainData.extension && x.peernumber == __remainData.peernumber);
                    //isExtensionOUT.RemoveAll(x => x.seqidx == __remainData.seqidx);
                }

                string __path = string.Format("{0}\\{1}", _option.SaveDirectory, __remainData.savepath);
                if (!Directory.Exists(__path))
                {
                    Directory.CreateDirectory(__path);
                }

                string _wavfilename = __remainData.wavefilename;
                string _wavfilefullname = string.Format("{0}\\{1}\\{2}", _option.SaveDirectory, __remainData.savepath, __remainData.wavefilename);
                //string _ext = __remainData.extension;
                //string _peernum = __remainData.peernumber;

                // change wave to mp3 & insert db
                WriteFN2DB(_option.SaveFileType, _wavfilename, _wavfilefullname, __remainData.extension, __remainData.peernumber);

                lock (recIngList)
                {
                    recIngList.Remove(__remainData);
                    __remainData.Dispose(); __remainData = null;
                }
            }
        }

        private void WaveFileWriting(byte[] buff, RecIngData __ingData)
        {
            if (buff.Length == 0) return;

            using (MemoryStream memStm = new MemoryStream(buff))
            using (RawSourceWaveStream rawSrcStm = new RawSourceWaveStream(memStm, pcmFormat))
            {
                if (__ingData.writer == null)
                {
                    string __path = string.Format("{0}\\{1}", _option.SaveDirectory, __ingData.savepath);
                    if (!Directory.Exists(__path))
                    {
                        Directory.CreateDirectory(__path);
                    }
                    string __filefullname = string.Format("{0}\\{1}\\{2}", _option.SaveDirectory, __ingData.savepath, __ingData.wavefilename);
                    __ingData.writer = new WaveFileWriter(__filefullname, pcmFormat);
                }

                __ingData.writer.Write(buff, 0, buff.Length);
                __ingData.writer.Flush();

                DateTime __now = DateTime.Now;
                TimeSpan __ts = __now - new DateTime(1970, 1, 1, 0, 0, 0, 0);

                __ingData.endtimer = (long)__ts.TotalMilliseconds;
            }
        }

        private void LameWavToMp3(string wavFile, string outmp3File)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = @".\lame.exe";
                psi.Arguments = "-h -b 32 " + wavFile + " " + outmp3File;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                Process p = Process.Start(psi);
                p.WaitForExit();
            }
            catch (Exception e)
            {
                util.WriteLog(e.Message);
                //throw e;
            }
        }

        private void FileName2DB(string _fn, string _ext, string _peernum)
        {
            DataTable dt = util.CreateDT2SP();
            dt.Rows.Add("@EXTENTION", _ext);
            dt.Rows.Add("@PEERNUMBER", _peernum);
            dt.Rows.Add("@FN", _fn);

            using (FirebirdDBHelper db = new FirebirdDBHelper(util.strFBDBConn))
            {
                try
                {
                    db.BeginTran();
                    db.ExcuteSP("INS_RECINFO", dt);
                    db.Commit();
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException se)
                {
                    db.Rollback();
                    util.WriteLog(se.Message);
                }
            }
        }

        private void WriteFN2DB(string _filetype, string _wavfilename, string _wavfilefullname, string _ext, string _peernum)
        {
            // change wave to mp3 & insert db
            if (_filetype == "MP3")
            {
                string __mp3FileName = _wavfilename.Replace(".wav", ".mp3");
                string __mp3FullFileName = _wavfilefullname.Replace(".wav", ".mp3");

                this.FileName2DB(__mp3FileName, _ext, _peernum);

                LameWavToMp3(_wavfilefullname, __mp3FullFileName);

                if (File.Exists(_wavfilefullname)) File.Delete(_wavfilefullname);
            }
            else
            {
                this.FileName2DB(_wavfilename, _ext, _peernum);
            }
        }

        public void Dispose()
        {
            this.MixCloseFlush();

            this._sockRTPSrv.Close();
            this._sockCmdSrv.Close();

            this.client.Dispose();
            this._sockRTPSrv.Dispose();
            this._sockCmdSrv.Dispose();

            this._sockRTPSrv = null;
            this._sockCmdSrv = null;
        }
    }
}
