using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.Collections.ObjectModel;

using Com.Huen.DataModel;

namespace Com.Huen.Sockets
{
    public class SocketStruct
    {
        public SocketStruct() { }

        #region 소켓 변수
        // command struct cmd 상수 request
        public const byte REGISTER_REQ = 1;
        public const byte UNREGISTER_REQ = 2;
        public const byte MAKE_CALL_REQ = 3;
        public const byte PICKUP_CALL_REQ = 4;
        public const byte TRANSFER_CALL_REQ = 5;
        public const byte DROP_CALL_REQ = 6;
        public const byte HOLD_CALL_REQ = 34;
        public const byte ACTIVE_CALL_REQ = 36;

        // command struct cmd 상수 response
        public const byte CALL_STATUS = 10;
        public const byte REGISTER_RES = 11;
        public const byte UNREGISTER_RES = 12;
        public const byte MAKE_CALL_RES = 13;
        public const byte PICKUP_CALL_RES = 14;
        public const byte TRANSFER_CALL_RES = 15;
        public const byte DROP_CALL_RES = 16;
        public const byte HOLD_CALL_RES = 35;
        public const byte ACTIVE_CALL_RES = 37;

        // command struct direct 상수
        public const byte DIRECT_OUTGOING = 1;
        public const byte DIRECT_INCOMING = 2;

        // command struct type 상수
        public const byte TYPE_COUPLEPHONE = 1;
        //public const byte TYPE_GROUPWARE = 2;
        //public const byte TYPE_IPPHONE = 3;

        // command struct status 상수
        public const byte STATUS_SUCCESS = 0;
        public const byte STATUS_FAIL = 1;
        public const byte STATUS_NAT_SUCCESS = 99;

        public const byte STATUS_CALL_IDLE = 10;
        public const byte STATUS_CALL_INVITING = 11;
        public const byte STATUS_CALL_PROCEEDING = 12;
        public const byte STATUS_CALL_RINGING = 13;
        public const byte STATUS_CALL_CONNECTED = 15;
        public const byte STATUS_CALL_TERMINATED = 21;
        public const byte STATUS_CALL_NOT_FOUND = 24;
        public const byte STATUS_CALL_DND = 25;
        public const byte STATUS_CALL_NO_ANSWER = 26;
        public const byte STATUS_CALL_FORBIDDEN = 27;
        public const byte STATUS_CALL_BUSY = 28;
        public const byte STATUS_CALL_SYSTEM_ERROR = 29;
        #endregion 소켓 변수
    }

    #region 커플모드 컨트롤 Sructure

    // 커플모드 컨트롤 Structure
    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi, Size = 64)]
    public struct CommandMsg
    {
        public byte cmd;
        public byte direct;
        public byte type;
        public byte status;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string from_ext;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string to_ext;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string userid;
        public int ip;
        public int port;
        public int data_port;
    }

    // 커플모드 SMS Structure

    #endregion 커플모드 컨트롤 Sructure

    #region CDR 구조체 s

    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi, Size = 524)]
    public struct CdrRequest_t
    {
        public int cmd;
        public IntPtr pCdr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] data;
        public IntPtr next;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi, Size = 12)]
    public struct CdrResponse_t
    {
        public Int32 cmd;
        public IntPtr pCdr;
        public Int32 status;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 0, CharSet = CharSet.Ansi, Size = 512)]
    public struct CdrList
    {
        public int seq;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string office_name;
        public Int32 start_yyyy;
        public Int32 start_month;
        public Int32 start_day;
        public Int32 start_hour;
        public Int32 start_min;
        public Int32 start_sec;
        public Int32 end_yyyy;
        public Int32 end_month;
        public Int32 end_day;
        public Int32 end_hour;
        public Int32 end_min;
        public Int32 end_sec;
        public Int32 caller_type;
        public Int32 callee_type;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string caller;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string callee;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string caller_ipn_number;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string caller_group_code;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string caller_group_name;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string caller_human_name;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string callee_ipn_number;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string callee_group_code;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string callee_group_name;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string callee_human_name;
        public Int32 result;
        public IntPtr next;
    }
    #endregion CDR 구조체 e

    #region RecordInfo 구조체 s
    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 0, CharSet = CharSet.Ansi, Size = 416)]
    public struct RecordInfo_t
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string extension;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string peer_number;
        public Int32 isExtension;
        public Int32 codec;
        public Int32 seq;
        public Int32 rtp_seq;
        public Int32 yyyy;
        public Int32 month;
        public Int32 day;
        public Int32 hh;
        public Int32 mm;
        public Int32 sec;
        public Int32 msec;
        public Int32 offset;
        public Int32 size;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 332)]
        public byte[] voice;
        public IntPtr next;
    }
    #endregion RecordInfo 구조체 e

    #region RTP Record Intercept Request 구조체 s
    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi, Size = 9)]
    public struct InterceptReq
    {
        // 1 내선 전화 상태 요청
        // 2 RTP Redirect 요청
        // 3 RTP Redirect 해제 요청
        // 4 파일 Stream 요청
        // 5 파일 Down 요청
        public Int32 cmd;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string extnum;
    }
    #endregion RTP Record Intercept Request 구조체 e

    #region RTP Record Intercept Response 구조체 s
    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi, Size = 8)]
    public struct InterceptRes
    {
        // 1 내선 전화 상태 응답
        // 2 RTP Redirect 응답
        // 3 RTP Redirect 해제 응답
        // 4 파일 Stream 응답
        public Int32 cmd;
        // 1 각 응답 결과 Success
        // 0 각 응답 결과 Fail
        public Int32 result;
    }
    #endregion RTP Record Intercept Response 구조체 e

    #region RTP Recorder command class s
    [Serializable()]
    public class InterceptRequest
    {
        // 1 내선 전화 상태 요청
        // 2 RTP Redirect 요청
        // 3 RTP Redirect 해제 요청
        // 4 파일 Stream 요청
        // 5 파일 Down 요청
        public Int32 cmd;
        public string extnum;
    }

    [Serializable()]
    public class InterceptResponse
    {
        public Int32 cmd;
        public Int32 result;
        public ObservableCollection<InnerTel> innertels;
    }
    #endregion RTP Recorder command class e

    #region File transfer Request 구조체 s
    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi, Size = 267)]
    public struct FileTransferHeader
    {
        // Cmd 1: 파일 download, 2:파일 upload, 3: 파일 download 응답
        public Int32 Cmd;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string FileName;
        public long FileSize;
    }
    #endregion File transfer Request 구조체 e
}
