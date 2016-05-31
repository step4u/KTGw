using Com.Huen.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using WinSound;
using NAudio.Wave;
using System.IO;

namespace Com.Huen.DataModel
{
    public delegate void EndOfRtpEventHandler(object sender, EventArgs e);

    public class RTPRecInfo : IDisposable
    {
        public event EndOfRtpEventHandler EndOfRtpEvent;

        private WaveFileWriter writer = null;
        private WaveFormat pcmFormat16 = new WaveFormat(8000, 16, 1);
        private WaveFormat pcmFormat8 = new WaveFormat(8000, 8, 1);

        private IWavePlayer waveOut;

        public string ext = string.Empty;
        public string peer = string.Empty;
        public WaveFormat codec;
        public string savepath = string.Empty;
        public string filename = string.Empty;

        private Timer endtimer;
        private int endcount = 0;

        public RTPRecInfo(RecordInfo_t recInfo, string savepath, string filename)
        {
            if (!Directory.Exists(savepath)) Directory.CreateDirectory(savepath);

            waveOut = new DirectSoundOut(300);

            RTPPacket _rtp = new RTPPacket();
            this.codec = GetWaveFormat(_rtp.PayloadType);
            this.Add(recInfo);

            DateTime now = DateTime.Now;
            TimeSpan ts = now - (new DateTime(1970, 1, 1, 0, 0, 0, 0));
            this.savepath = savepath;
            this.filename = filename;

            //writer = new WaveFileWriter(string.Format(@"{0}\{1}", savepath, filename), pcmFormat8);
            //writer = new WaveFileWriter(string.Format(@"{0}\{1}", savepath, filename), this.codec);

            this.InitTimer();
        }

        BufferedStream bws;
        public void Add(RecordInfo_t recInfo)
        {
            if (recInfo.size == 0) this.endcount++;

            RTPPacket _rtp = new RTPPacket(this.GetRtpPayload(recInfo));

            MemoryStream memstrem = new MemoryStream(_rtp.Data);
            RawSourceWaveStream rawsrcstream = new RawSourceWaveStream(memstrem, this.codec);
            WaveStream wavestream = WaveFormatConversionStream.CreatePcmStream(rawsrcstream);

            byte[] output = new byte[wavestream.Length];
            wavestream.Read(output, 0, output.Length);
            var convStm = new WaveFormatConversionStream(pcmFormat8, rawsrcstream);

            waveOut.Init(convStm);
            waveOut.Play();


            // string msg = string.Format("seq:{0}, ext:{1}, peer:{2}, isExtension:{3}, size:{4}, bytesLength:{5}, codec:{6}", obj.seq, obj.extension, obj.peer_number, obj.isExtension, obj.size - 12, obj.voice.Length, obj.codec);
            // util.WriteLogTest3(msg, this.filename);

            //if (endcount > 1)
            //{
            //    if (endtimer != null)
            //    {
            //        endtimer.Enabled = false;
            //        endtimer = null;
            //    }

            //    if (EndOfRtpEvent != null)
            //        EndOfRtpEvent(this, new EventArgs());

            //    return;
            //}

            //if (endtimer != null)
            //{
            //    endtimer.Enabled = false;
            //    endtimer.Enabled = true;
            //}
        }

        private void InitTimer()
        {
            endtimer = new Timer();
            endtimer.Interval = 7000;
            endtimer.Enabled = true;
            endtimer.Elapsed += Endtimer_Elapsed;
            endtimer.Start();
        }

        private void Endtimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (EndOfRtpEvent != null)
                EndOfRtpEvent(this, new EventArgs());
        }

        private byte[] GetRtpPayload(RecordInfo_t recInfo)
        {
            byte[] rtpPayload = new byte[recInfo.size];
            Array.Copy(recInfo.voice, 0, rtpPayload, 0, rtpPayload.Length);

            return rtpPayload;
        }

        private WaveFormat GetWaveFormat(int type)
        {
            WaveFormat wavformat = null;

            switch (type)
            {
                case 0:
                    //wavformat = WaveFormat.CreateMuLawFormat(8000, 1);
                    //break;
                case 8:
                    wavformat = WaveFormat.CreateALawFormat(8000, 1);
                    break;
                case 4:
                    wavformat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.G723, 8000, 1, 8000, 1, 8);
                    break;
                case 18:
                    wavformat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.G729, 8000, 1, 8000, 1, 8);
                    break;
                default:
                    wavformat = WaveFormat.CreateALawFormat(8000, 1);
                    break;
            }

            return wavformat;
        }

        public void Dispose()
        {
            
        }
    }
}
