using System;
using System.Collections.Generic;
using NAudio.Wave;
using Com.Huen.Sockets;

namespace Com.Huen.DataModel
{
    public class RcvData : IDisposable
    {
        //public long seqidx = 0;
        public int isExtension = 0;
        public string extension = string.Empty;
        public string peernumber = string.Empty;
        //public int codec = 0;
        public long seqnum = 0;
        public int size = 0;
        public byte[] buffers = null;

        public void Dispose()
        {
            isExtension = 0;
            extension = null;
            peernumber = null;
            seqnum = 0;
            size = 0;
            buffers = null;
        }
    }
}
