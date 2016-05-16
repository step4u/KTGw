using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Huen.DataModel
{
    public class LGuMessage
    {
        public string EVENT = string.Empty;
        public string CALLERID = string.Empty;
        public string CALLER1ID = string.Empty;
        public string CALLER2ID = string.Empty;
        public string CHANNEL1 = string.Empty;
        public string CHANNEL2 = string.Empty;
        public string CHANNEL = string.Empty;
        public string RECHANNEL = string.Empty;
        public string ISDIAL = string.Empty;
        public string CMD = string.Empty;
        public string MSG = string.Empty;
        public string STATUS = string.Empty;
        public string INEXTEN = string.Empty;
        public string EXTEN = string.Empty;
        public string AGENT = string.Empty;
        public string ATXFER = string.Empty;
        public string UNIQUEID = string.Empty;
        public string UNIQUEID1 = string.Empty;
        public string UNIQUEID2 = string.Empty;
        public string SRCUNIQUEID = string.Empty;
        public string HCAUSE = string.Empty;

        //public LGuMessage() { }

        public LGuMessage(string msg)
        {
            var msgs = msg.Split('|');
            EVENT = msgs[0];

            for (int i = 1; i < msgs.Length; i++)
            {
                var keyval = msgs[i].Split(':');

                switch (keyval[0])
                {
                    case "CALLERID":
                        CALLERID = keyval[1];
                        break;
                    case "CALLER1ID":
                        CALLER1ID = keyval[1];
                        break;
                    case "CALLER2ID":
                        CALLER2ID = keyval[1];
                        break;
                    case "CHANNEL1":
                        CHANNEL1 = keyval[1];
                        break;
                    case "CHANNEL2":
                        CHANNEL2 = keyval[1];
                        break;
                    case "MSG":
                        MSG = keyval[1];
                        break;
                    case "STATUS":
                        STATUS = keyval[1];
                        break;
                    case "ISDIAL":
                        ISDIAL = keyval[1];
                        break;
                    case "CHANNEL":
                        CHANNEL = keyval[1];
                        break;
                    case "RECHANNEL":
                        RECHANNEL = keyval[1];
                        break;
                    case "EXTEN":
                        EXTEN = keyval[1];
                        break;
                    case "INEXTEN":
                        INEXTEN = keyval[1];
                        break;
                    case "AGENT":
                        AGENT = keyval[1];
                        break;
                    case "ATXFER":
                        ATXFER = keyval[1];
                        break;
                    case "UNIQUEID":
                        UNIQUEID = keyval[1];
                        break;
                    case "UNIQUEID1":
                        UNIQUEID1 = keyval[1];
                        break;
                    case "UNIQUEID2":
                        UNIQUEID2 = keyval[1];
                        break;
                    case "SRCUNIQUEID":
                        SRCUNIQUEID = keyval[1];
                        break;
                    case "HCAUSE":
                        HCAUSE = keyval[1];
                        break;
                }
            }
        }
    }

    public class LGuCommandResult
    {
        #region 속성
        private string _event = string.Empty;
        private string _cmd = string.Empty;
        private string _peer = string.Empty;
        public List<LGuPeerStatus> PeerStatus = new List<LGuPeerStatus>();

        public string EVENT
        {
            get
            {
                return _event;
            }
            set
            {
                _event = value;
            }
        }
        public string CMD
        {
            get
            {
                return _cmd;
            }
            set
            {
                _cmd = value;
            }
        }

        #endregion 속성 끝

        //public LGuCommandResult() { }

        public LGuCommandResult(string strResult)
        {
            var arrResult = strResult.Split('|');
            this.EVENT = arrResult[0];
            var arrCmd = arrResult[1].Split(':');
            this.CMD = arrCmd[1];

            for (int i = 2; i < arrResult.Length; i++)
            {
                PeerStatus.Add(GetPeerStatus(arrResult[i]));
            }
        }

        private LGuPeerStatus GetPeerStatus(string info)
        {
            var infos = info.Split(':');
            if (infos.Length == 3)
                return new LGuPeerStatus() { PEERNUMBER=infos[0], PEERINFO=infos[1], CURRENTSTATUS=infos[2] };
            else if (infos.Length == 1)
                return new LGuPeerStatus() { PEERNUMBER = infos[0] };
            else
                return new LGuPeerStatus() { PEERNUMBER = infos[0], PEERINFO = infos[1] };
        }
    }

    public class LGuPeerStatus
    {
        #region 속성
        public string PEERNUMBER = string.Empty;
        public string PEERINFO = string.Empty;
        public string CURRENTSTATUS = string.Empty;
        #endregion 속성 끝

        //public LGuPeerStatus() { }
    }

    public enum LGuStatusEnum
    {
        None,
        Ringing,
        Calling,
        Connected,
    }

    public enum LGuActionStatus
    {
        None,
        Inneroutbound,
        Outbound,
        Transfer,
    }

    public enum GroupAction
    {
        None,
        Create,
        Modify,
        Remove,
    }
}
