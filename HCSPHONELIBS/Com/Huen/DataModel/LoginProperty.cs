using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Com.Huen.DataModel
{
    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct LoginProperty
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string U;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string P;
        public bool SaveID;
        public bool AutoLogin;
    }
}
