using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Com.Huen.Libs;
using Com.Huen.DataModel;

namespace Com.Huen.Sockets
{

    public delegate void UDPClientReceiveEventHandler(object sender, Object e);
    public delegate void UDPClientReceiveEventHandler2(object sender, byte[] buffer);
    //public delegate void UdpClientConnectedEventHandler(object sender, bool e);
    //public delegate void UdpClientDisConnectedEventHandler(object sender, bool e);

    public class HUDPClient : IDisposable
    {
        public event UDPClientReceiveEventHandler2 UDPClientEventReceiveMessage;

        private Thread SockThread;

        public System.Net.Sockets.UdpClient udpclient = null;

        private IPEndPoint _remoteServerIEP;
        private string _remoteServerIp = string.Empty;
        private int _remoteServerPort = 0;
        private int _serverPort = 0;
        private string _from_ext = string.Empty;
        private string _to_ext = string.Empty;
        private string _userid = string.Empty;
        private MsgKinds _msgkind = MsgKinds.CommandMessage;
        private bool _IsConnected = false;
        private bool _IsStarted = false;

        #region 속성
        public IPEndPoint RemoteServerIEP
        {
            get
            {
                _remoteServerIEP = new IPEndPoint(IPAddress.Parse(RemoteServerIP), RemoteServerPort);
                return _remoteServerIEP;
            }
            set
            {
                _remoteServerIEP = value;
            }
        }

        public string RemoteServerIP
        {
            get
            {
                return string.IsNullOrEmpty(_remoteServerIp) ? (string)util.LoadProjectResource("UDP_SERVERIP", "") : _remoteServerIp;
            }
            set
            {
                _remoteServerIp = value;
            }
        }

        public int RemoteServerPort
        {
            get
            {
                return (_remoteServerPort == 0) ? (int)util.LoadProjectResource("UDP_SERVERPORT", "") : _remoteServerPort;
            }
            set
            {
                _remoteServerPort = value;
            }
        }

        public int ServerPort
        {
            get
            {
                return (_serverPort == 0) ? int.Parse((string)util.LoadProjectResource("DEFAULT_CDR_PORT", "COMMONRES", "")) : _serverPort;
            }
            set
            {
                _serverPort = value;
            }
        }

        public string From_Ext
        {
            get
            {
                return _from_ext;
            }
            set
            {
                _from_ext = value;
            }
        }

        public string To_Ext
        {
            get
            {
                return _to_ext;
            }
            set
            {
                _to_ext = value;
            }
        }

        public string UserId
        {
            get
            {
                return _userid;
            }
            set
            {
                _userid = value;
            }
        }

        public MsgKinds SocketMsgKinds
        {
            get
            {
                return _msgkind;
            }
            set
            {
                _msgkind = value;
            }
        }

        public bool IsConnected
        {
            get
            {
                return _IsConnected;
            }
            set
            {
                _IsConnected = value;
            }
        }
        #endregion 속성

        public HUDPClient() { }
        public HUDPClient(int _srvPort)
        {
            ServerPort = _srvPort;
        }

        public HUDPClient(string _srvIP, int _srvPort)
        {
            RemoteServerIP = _srvIP;
            RemoteServerPort = _srvPort;
        }

        public IPEndPoint localSrvIEP = null;
        public IPEndPoint remoteCLIPE = null;
        public void StartServer()
        {
            localSrvIEP = new IPEndPoint(IPAddress.Any, ServerPort);
            udpclient = new System.Net.Sockets.UdpClient(localSrvIEP);

            _IsStarted = true;

            SockThread = new Thread(new ThreadStart(SendReceiveMsg));
            SockThread.IsBackground = true;
            SockThread.Start();
        }

        public void StartClient()
        {
            udpclient = new System.Net.Sockets.UdpClient();

            _IsStarted = true;

            SockThread = new Thread(new ThreadStart(SendReceiveMsg));
            SockThread.IsBackground = true;
            SockThread.Start();
        }

        public void Connect()
        {
            if (!IsConnected)
            {
                udpclient.Connect(remoteCLIPE);
                _IsConnected = true;
            }
        }

        public void Stop()
        {
            _IsStarted = false;

            SockThread.Join();
            udpclient = null;
        }

        public void Close()
        {
            udpclient.Close();
            _IsConnected = false;
        }

        public void Dispose()
        {
            udpclient.Close();
            udpclient = null;
        }

        private Object msg;
        private Object rcvMsg;
        public void Send(byte st)
        {
            msg = GetCommandMsg(st);
            byte[] bytes = util.GetBytes(msg);

            try
            {
                udpclient.Send(bytes, bytes.Length);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                throw ex;
            }
        }

        public void Send(byte st, string toext)
        {
            msg = GetCommandMsg(st, toext);
            byte[] bytes = util.GetBytes(msg);

            try
            {
                udpclient.Send(bytes, bytes.Length);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                throw ex;
            }
        }

        public void Send(byte st, MsgKinds msgkinds, Object revMsg)
        {
            switch (msgkinds)
            {
                case MsgKinds.CommandMessage:
                    msg = GetCommandMsg(st);
                    break;
                case MsgKinds.SMSMessage:
                    break;
                case MsgKinds.GroupWareMessage:
                    break;
                case MsgKinds.CdrResponse:
                    msg = GetMessage(st, MsgKinds.CdrResponse, revMsg);
                    break;
                default:
                    msg = GetCommandMsg(st);
                    break;
            }

            byte[] bytes = util.GetBytes(msg);

            try
            {
                udpclient.Send(bytes, bytes.Length, remoteCLIPE);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                throw ex;
            }
        }

        private void SendReceiveMsg()
        {
            try
            {
                remoteCLIPE = new IPEndPoint(IPAddress.Any, 0);

                while (_IsStarted)
                {
                    byte[] rcvBuffer = null;
                    rcvBuffer = udpclient.Receive(ref remoteCLIPE);

                    if (rcvBuffer.Length > 0)
                    {
                        if (UDPClientEventReceiveMessage != null)
                            UDPClientEventReceiveMessage(this, rcvBuffer);
                    }

                    Thread.Sleep(0);
                }
            }
            catch (System.Net.Sockets.SocketException se)
            {
                this.Stop();
                MessageBox.Show(se.ErrorCode + " : " + se.Message);
                //throw se;
            }
        }

        public CommandMsg GetCommandMsg(byte st)
        {
            CommandMsg rtnMsg = new CommandMsg();

            switch (st)
            {
                case SocketStruct.REGISTER_REQ:
                    rtnMsg.cmd = SocketStruct.REGISTER_REQ;
                    rtnMsg.type = SocketStruct.TYPE_COUPLEPHONE;
                    rtnMsg.direct = SocketStruct.DIRECT_OUTGOING;
                    rtnMsg.from_ext = UserId;
                    //rtnMsg.to_ext = to_ext;
                    rtnMsg.userid = string.Empty;
                    break;
                case SocketStruct.UNREGISTER_REQ:
                    rtnMsg.cmd = SocketStruct.UNREGISTER_REQ;
                    rtnMsg.type = SocketStruct.TYPE_COUPLEPHONE;
                    rtnMsg.direct = SocketStruct.DIRECT_OUTGOING;
                    rtnMsg.from_ext = UserId;
                    //rtnMsg.to_ext = to_ext;
                    rtnMsg.userid = string.Empty;
                    break;
                case SocketStruct.PICKUP_CALL_REQ:
                    break;
                case SocketStruct.MAKE_CALL_REQ:
                    rtnMsg.cmd = SocketStruct.MAKE_CALL_REQ;
                    rtnMsg.type = SocketStruct.TYPE_COUPLEPHONE;
                    rtnMsg.from_ext = From_Ext;
                    rtnMsg.to_ext = To_Ext;
                    rtnMsg.direct = SocketStruct.DIRECT_OUTGOING;
                    break;
            }

            return rtnMsg;
        }

        public CommandMsg GetCommandMsg(byte st, string toext)
        {
            CommandMsg rtnMsg = new CommandMsg();

            switch (st)
            {
                case SocketStruct.REGISTER_REQ:
                    rtnMsg.cmd = SocketStruct.REGISTER_REQ;
                    rtnMsg.type = SocketStruct.TYPE_COUPLEPHONE;
                    rtnMsg.direct = SocketStruct.DIRECT_OUTGOING;
                    rtnMsg.from_ext = UserId;
                    //rtnMsg.to_ext = to_ext;
                    rtnMsg.userid = string.Empty;
                    break;
                case SocketStruct.UNREGISTER_REQ:
                    rtnMsg.cmd = SocketStruct.UNREGISTER_REQ;
                    rtnMsg.type = SocketStruct.TYPE_COUPLEPHONE;
                    rtnMsg.direct = SocketStruct.DIRECT_OUTGOING;
                    rtnMsg.from_ext = UserId;
                    //rtnMsg.to_ext = to_ext;
                    rtnMsg.userid = string.Empty;
                    break;
                case SocketStruct.PICKUP_CALL_REQ:
                    break;
                case SocketStruct.MAKE_CALL_REQ:
                    rtnMsg.cmd = SocketStruct.MAKE_CALL_REQ;
                    rtnMsg.type = SocketStruct.TYPE_COUPLEPHONE;
                    rtnMsg.from_ext = UserId;
                    rtnMsg.to_ext = toext;
                    rtnMsg.direct = SocketStruct.DIRECT_OUTGOING;
                    break;
            }

            return rtnMsg;
        }

        public Object GetMessage(byte st, MsgKinds msgkinds, Object rsMsg)
        {
            Object rtnMsg = null;

            switch (msgkinds)
            {
                case MsgKinds.CommandMessage:
                    break;
                case MsgKinds.GroupWareMessage:
                    break;
                case MsgKinds.SMSMessage:
                    break;
                case MsgKinds.CdrRequest:
                    break;
                case MsgKinds.CdrResponse:
                    CdrRequest_t cdr_req = (CdrRequest_t)rsMsg;
                    CdrResponse_t cdr_res = new CdrResponse_t();
                    cdr_res.cmd = 2;
                    cdr_res.pCdr = cdr_req.pCdr;
                    cdr_res.status = 0;
                    rtnMsg = cdr_res;
                    break;
                case MsgKinds.CdrList:
                    break;
                default:
                    break;
            }

            return rtnMsg;
        }

    }
}
