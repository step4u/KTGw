﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using System.IO;

using Com.Huen.Libs;
using Com.Huen.Sockets;
using Microsoft.Win32;

namespace CallRecorder
{
    public partial class CoretreeCallRecorder : ServiceBase
    {
        private RTPRecorder2 recorder = null;
        private FileTransferServer filesrv = null;

        public CoretreeCallRecorder()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
            //util.WriteLogTest2(Path.GetPathRoot(Environment.SystemDirectory));

            try
            {
                recorder = new RTPRecorder2();
                //_recorder.StartCmdSrv();
                //_recorder.StartRtpRedirectSrv();

                filesrv = new FileTransferServer();
            }
            catch (Exception ex)
            {
                util.WriteLog(ex.Message);
            }
        }

        protected override void OnStop()
        {
        }
    }
}
