using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Diagnostics;

using Com.Huen.Libs;
using Com.Huen.DataModel;
using Com.Huen.Sockets;
using System.Windows.Threading;

using NAudio.Wave;

namespace CallInterceptor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket _s;
        private bool _IsSocketStarted = false;

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private EndPoint _localep;
        private EndPoint _serverep;
        private Thread _threadIntercept;
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.StartSocket();
        }

        private void StartSocket()
        {
            _localep = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                _serverep = new IPEndPoint(IPAddress.Parse("192.168.1.2"), 21020);

                _s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                _s.Connect(_serverep);
            }
            catch (SocketException se)
            {
                throw se;
            }

            _threadIntercept = new Thread(new ThreadStart(InterceptorReceiver));
            _threadIntercept.IsBackground = true;
            _threadIntercept.Start();

            _IsSocketStarted = true;
        }

        private InterceptorStatus _interceptStatus = InterceptorStatus.None;
        private InterceptReq _req;
        private InterceptRes _res;
        private RecordInfo_t _rtp;
        private void InterceptorReceiver()
        {
            while (_IsSocketStarted)
            {
                byte[] __buffer = null;

                if (_interceptStatus == InterceptorStatus.InnertelStatusReq)
                {
                    _rtp = new RecordInfo_t();
                    __buffer = util.GetBytes(_rtp);
                }
                else
                {
                    _res = new InterceptRes();
                    __buffer = util.GetBytes(_res);
                }

                int __count = 0;

                try
                {
                    __count = _s.ReceiveFrom(__buffer, SocketFlags.None, ref _localep);
                }
                catch (SocketException se)
                {
                    throw se;
                }

                if (__count == 0)
                    return;


                if (_interceptStatus == InterceptorStatus.None)
                {
                    _res = util.GetObject<InterceptRes>(__buffer);

                    if (_res.cmd == 3 && _res.result == 1)
                    {
                        _interceptStatus = InterceptorStatus.FileTransferReq;

                        if (playbackState == StreamingPlaybackState.Stopped)
                        {
                            playbackState = StreamingPlaybackState.Buffering;
                            this.bufferedWaveProvider = null;
                            ThreadPool.QueueUserWorkItem(new WaitCallback(_playTimer_Tick));
                        }
                        else if (playbackState == StreamingPlaybackState.Paused)
                        {
                            playbackState = StreamingPlaybackState.Buffering;
                        }
                    }
                }
                else
                {
                    _rtp = util.GetObject<RecordInfo_t>(__buffer);

                    int nDataSize = _rtp.size - 12;

                    if (nDataSize != 80 && nDataSize != 160 && nDataSize != 240 && nDataSize != -12) return;

                    //this.Rtp2Wav(recInfo, nDataSize);
                    this.Rtp2Wav2(_rtp, nDataSize);
                }

                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    statustxt0.Text = _interceptStatus.ToString();
                }));
            }
        }

        private void StopSocket()
        {
            _IsSocketStarted = false;
            _threadIntercept.Join();
            _s.Close();
        }

        #region 소팅, 믹싱, 플레이
        enum StreamingPlaybackState
        {
            Stopped,
            Playing,
            Buffering,
            Paused
        }

        private WaveFormat mulawFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.MuLaw, 8000, 1, 8000, 1, 8);
        private WaveFormat pcmFormat = new WaveFormat(8000, 16, 1);

        private string seqnum = string.Empty;
        private List<RcvData> rcvqueList = new List<RcvData>();
        private List<RecInfos> lExtension0 = new List<RecInfos>();
        private List<RecInfos> lExtension1 = new List<RecInfos>();

        public void Rtp2Wav2(RecordInfo_t recordInfo, int dataSize)
        {
            string fn = string.Empty;

            RcvData ingData = null;
            ingData = rcvqueList.Find(
                delegate(RcvData qlist)
                {
                    return qlist.extension == recordInfo.extension && qlist.peernumber == recordInfo.peer_number;
                });

            if (ingData == null)
            {
                TimeSpan __ts = DateTime.Now - new DateTime(1970, 1, 1);
                Int32 __seqnum = (Int32)__ts.TotalSeconds;

                ingData = new RcvData() { extension = recordInfo.extension, peernumber = recordInfo.peer_number, seqnum = __seqnum };
                rcvqueList.Add(ingData);
            }

            if (dataSize == -12)
            {
                // MP3로 저장할 경우 이 곳에서 마지막에 wav를 mp3로 변환

                ProcessMixing2(ingData, dataSize);

                lock (rcvqueList)
                {
                    rcvqueList.Remove(ingData);
                }

                return;
            }

            RecInfos recinfo = new RecInfos()
            {
                rcvData = ingData
                ,
                isExtension = recordInfo.isExtension
                ,
                seq = recordInfo.seq
                ,
                size = recordInfo.size
                ,
                voice = recordInfo.voice
            };

            if (recordInfo.isExtension == 0)
            {
                lock (lExtension0)
                {
                    lExtension0.Add(recinfo);
                }
            }

            if (recordInfo.isExtension == 1)
            {
                lock (lExtension1)
                {
                    lExtension1.Add(recinfo);
                }
            }

            int list0 = lExtension0.Count(
                    delegate(RecInfos list)
                    {
                        return list.rcvData.Equals(ingData) && list.isExtension == 0;
                    });

            int list1 = lExtension1.Count(
                    delegate(RecInfos list)
                    {
                        return list.rcvData.Equals(ingData) && list.isExtension == 1;
                    });

            if (list0 >= 20 && list1 >= 20)
            {
                ProcessMixing2(ingData, dataSize);
            }
        }

        private void ProcessMixing2(RcvData data, int dataSize)
        {
            string processingFn = string.Format("d:\\{0}_{1}_{2}.wav", data.seqnum, data.extension, data.peernumber);

            List<RecInfos> ls0 = lExtension0.FindAll(
                        delegate(RecInfos list)
                        {
                            return list.rcvData.Equals(data) && list.isExtension == 0;
                        });

            List<RecInfos> ls1 = lExtension1.FindAll(
                        delegate(RecInfos list)
                        {
                            return list.rcvData.Equals(data) && list.isExtension == 1;
                        });

            IsExtensionComparer isExtensionCompare = new IsExtensionComparer();
            ls0.Sort(isExtensionCompare);
            ls1.Sort(isExtensionCompare);

            int count = 0;
            int count0 = ls0.Count();
            int count1 = ls1.Count();

            if (count0 - count1 < 0)
                count = count0;
            else
                count = count1;

            byte[] buffWriting = new byte[320 * count];

            for (int i = 0; i < count; i++)
            {
                if (ls0[i].seq == ls1[i].seq)
                {
                    // 믹싱

                    // 코덱 종류에 따라서 바이트 길이는 달라질 수 있다. 실제로 만들 때 경우의 수 확인하고 만들어야 한다.
                    byte[] wavSrc0 = new byte[160];
                    byte[] wavSrc1 = new byte[160];

                    Array.Copy(ls0[i].voice, 12, wavSrc0, 0, wavSrc0.Length);
                    Array.Copy(ls1[i].voice, 12, wavSrc1, 0, wavSrc1.Length);

                    WaveMixerStream32 mixer = new WaveMixerStream32();
                    //mixer.AutoStop = true;

                    WaveChannel32 channelStm = null;

                    MemoryStream memStm = null;
                    BufferedStream bufStm = null;
                    RawSourceWaveStream rawSrcStm = null;
                    WaveFormatConversionStream conversionStm = null;

                    for (int j = 0; j < 2; j++)
                    {
                        if (j == 0)
                            memStm = new MemoryStream(wavSrc0);
                        else
                            memStm = new MemoryStream(wavSrc1);

                        bufStm = new BufferedStream(memStm);
                        rawSrcStm = new RawSourceWaveStream(bufStm, mulawFormat);
                        conversionStm = new WaveFormatConversionStream(pcmFormat, rawSrcStm);

                        channelStm = new WaveChannel32(conversionStm);
                        mixer.AddInputStream(channelStm);
                    }
                    mixer.Position = 0;

                    Wave32To16Stream to16 = new Wave32To16Stream(mixer);
                    var convStm = new WaveFormatConversionStream(pcmFormat, to16);
                    byte[] tobyte = new byte[(int)convStm.Length];
                    int chk = convStm.Read(tobyte, 0, (int)convStm.Length);
                    Buffer.BlockCopy(tobyte, 0, buffWriting, i * tobyte.Length, tobyte.Length);

                    conversionStm.Close();
                    rawSrcStm.Close();
                    bufStm.Close();
                    memStm.Close();

                    convStm.Close();
                    to16.Close();
                    channelStm.Close();
                    mixer.Close();

                    // 삭제
                    lExtension0.Remove(ls0[i]);
                    lExtension1.Remove(ls1[i]);
                }
                else if (ls0[i].seq - ls1[i].seq < 0)
                {
                    // ls0 만 믹싱
                    // ls0 원본에 byte[] 붙임 > 원본 byte[]를 wavesream 으로 변환 > wave 파일로 저장

                    if (File.Exists(processingFn))
                    {
                        //wavefilestream = new WaveFileReader(processingFn);
                    }
                    else
                    {

                    }

                    // 삭제
                    lExtension0.Remove(ls0[i]);
                    ls1.Insert(i + 1, ls1[i]);
                }
                else if (ls0[i].seq - ls1[i].seq > 0)
                {
                    // ls1 만 믹싱
                    // ls1 원본에 byte[] 붙임 > 원본 byte[]를 wavesream 으로 변환 > wave 파일로 저장

                    if (File.Exists(processingFn))
                    {
                        //wavefilestream = new WaveFileReader(processingFn);
                    }
                    else
                    {

                    }

                    // 삭제
                    lExtension1.Remove(ls1[i]);
                    ls0.Insert(i + 1, ls0[i]);
                }
            }

            if (playbackState != StreamingPlaybackState.Stopped)
            {
                this.StreamMedia(buffWriting);
            }
        }

        private BufferedWaveProvider bufferedWaveProvider;
        private IWavePlayer waveOut;
        private volatile StreamingPlaybackState playbackState;
        private volatile bool fullyDownloaded;
        private VolumeWaveProvider16 volumeProvider;
        private WaveStream _playWavestream;

        private void StreamMedia(byte[] _stream)
        {
            if (playbackState == StreamingPlaybackState.Stopped)
                return;

            this.fullyDownloaded = false;
            MemoryStream __memstream = new MemoryStream(_stream);

            //byte[] buffer = new byte[16384 * 4]; // needs to be big enough to hold a decompressed frame
            byte[] buffer = new byte[__memstream.Length];

            //var readFullyStream = new ReadFullyStream(__memstream);
            //do
            //{
            if (bufferedWaveProvider != null && bufferedWaveProvider.BufferLength - bufferedWaveProvider.BufferedBytes < bufferedWaveProvider.WaveFormat.AverageBytesPerSecond / 4)
            {
                Debug.WriteLine("Buffer getting full, taking a break");
                Thread.Sleep(500);
            }
            else
            {
                if (_playWavestream == null)
                {
                    this.bufferedWaveProvider = new BufferedWaveProvider(pcmFormat);
                    this.bufferedWaveProvider.BufferDuration = TimeSpan.FromSeconds(20); // allow us to get well ahead of ourselves
                }

                _playWavestream = new RawSourceWaveStream(__memstream, pcmFormat);
                _playWavestream.Read(buffer, 0, buffer.Length);
                bufferedWaveProvider.AddSamples(buffer, 0, buffer.Length);
            }

            //} while (playbackState != StreamingPlaybackState.Stopped);
        }

        void _playTimer_Tick(object state)
        {
            if (playbackState != StreamingPlaybackState.Stopped)
            {
                if (this.waveOut == null && this.bufferedWaveProvider != null)
                {
                    Debug.WriteLine("Creating WaveOut Device");
                    this.waveOut = CreateWaveOut();
                    waveOut.PlaybackStopped += waveOut_PlaybackStopped;
                    this.volumeProvider = new VolumeWaveProvider16(bufferedWaveProvider);
                    //this.volumeProvider.Volume = this.volumeSlider1.Volume;
                    waveOut.Init(volumeProvider);
                    //progressBarBuffer.Maximum = (int)bufferedWaveProvider.BufferDuration.TotalMilliseconds;
                }
                else if (bufferedWaveProvider != null)
                {
                    var bufferedSeconds = bufferedWaveProvider.BufferedDuration.TotalSeconds;
                    //ShowBufferState(bufferedSeconds);
                    // make it stutter less if we buffer up a decent amount before playing
                    if (bufferedSeconds < 0.5 && this.playbackState == StreamingPlaybackState.Playing && !this.fullyDownloaded)
                    {
                        this.playbackState = StreamingPlaybackState.Buffering;
                        waveOut.Pause();
                        Debug.WriteLine(String.Format("Paused to buffer, waveOut.PlaybackState={0}", waveOut.PlaybackState));
                    }
                    else if (bufferedSeconds > 4 && this.playbackState == StreamingPlaybackState.Buffering)
                    {
                        waveOut.Play();
                        Debug.WriteLine(String.Format("Started playing, waveOut.PlaybackState={0}", waveOut.PlaybackState));
                        this.playbackState = StreamingPlaybackState.Playing;
                    }
                    else if (this.fullyDownloaded && bufferedSeconds == 0)
                    {
                        Debug.WriteLine("Reached end of stream");
                        StopPlayback();
                    }
                }

            }

            if (bufferedWaveProvider != null)
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    statustxt0.Text = bufferedWaveProvider.BufferedDuration.ToString();
                }));
            }

            Thread.Sleep(250);
            ThreadPool.QueueUserWorkItem(new WaitCallback(_playTimer_Tick));
        }

        private IWavePlayer CreateWaveOut()
        {
            return new WaveOut();
            //return new DirectSoundOut();
        }

        private void StopPlayback()
        {
            if (playbackState != StreamingPlaybackState.Stopped)
            {
                if (!fullyDownloaded)
                {
                    //webRequest.Abort();
                }
                this.playbackState = StreamingPlaybackState.Stopped;
                if (waveOut != null)
                {
                    waveOut.Stop();
                    waveOut.Dispose();
                    waveOut = null;
                }
                //_playTimer.Enabled = false;
                // n.b. streaming thread may not yet have exited
                Thread.Sleep(500);
                //ShowBufferState(0);
            }
        }

        private void waveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            Debug.WriteLine("Playback Stopped");
            if (e.Exception != null)
            {
                MessageBox.Show(String.Format("Playback Error {0}", e.Exception.Message));
            }
        }
        #endregion 

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _req = new InterceptReq() { cmd = 1 };
            byte[] __bytes = util.GetBytes(_req);
            //_s.SendTo(__bytes, SocketFlags.None, _serverep);
            _s.Send(__bytes, 0, __bytes.Length, SocketFlags.None);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _req = new InterceptReq() { cmd = 3 };
            byte[] __bytes = util.GetBytes(_req);
            //_s.SendTo(__bytes, SocketFlags.None, _serverep);
            _s.Send(__bytes, 0, __bytes.Length, SocketFlags.None);
        }
    }
}
