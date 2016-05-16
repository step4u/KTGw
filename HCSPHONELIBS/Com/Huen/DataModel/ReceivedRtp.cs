using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Huen.DataModel
{
    public class ReceivedRtp : IDisposable
    {
        public int isExtension = -1;
        public string ext = string.Empty;
        public string peer = string.Empty;
        public int seq = -1;
        public int size = -1;
        public byte[] buff = null;

        public void Dispose()
        {
            isExtension = -1;
            ext = string.Empty;
            peer = string.Empty;
            seq = -1;
            size = -1;
            buff = null;
        }
    }
}
