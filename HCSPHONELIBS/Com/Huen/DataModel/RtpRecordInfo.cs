using System;
using System.Collections.Generic;
using System.Linq;

using System.Timers;
using System.IO;

using NAudio.Wave;
using Com.Huen.Sockets;
using Com.Huen.Libs;

namespace Com.Huen.DataModel
{
    public delegate void EndOfRtpStreamEventHandler(object sender, EventArgs e);
    public class RtpRecordInfo : IDisposable
    {
        public event EndOfRtpStreamEventHandler EndOfRtpStreamEvent;

        private Timer timer;
        private Timer endtimer;
        private short endcount = 0;
        public double idx = 0.0d;
        public string ext = string.Empty;
        public string peer = string.Empty;
        public WaveFormat codec;
        public string savepath = string.Empty;
        public string filename = string.Empty;

        private List<ReceivedRtp> listIn;
        private List<ReceivedRtp> listOut;
        private WaveFileWriter writer = null;
        private WaveFormat pcmFormat16 = new WaveFormat(8000, 16, 1);
        private WaveFormat pcmFormat8 = new WaveFormat(8000, 8, 1);

        //public RtpRecordInfo() : this (WaveFormat.CreateMuLawFormat(8000, 1), "", "")
        //{
        //}

        public RtpRecordInfo(WaveFormat _codec, string savepath, string filename)
        {
            DateTime now = DateTime.Now;
            TimeSpan ts = now - (new DateTime(1970, 1, 1, 0, 0, 0, 0));
            this.idx = ts.TotalMilliseconds;
            this.codec = _codec;
            this.savepath = savepath;
            this.filename = filename;
            listIn = new List<ReceivedRtp>();
            listOut = new List<ReceivedRtp>();

            writer = new WaveFileWriter(string.Format(@"{0}\{1}", savepath, filename), pcmFormat8);
            this.InitTimer();
        }

        private void InitTimer()
        {
            timer = new Timer();
            timer.Interval = 3000;
            timer.Enabled = true;
            timer.Elapsed += timer_Elapsed;
            timer.Start();

            endtimer = new Timer();
            endtimer.Interval = 7000;
            endtimer.Enabled = true;
            endtimer.Elapsed += endtimer_Elapsed;
            endtimer.Start();
        }

        void endtimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (timer != null)
            {
                timer.Enabled = false;
                timer.Close();
            }

            this.MixRtp(MixType.FINAL);

            if (EndOfRtpStreamEvent != null)
                EndOfRtpStreamEvent(this, new EventArgs());
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.MixRtp(MixType.NORMAL);
        }

        public long chkcount = 0;
        public int firstIsExtension = -1;
        public void Add(RecordInfo_t obj)
        {
            // string msg = string.Format("seq:{0}, ext:{1}, peer:{2}, isExtension:{3}, size:{4}, codec:{5}", obj.seq, obj.extension, obj.peer_number, obj.isExtension, obj.size - 12, obj.codec);
            // util.WriteLogTest3(msg, this.filename);

            if (obj.size == 0)
                endcount++;

            if (endcount > 1)
            {
                if (timer != null)
                {
                    timer.Enabled = false;
                    timer = null;
                }

                if (endtimer != null)
                {
                    endtimer.Enabled = false;
                    endtimer = null;
                }

                System.Threading.Thread.Sleep(3000);

                this.MixRtp(MixType.FINAL);

                if (EndOfRtpStreamEvent != null)
                    EndOfRtpStreamEvent(this, new EventArgs());

                return;
            }

            if (endtimer != null)
            {
                endtimer.Enabled = false;
                endtimer.Enabled = true;
            }

            if (obj.size == 0) return;

            if (obj.isExtension == 1)
            {
                lock (listIn)
                {
                    listIn.Add(new ReceivedRtp() { seq = obj.seq, size = obj.size, isExtension = obj.isExtension, ext = obj.extension, peer = obj.peer_number, buff = obj.voice });
                }
            }
            else
            {
                lock (listOut)
                {
                    listOut.Add(new ReceivedRtp() { seq = obj.seq, size = obj.size, isExtension = obj.isExtension, ext = obj.extension, peer = obj.peer_number, buff = obj.voice });
                }
            }
        }

        private void MixRtp(MixType mixtype)
        {
            if (timer != null) timer.Enabled = false;

            if (listIn == null || listOut == null) return;

            List<ReceivedRtp> linin = null;
            List<ReceivedRtp> linout = null;

            lock (listIn)
            {
                linin = new List<ReceivedRtp>(listIn);
            }

            lock (listOut)
            {
                linout = new List<ReceivedRtp>(listOut);
            }

            //Com.Huen.Libs.SortRtpSeq sorting = new Com.Huen.Libs.SortRtpSeq();
            //linin.Sort(sorting);
            //linout.Sort(sorting);
            linin.OrderBy(x => x.seq);
            linout.OrderBy(x => x.seq);

            var itemIn = linin.FirstOrDefault();
            var itemOut = linout.FirstOrDefault();

            DelayedMil _delayedms = DelayedMil.same;
            if (itemIn == null || itemOut == null)
            {
                return;
            }
            else
            {
                byte[] mixedbytes = null;
                float times = 0.8f;
                if ((itemIn.size - headersize) == 80 && (itemOut.size - headersize) == 160)
                {
                    _delayedms = DelayedMil.i80o160;

                    float xtimes = (float)linin.Count / (float)(linout.Count * 2);
                    int _count = 0;
                    if (xtimes >= 1)
                    {
                        _count = mixtype == MixType.NORMAL ? (int)(linout.Count * times) : linout.Count;
                    }
                    else
                    {
                        _count = mixtype == MixType.NORMAL ? (int)((((float)linout.Count) * xtimes) * times) : (int)(((float)linout.Count) * xtimes);
                    }

                    for (int i = 0; i < _count; i++)
                    {
                        mixedbytes = this.Mixing(linin, linout[i], _delayedms);
                        this.WaveFileWriting(mixedbytes);
                    }
                }
                else if ((itemIn.size - headersize) == 160 && (itemOut.size - headersize) == 80)
                {
                    _delayedms = DelayedMil.i160o80;

                    float xtimes = (float)linout.Count / (float)(linin.Count * 2);
                    int _count = 0;
                    if (xtimes >= 1)
                    {
                        _count = mixtype == MixType.NORMAL ? (int)(linout.Count * times) : linout.Count;
                    }
                    else
                    {
                        _count = mixtype == MixType.NORMAL ? (int)((((float)linout.Count) * xtimes) * times) : (int)(((float)linout.Count) * xtimes);
                    }

                    for (int i = 0; i < _count; i++)
                    {
                        mixedbytes = this.Mixing(linout, linin[i], _delayedms);
                        this.WaveFileWriting(mixedbytes);
                    }
                }
                else if ((itemIn.size - headersize) == 80 && (itemOut.size - headersize) == 240)
                {
                    _delayedms = DelayedMil.i80o240;

                    float xtimes = (float)linin.Count / (float)(linout.Count * 3);
                    int _count = 0;
                    if (xtimes >= 1)
                    {
                        _count = mixtype == MixType.NORMAL ? (int)(linout.Count * times) : linout.Count;
                    }
                    else
                    {
                        _count = mixtype == MixType.NORMAL ? (int)((((float)linout.Count) * xtimes) * times) : (int)(((float)linout.Count) * xtimes);
                    }

                    for (int i = 0; i < _count; i++)
                    {
                        mixedbytes = this.Mixing(linin, linout[i], _delayedms);
                        this.WaveFileWriting(mixedbytes);
                    }
                }
                else if ((itemIn.size - headersize) == 240 && (itemOut.size - headersize) == 80)
                {
                    _delayedms = DelayedMil.i240o80;

                    float xtimes = (float)linout.Count / (float)(linin.Count * 3);
                    int _count = 0;
                    if (xtimes >= 1)
                    {
                        _count = mixtype == MixType.NORMAL ? (int)(linout.Count * times) : linout.Count;
                    }
                    else
                    {
                        _count = mixtype == MixType.NORMAL ? (int)((((float)linout.Count) * xtimes) * times) : (int)(((float)linout.Count) * xtimes);
                    }

                    for (int i = 0; i < _count; i++)
                    {
                        mixedbytes = this.Mixing(linout, linin[i], _delayedms);
                        this.WaveFileWriting(mixedbytes);
                    }
                }
                else if (itemIn.size == itemOut.size)
                {
                    _delayedms = DelayedMil.same;

                    float xtimes = (float)linin.Count / (float)linout.Count;
                    int _count = 0;
                    if (xtimes >= 1)
                    {
                        _count = mixtype == MixType.NORMAL ? (int)(linout.Count * times) : linout.Count;
                    }
                    else
                    {
                        _count = mixtype == MixType.NORMAL ? (int)((((float)linout.Count) * xtimes) * times) : (int)(((float)linout.Count) * xtimes);
                    }

                    for (int i = 0; i < _count; i++)
                    {
                        mixedbytes = this.Mixing(linin, linout[i], _delayedms);
                        this.WaveFileWriting(mixedbytes);
                    }
                }
            }

            if (timer != null) timer.Enabled = true;
        }

        private int headersize = 12;
        private byte[] Mixing(List<ReceivedRtp> list, ReceivedRtp baseitem, DelayedMil _delayedms)
        {
            byte[] mixedbytes = null;

            if (_delayedms == DelayedMil.i80o160)
            {
                int nseq = baseitem.seq * 2;
                ReceivedRtp _item0 = list.FirstOrDefault(x => x.seq == nseq);
                ReceivedRtp _item1 = list.FirstOrDefault(x => x.seq == nseq + 1);

                if (_item0 == null)
                {
                    _item0 = new ReceivedRtp() { buff = new byte[332], seq = nseq, size = 92, ext = baseitem.ext, peer = baseitem.peer };
                }

                if (_item1 == null)
                {
                    _item1 = new ReceivedRtp() { buff = new byte[332], seq = nseq + 1, size = 92, ext = baseitem.ext, peer = baseitem.peer };
                }

                // item2 + tmpitem mix with item1 and write
                byte[] tmpbuff = new byte[332];
                Array.Copy(_item0.buff, 0, tmpbuff, 0, _item0.size);
                Array.Copy(_item1.buff, headersize, tmpbuff, _item0.size, (_item1.size - headersize));
                ReceivedRtp _itm = new ReceivedRtp() { buff = tmpbuff, size = (_item0.size + _item1.size - headersize) };
                mixedbytes = this.RealMix(_itm, baseitem);

                // util.WriteLogTest3(_delayedms.ToString() + " // codec: " + _item0.codec.ToString() + " // " + (_item0.size - 12) + " // basecodec: " + baseitem.codec + " // " + (baseitem.size - 12) + " // " + baseitem.seq + " // " + _item0.seq + " // " + _item1.seq, this.filename);

                lock (listIn)
                {
                    listIn.RemoveAll(x => x.seq == _item0.seq);
                    listIn.RemoveAll(x => x.seq == _item1.seq);
                }

                lock (listOut)
                {
                    listOut.RemoveAll(x => x.seq == baseitem.seq);
                }
            }
            else if (_delayedms == DelayedMil.i160o80)
            {
                int nseq = baseitem.seq * 2;
                ReceivedRtp _item0 = list.FirstOrDefault(x => x.seq == nseq);
                ReceivedRtp _item1 = list.FirstOrDefault(x => x.seq == nseq + 1);

                if (_item0 == null)
                {
                    _item0 = new ReceivedRtp() { buff = new byte[332], seq = nseq, size = 92, ext = baseitem.ext, peer = baseitem.peer };
                }

                if (_item1 == null)
                {
                    _item1 = new ReceivedRtp() { buff = new byte[332], seq = nseq + 1, size = 92, ext = baseitem.ext, peer = baseitem.peer };
                }

                // item2 + tmpitem mix with item1 and write
                byte[] tmpbuff = new byte[332];
                Array.Copy(_item0.buff, 0, tmpbuff, 0, _item0.size);
                Array.Copy(_item1.buff, headersize, tmpbuff, _item0.size, (_item1.size - headersize));
                ReceivedRtp _itm = new ReceivedRtp() { buff = tmpbuff, size = (_item0.size + _item1.size - headersize) };
                mixedbytes = this.RealMix(_itm, baseitem);

                // util.WriteLogTest3(_delayedms.ToString() + " // " + (baseitem.size - 12) + " // " + baseitem.seq + " // " + _item0.seq + " // " + _item1.seq, this.filename);

                lock (listIn)
                {
                    listIn.RemoveAll(x => x.seq == baseitem.seq);
                }

                lock (listOut)
                {
                    listOut.RemoveAll(x => x.seq == _item0.seq);
                    listOut.RemoveAll(x => x.seq == _item1.seq);
                }
            }
            else if (_delayedms == DelayedMil.i80o240)
            {
                int nseq = baseitem.seq * 3;
                ReceivedRtp _item0 = list.FirstOrDefault(x => x.seq == nseq);
                ReceivedRtp _item1 = list.FirstOrDefault(x => x.seq == nseq + 1);
                ReceivedRtp _item2 = list.FirstOrDefault(x => x.seq == nseq + 2);

                if (_item0 == null)
                {
                    _item0 = new ReceivedRtp() { buff = new byte[332], seq = nseq, size = 92, ext = baseitem.ext, peer = baseitem.peer };
                }

                if (_item1 == null)
                {
                    _item1 = new ReceivedRtp() { buff = new byte[332], seq = nseq + 1, size = 92, ext = baseitem.ext, peer = baseitem.peer };
                }

                if (_item2 == null)
                {
                    _item2 = new ReceivedRtp() { buff = new byte[332], seq = nseq + 2, size = 92, ext = baseitem.ext, peer = baseitem.peer };
                }

                // item2 + tmpitem mix with item1 and write
                byte[] tmpbuff = new byte[332];
                Array.Copy(_item0.buff, 0, tmpbuff, 0, _item0.size);
                Array.Copy(_item1.buff, headersize, tmpbuff, _item0.size, _item1.size - headersize);
                Array.Copy(_item2.buff, headersize, tmpbuff, _item0.size + _item0.size - headersize, _item2.size - headersize);
                ReceivedRtp _itm = new ReceivedRtp() { buff = tmpbuff, size = _item0.size + _item1.size + _item2.size - headersize };
                mixedbytes = this.RealMix(_itm, baseitem);

                // util.WriteLogTest3(_delayedms.ToString() + " // " + (baseitem.size - 12) + " // " + baseitem.seq + " // " + _item0.seq + " // " + _item1.seq + " // " + _item2.seq, this.filename);

                lock (listIn)
                {
                    listIn.RemoveAll(x => x.seq == _item0.seq);
                    listIn.RemoveAll(x => x.seq == _item1.seq);
                    listIn.RemoveAll(x => x.seq == _item2.seq);
                }

                lock (listOut)
                {
                    listOut.RemoveAll(x => x.seq == baseitem.seq);
                }
            }
            else if (_delayedms == DelayedMil. i240o80)
            {
                int nseq = baseitem.seq * 3;
                ReceivedRtp _item0 = list.FirstOrDefault(x => x.seq == nseq);
                ReceivedRtp _item1 = list.FirstOrDefault(x => x.seq == nseq + 1);
                ReceivedRtp _item2 = list.FirstOrDefault(x => x.seq == nseq + 2);

                if (_item0 == null)
                {
                    _item0 = new ReceivedRtp() { buff = new byte[332], seq = nseq, size = 92, ext = baseitem.ext, peer = baseitem.peer };
                }

                if (_item1 == null)
                {
                    _item1 = new ReceivedRtp() { buff = new byte[332], seq = nseq + 1, size = 92, ext = baseitem.ext, peer = baseitem.peer };
                }

                if (_item2 == null)
                {
                    _item2 = new ReceivedRtp() { buff = new byte[332], seq = nseq + 2, size = 92, ext = baseitem.ext, peer = baseitem.peer };
                }

                // util.WriteLogTest3(_delayedms.ToString() + " // " + (baseitem.size - 12) + " // " + baseitem.seq + " // " + _item0.seq + " // " + _item1.seq + " // " + _item2.seq, this.filename);

                // item2 + tmpitem mix with item1 and write
                byte[] tmpbuff = new byte[332];
                Array.Copy(_item0.buff, 0, tmpbuff, 0, _item0.size);
                Array.Copy(_item1.buff, headersize, tmpbuff, _item0.size, _item1.size - headersize);
                Array.Copy(_item1.buff, headersize, tmpbuff, _item0.size + _item1.size - headersize, _item2.size - headersize);
                ReceivedRtp _itm = new ReceivedRtp() { buff = tmpbuff, size = _item0.size + _item1.size + _item2.size - headersize };
                mixedbytes = this.RealMix(_itm, baseitem);

                lock (listIn)
                {
                    listIn.RemoveAll(x => x.seq == baseitem.seq);
                }

                lock (listOut)
                {
                    listOut.RemoveAll(x => x.seq == _item0.seq);
                    listOut.RemoveAll(x => x.seq == _item1.seq);
                    listOut.RemoveAll(x => x.seq == _item2.seq);
                }
            }
            else if (_delayedms == DelayedMil.same)
            {
                ReceivedRtp _item = list.FirstOrDefault(x => x.seq == baseitem.seq);
                if (_item == null)
                {
                    _item = new ReceivedRtp() { buff = new byte[332], seq = baseitem.seq, size = baseitem.size, ext = baseitem.ext, peer = baseitem.peer };
                }
                mixedbytes = this.RealMix(_item, baseitem);

                // util.WriteLogTest3(_delayedms.ToString() + " // " + (baseitem.size - 12) + " // " + baseitem.seq + " // " + _item.seq, this.filename);

                lock (listIn)
                {
                    listIn.RemoveAll(x => x.seq == _item.seq);
                }

                lock (listOut)
                {
                    listOut.RemoveAll(x => x.seq == baseitem.seq);
                }
            }

            return mixedbytes;
        }

        private byte[] RealMix(ReceivedRtp item1, ReceivedRtp item2)
        {
            if (item1 == null || item2 == null) return null;

            if (item1.size == 0 || item2.size == 0) return null;

            byte[] wavSrc1 = new byte[item1.size - headersize];
            byte[] wavSrc2 = new byte[item2.size - headersize];

            Array.Copy(item1.buff, headersize, wavSrc1, 0, (item1.size - headersize));
            Array.Copy(item2.buff, headersize, wavSrc2, 0, (item2.size - headersize));

            WaveMixerStream32 mixer = new WaveMixerStream32();
            // mixer.AutoStop = true;
            MemoryStream memstrem = new MemoryStream(wavSrc1);
            RawSourceWaveStream rawsrcstream = new RawSourceWaveStream(memstrem, this.codec);
            WaveFormatConversionStream conversionstream = new WaveFormatConversionStream(pcmFormat16, rawsrcstream);
            WaveChannel32 channelstream = new WaveChannel32(conversionstream);
            mixer.AddInputStream(channelstream);

            memstrem = new MemoryStream(wavSrc2);
            rawsrcstream = new RawSourceWaveStream(memstrem, this.codec);
            conversionstream = new WaveFormatConversionStream(pcmFormat16, rawsrcstream);
            channelstream = new WaveChannel32(conversionstream);
            mixer.AddInputStream(channelstream);
            mixer.Position = 0;

            Wave32To16Stream to16 = new Wave32To16Stream(mixer);
            var convStm = new WaveFormatConversionStream(pcmFormat8, to16);
            byte[] mixedbytes = new byte[(int)convStm.Length];
            int chk = convStm.Read(mixedbytes, 0, (int)convStm.Length);
            //Buffer.BlockCopy(tobyte, 0, writingBuffer, 0, tobyte.Length);

            memstrem.Close();
            rawsrcstream.Close();
            conversionstream.Close();
            channelstream.Close();

            convStm.Close(); convStm.Dispose(); convStm = null;
            to16.Close(); to16.Dispose(); to16 = null;
            mixer.Close(); mixer.Dispose(); mixer = null;

            return mixedbytes;
        }

        //private byte[] StereoToMono(byte[] input)
        //{
        //    byte[] output = new byte[input.Length / 2];
        //    int outputIndex = 0;
        //    for (int n = 0; n < input.Length; n += 4)
        //    {
        //        // copy in the first 16 bit sample
        //        output[outputIndex++] = input[n];
        //        output[outputIndex++] = input[n + 1];
        //    }
        //    return output;
        //}

        private void WaveFileWriting(byte[] buff)
        {
            if (buff == null) return;

            if (buff.Length == 0) return;

            //using (MemoryStream memStm = new MemoryStream(buff))
            //using (RawSourceWaveStream rawSrcStm = new RawSourceWaveStream(memStm, pcmFormat))
            //{
                //if (this.writer == null)
                //{
                //    this.writer = new WaveFileWriter(GetFullFN, pcmFormat);
                //}

                this.writer.Write(buff, 0, buff.Length);
                this.writer.Flush();
            //}
        }

        public void Dispose()
        {
            if (this.writer != null)
            {
                writer.Close();
                //writer = null;
                //writer.Dispose();
            }

            if (listIn != null)
            {
                listIn.Clear(); listIn = null;
            }

            if (listOut != null)
            {
                listOut.Clear(); listOut = null;
            }
        }

        private enum DelayedMil
        {
            i80o160 = 0,
            i160o80 = 1,
            i80o240 = 3,
            i240o80 = 4,
            same = 5
        }

        private enum MixType
        {
            NORMAL,
            FINAL
        }
    }
}
