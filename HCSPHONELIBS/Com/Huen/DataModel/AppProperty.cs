using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;

namespace Com.Huen.DataModel
{
    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct AppProperty
    {
        public double AT;
        public double AL;
        public double AW;
        public double AH;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string ServerviceKind;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string ServerIp;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string UserTel;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string UserPwd;
        public bool useToolbar;
        public Int32 popKind;
        public Int32 feeDay;
        public bool useSlideAlarm;
        public bool useStartAlarmList;
        public GridLength alarmListColDef0;
        public GridLength alarmListColDef1;
    }
}
