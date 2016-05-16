using System;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Com.Huen.DataModel;
using Com.Huen.Libs;

namespace Com.Huen.Sockets
{
    public class FileTransferServer : IDisposable
    {
        private string inipath = string.Format(@".\{0}", "env.ini");

        private string filename = string.Empty;
        private FileInfo fileinfo;

        private Socket sockFileSrv;
        private EndPoint localep;
        private Thread srvThread;
        private bool hasData = false;

        public FileTransferServer()
        {
            Options.companyname = "Coretree";
            Options.appname = "Call Recorder";

            this.ReadIni();

            this.StartServer();
        }

        private void ReadIni()
        {
            Ini ini = new Ini(inipath);

            // Options.savedir = string.IsNullOrEmpty(ini.IniReadValue("RECORDER", "savedir")) == false ? ini.IniReadValue("RECORDER", "savedir") : string.Format(@".\{0}", "RecFiles");
        }

        private void StartServer()
        {
            try
            {
                localep = new IPEndPoint(IPAddress.Any, 21022);
                sockFileSrv = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sockFileSrv.Bind(localep);
            }
            catch (SocketException se)
            {
                util.WriteLog(se.Message);
            }

            srvThread = new Thread(new ThreadStart(this.Server));
            srvThread.IsBackground = true;
            srvThread.Start();
        }

        private void Server()
        {
            byte[] buffer = new byte[1024];
            hasData = true;

            sockFileSrv.Listen(100);

            while (hasData)
            {
                try
                {
                    using (Socket sock = sockFileSrv.Accept())
                    using (NetworkStream _ns = new NetworkStream(sock))
                    {
                        // first get the header. Header has the file size.
                        Header header = Header.Deserialize(_ns);

                        switch (header.Cmd)
                        {
                            case 1:
                                // 파일 download 요청
                                string tmppath = string.Format("{0}-{1}-{2}", header.FileName.Substring(0, 4), header.FileName.Substring(4, 2), header.FileName.Substring(6, 2));
                                string filepath = string.Format(@"{0}\{1}", Options.savedir, tmppath);
                                string filefullname = string.Format(@"{0}\{1}", filepath, header.FileName);
                                fileinfo = new FileInfo(filefullname);

                                header.Cmd = 3;
                                header.FileName = fileinfo.Name;
                                header.FileSize = fileinfo.Length;

                                // 파일명으로 path 찾아 filefullname을 만들어 그 파일을 sendfile로 보낸다.

                                byte[] headerBuffer = null;
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    header.Serialize(ms);
                                    ms.Seek(0, SeekOrigin.Begin);
                                    headerBuffer = ms.ToArray();
                                }

                                sock.SendFile(filefullname, headerBuffer, null, TransmitFileOptions.UseDefaultWorkerThread);
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

#if false
        private void LoadRegistry(object state)
        {
            _reg = new ModifyRegistry(util.LoadProjectResource("REG_SUBKEY_CALLRECORDER", "COMMONRES", "").ToString());
            byte[] __bytes = (byte[])_reg.GetValue(RegKind.LocalMachine, "CR");
            _option = (CRAgentOption)util.ByteArrayToObject(__bytes);

            Thread.Sleep(1000);
            ThreadPool.QueueUserWorkItem(new WaitCallback(LoadRegistry));
        }
#endif

        public void Dispose()
        {
            sockFileSrv.Close();
            sockFileSrv.Dispose();
            sockFileSrv = null;
        }

    }
}
