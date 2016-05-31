using System;
using System.Windows;



using Com.Huen.Libs;
using Com.Huen.Sockets;

namespace RecTestWin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private RTPRecorder _recorder = null;
        private RTPRecorder3 _recorder = null;
        private FileTransferServer _filesrv = null;
        //private FileTransferServer2 _filesrv = null;
        private ModifyRegistry _reg;
        // private bool _isregistered = false;

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // _reg = new ModifyRegistry(util.LoadProjectResource("REG_SUBKEY_CALLRECORDER", "COMMONRES", "").ToString());

            try
            {
                _recorder = new RTPRecorder3();
                //_recorder.StartCmdSrv();
                //_recorder.StartRtpRedirectSrv();

                _filesrv = new FileTransferServer();
                //_filesrv = new FileTransferServer2();
            }
            catch (Exception ex)
            {
                // util.WriteLog(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog __dialog = new System.Windows.Forms.FolderBrowserDialog();
            __dialog.ShowDialog();
            MessageBox.Show(__dialog.SelectedPath);
        }
    }
}
