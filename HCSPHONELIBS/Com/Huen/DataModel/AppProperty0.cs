using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;

namespace Com.Huen.DataModel
{
    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct AppProperty0
    {
        public Int32 popKind;
        public Int32 feeDay;
        public bool useSlideAlarm;
        public bool useStartAlarmList;
        public GridLength alarmListColDef0;
        public GridLength alarmListColDef1;
    }
}
