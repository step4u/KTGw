using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Huen.Libs
{
    public class ApplicationEnums
    {

    }

    // 레지스트리 종류
    public enum RegKind
    {
        LocalMachine,
        CurrentUser
    }

    // AgentWindow Addressbook ContextMenu Behavior
    public enum AddrBookContextBehavior
    {
        Add,
        Modify
    }

    public enum DbKinds
    {
        MsSql,
        Oracle,
        MySql,
        Mdb
    }

    public enum MsgKinds
    {
        CommandMessage,
        GroupWareMessage,
        SMSMessage,
        CdrRequest,
        CdrResponse,
        CdrList,
        RecordInfo
    }

    public enum CallStatus
    {
        None,
        Ringing,
        Connected,
        Terminated
    }

    public enum UDPKinds
    {
        Client,
        Server
    }

    public enum InterceptorStatus
    {
        None,
        InnertelStatusReq,
        RtpReq,
        FileListenReq,
        FileTransferReq,
    }
}
