using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.Windows;

namespace Com.Huen.DataModel
{
    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct CRInterceptOption
    {
        public double WinTop;
        public double WinLeft;
        public double WinWidth;
        public double WinHeight;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string ServerIP;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 200)]
        public string SaveDirectory;
        public bool UsingInnertelCol;
        public GridLength GridColDef0;
        public GridLength GridColDef1;
        public GridLength GridColDef2;
    }
}
