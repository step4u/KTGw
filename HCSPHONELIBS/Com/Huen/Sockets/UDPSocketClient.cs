using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

using Com.Huen.Libs;

namespace Com.Huen.Sockets
{
    public delegate void UDPSocketCommandEventHandler(object sender, CommandMsg e);

    public interface IUDPSocketClient
    {
        event UDPSocketCommandEventHandler UDPSocketReceiveCommandMessage;

        string ServerIp
        {
            get;
            set;
        }

        int ServerPort
        {
            get;
            set;
        }

        string From_Ext
        {
            get;
            set;
        }

        string To_Ext
        {
            get;
            set;
        }

        string UserId
        {
            get;
            set;
        }

        void Start();
        void Send(byte st);
    }

    public class UDPSocketClient : IUDPSocketClient
    {
        //public delegate void SocketCmdEventHandler(object sender, CommandMsg e);
        public event UDPSocketCommandEventHandler UDPSocketReceiveCommandMessage;

        private Socket s = null;
        private IPEndPoint _srvEndPoint;
        private EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        private CommandMsg sendMsg;
        private CommandMsg rcvMsg;

        // 서버 주소, 포트
        private string _server = string.Empty;
        private int _serverPort = 31001;
        private string _from_ext = string.Empty;
        private string _to_ext = string.Empty;
        private string _userid = string.Empty;

        public string ServerIp
        {
            get
            {
                return _server;
            }
            set
            {
                _server = value;
            }
        }

        public int ServerPort
        {
            get
            {
                return _serverPort;
            }
            set
            {
                _serverPort = value;
            }
        }

        public IPEndPoint ServerEndPoint
        {
            get
            {
                _srvEndPoint = new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort);
                return _srvEndPoint;
            }
            set
            {
                _srvEndPoint = value;
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

        private Thread SockThread;

        public UDPSocketClient() { }

        public UDPSocketClient(string srvIp, int srvPort)
        {
            this.ServerIp = srvIp;
            this.ServerPort = srvPort;
        }

        public void Start()
        {
            SockThread = new Thread(new ThreadStart(SendReceiveSocketMsg));
            SockThread.IsBackground = true;
            SockThread.Start();
        }

        public void Send(byte st)
        {
            try
            {
                CommandMsg msg = GetCmdMsg(st);
                byte[] buffer = util.getBytes(sendMsg);

                s.SendTo(buffer, ServerEndPoint);
            }
            catch(System.Net.Sockets.SocketException ex)
            {
                throw ex;
            }
        }

        public void Send(byte st, MsgKinds msgkinds)
        {
            Object msg = null;

            try
            {
                switch (msgkinds)
                {
                    case MsgKinds.CommandMessage:
                        msg = GetCmdMsg(st);
                        break;
                    case MsgKinds.GroupWareMessage:
                        break;
                    case MsgKinds.SMSMessage:
                        break;
                    default:
                        msg = GetCmdMsg(st);
                        break;
                }

                byte[] buffer = util.getBytes(msg);

                s.SendTo(buffer, ServerEndPoint);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                throw ex;
            }
        }

        private void SendReceiveSocketMsg()
        {
            try
            {
                ////srvEndPoint = new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort);
                s = new Socket(ServerEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

                //sendMsg = GetCmdMsg(SocketStruct.REGISTER_REQ);

                //byte[] buffer = SocketStruct.getBytes(sendMsg);

                ////s.SendTo(buffer, buffer.Length, SocketFlags.None, serverEndPoint);
                //s.SendTo(buffer, ServerEndPoint);

                int bytes = 0;

                rcvMsg = new CommandMsg();
                byte[] rcvBuffer = util.getBytes(rcvMsg);

                while (true)
                {
                    bytes = s.ReceiveFrom(rcvBuffer, ref remoteEndPoint);

                    if (bytes > 0)
                    {
                        rcvMsg = (CommandMsg)util.getObject(rcvBuffer);
                        //DoItFromSocketMessage(rcvMsg, sendMsg);
                        UDPSocketReceiveCommandMessage(this, rcvMsg);
                    }
                }
            }
            catch (SocketException se)
            {
                //MessageBox.Show(se.ErrorCode + " : " + se.Message);
            }

        }

        public CommandMsg GetCmdMsg(byte st)
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
    }
}
