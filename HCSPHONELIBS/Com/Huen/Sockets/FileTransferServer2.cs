using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Cache;
using System.Threading;
using System.Diagnostics;
using Microsoft.Win32;

using Com.Huen.DataModel;
using Com.Huen.Libs;
using Com.Huen.Sockets;

namespace Com.Huen.Sockets
{
    public class FileTransferServer2 : IDisposable
    {
        private ModifyRegistry _reg = null;
        private CRAgentOption _option = null;
        
        private string _filename = string.Empty;
        private FileInfo _fileinfo;

        private Socket _sockFileSrv;
        private EndPoint _localep;
        private EndPoint _remoteep;
        private Thread _srvThread;
        private bool hasData = false;

        public FileTransferServer2()
        {
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);

            ThreadPool.QueueUserWorkItem(new WaitCallback(LoadRegistry));
            this.StartServer();
        }

        private void StartServer()
        {
            try
            {
                _localep = new IPEndPoint(IPAddress.Any, 21022);
                //_remoteep = new IPEndPoint(IPAddress.Any, 0);
                _sockFileSrv = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _sockFileSrv.Bind(_localep);
            }
            catch (SocketException se)
            {
                util.WriteLog(se.Message);
            }

            _srvThread = new Thread(new ThreadStart(this.Server));
            _srvThread.IsBackground = true;
            _srvThread.Start();
        }

        private void Server()
        {
            byte[] buffer = new byte[1024];
            hasData = true;

            _sockFileSrv.Listen(100);

            while (hasData)
            {
                try
                {
                    using (Socket _sock = _sockFileSrv.Accept())
                    using (NetworkStream _ns = new NetworkStream(_sock))
                    {
                        // first get the header. Header has the file size.
                        Header _header = Header.Deserialize(_ns);

                        switch (_header.Cmd)
                        {
                            case 1:
                                // 파일 download 요청
                                string _tmppath = string.Format("{0}-{1}-{2}", _header.FileName.Substring(0, 4), _header.FileName.Substring(4, 2), _header.FileName.Substring(6, 2));
                                string _filepath = string.Format(@"{0}\{1}", _option.SaveDirectory, _tmppath);
                                string _filefullname = string.Format(@"{0}\{1}", _filepath, _header.FileName);
                                _fileinfo = new FileInfo(_filefullname);

                                _header.Cmd = 3;
                                _header.FileName = _fileinfo.Name;
                                _header.FileSize = _fileinfo.Length;

                                // 파일명으로 path 찾아 filefullname을 만들어 그 파일을 sendfile로 보낸다.

                                byte[] headerBuffer = null;
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    _header.Serialize(ms);
                                    ms.Seek(0, SeekOrigin.Begin);
                                    headerBuffer = ms.ToArray();
                                }

                                byte[] fileBuffer = File.ReadAllBytes(string.Format("{0}\\{1}", _option.SaveDirectory, _header.FileName));

                                byte[] databuff = new byte[headerBuffer.Length + fileBuffer.Length];
                                Array.Copy(headerBuffer, 0, databuff, 0, headerBuffer.Length);
                                Array.Copy(fileBuffer, 0, databuff, headerBuffer.Length, fileBuffer.Length);

                                // send the header and data
                                int buffLength = 1024;
                                int remainbuff = databuff.Length;
                                int sentcount = 0;
                                int count = 0;
                                while (remainbuff > 0)
                                {
                                    if (remainbuff < buffLength)
                                        buffLength = remainbuff;

                                    sentcount = _sock.Send(databuff, sentcount * count, buffLength, SocketFlags.None);
                                    remainbuff -= sentcount;
                                    count++;
                                }
                                break;
                            case 2:
                                // 파일 upload 요청

                                int read = _ns.Read(buffer, 0, buffer.Length);

                                using (MemoryStream _ms = new MemoryStream())
                                {
                                    do
                                    {

                                    } while (read > 0);
                                }

                                //long remaining = _header.FileSize;

                                //while (remaining > 0)
                                //{
                                //    int readSize = buffer.Length;
                                //    if ((long)readSize > remaining)
                                //        readSize = (int)remaining;

                                //    int read = ns.Read(buffer, 0, readSize);
                                //    remaining -= read;
                                //}

                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    util.WriteLog(string.Format("FileTransfer socket error : {0}", ex.Message));
                }
            }
        }

        private void LoadRegistry(object state)
        {
            _reg = new ModifyRegistry(util.LoadProjectResource("REG_SUBKEY_CALLRECORDER", "COMMONRES", "").ToString());
            byte[] __bytes = (byte[])_reg.GetValue(RegKind.LocalMachine, "CR");
            _option = (CRAgentOption)util.ByteArrayToObject(__bytes);

            Thread.Sleep(1000);
            ThreadPool.QueueUserWorkItem(new WaitCallback(LoadRegistry));
        }

        public void Dispose()
        {
            _sockFileSrv.Close();
            _sockFileSrv.Dispose();
            _sockFileSrv = null;
        }

    }
}
