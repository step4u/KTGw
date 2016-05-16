using System;
using NAudio.Wave;

namespace Com.Huen.DataModel
{
    public class RecInfos
    {
        public RcvData rcvData;
        public int isExtension;
        public int seq;
        public int size;
        public byte[] voice;
    }

    public class RecIngData : IDisposable
    {
        public long seqidx = 0;
        public string extension = string.Empty;
        public string peernumber = string.Empty;
        public int codec = 0;
        public string savepath = string.Empty;
        public string wavefilename = string.Empty;
        public WaveFormat wavformat = WaveFormat.CreateMuLawFormat(8000, 1);
        public WaveFileWriter writer = null;
        public ushort endcount = 0;
        public long endtimer = 0;

        public RecIngData() { }

        public void Dispose()
        {
            seqidx = 0;
            extension = null;
            peernumber = null;
            codec = 0;
            savepath = null;
            wavefilename = null;
            wavformat = null;
            if (writer != null)
            {
                writer.Dispose();
            }
            endcount = 0;
            endtimer = 0;
        }
    }
}
