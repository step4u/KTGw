using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using System.ServiceProcess;

using System.IO;
using Com.Huen.Libs;
using Com.Huen.DataModel;

namespace Com.Huen.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CallRecorderAgent : Window
    {
        private System.Windows.Forms.NotifyIcon ni;
        private ServiceController servicecontroller;

        private bool trueExit = false;

        private string inipath = string.Format(@"{0}\{1}.ini", Options.ProgramDataPath, Options.AppName);

        public bool TrueExit
        {
            get { return trueExit; }
            set { trueExit = value; }
        }

        public CallRecorderAgent()
        {
            InitializeComponent();

            this.ReadIni();
            this.InitializeWindow();
            this.TrayIconInitialize();

            this.Closing += CallRecorderAgent_Closing;
            // this.StateChanged += CallRecorderAgent_StateChanged;

            this.Close();
        }

        private void CallRecorderAgent_StateChanged(object sender, EventArgs e)
        {
            CallRecorderAgent win = sender as CallRecorderAgent;
            var states = win.WindowState;

            if (states == WindowState.Normal)
            {
                this.ReadIni();
                this.InitializeWindow();
            }
        }

        void CallRecorderAgent_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!TrueExit)
            {
                e.Cancel = true;
                this.WindowState = WindowState.Minimized;
                this.Hide();
            }

            if (this.IsLoaded) this.SaveIni();
        }

        private void ReadIni()
        {
            Ini ini = new Ini(inipath);
            Options.filetype = string.IsNullOrEmpty(ini.IniReadValue("RECORDER", "filetype")) == false ? ini.IniReadValue("RECORDER", "filetype").ToLower() : "wav";
            Options.savedir = string.IsNullOrEmpty(ini.IniReadValue("RECORDER", "savedir")) == false ? ini.IniReadValue("RECORDER", "savedir") : string.Format(@"{0}\{1}", Options.ProgramDataPath, "RecFiles");
            Options.dbserverip = string.IsNullOrEmpty(ini.IniReadValue("RECORDER", "dbserverip")) == false ? ini.IniReadValue("RECORDER", "dbserverip") : "127.0.0.1";
            Options.autostart = string.IsNullOrEmpty(ini.IniReadValue("RECORDER", "autostart")) == false ? bool.Parse(ini.IniReadValue("RECORDER", "autostart")) : false;
            Options.recextensions = string.IsNullOrEmpty(ini.IniReadValue("RECORDER", "recexts").Trim()) == false ? ini.IniReadValue("RECORDER", "recexts").Split(',') : null;
        }

        private void SaveIni()
        {
            Ini ini = new Ini(inipath);

            ini.IniWriteValue("RECORDER", "filetype", Options.filetype);
            ini.IniWriteValue("RECORDER", "savedir", Options.savedir);
            ini.IniWriteValue("RECORDER", "dbserverip", Options.dbserverip);
            ini.IniWriteValue("RECORDER", "autostart", Options.autostart.ToString());
        }

        private void InitializeWindow()
        {
            if (Options.filetype == "wav")
            {
                fileWav.IsChecked = true;
            }
            else
            {
                fileMp3.IsChecked = true;
            }

            txtSelDir.Text = Options.savedir;
            txtDBip.Text = Options.dbserverip;
            chk_agentautostart.IsChecked = Options.autostart == true ? true : false;
        }

        private void TrayIconInitialize()
        {
            servicecontroller = new ServiceController("Coretree Call Recorder");

            ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = (System.Drawing.Icon)util.LoadProjectResource("icon", "COMMONRES", "");
            ni.Text = string.Format("{0}", "Coretree Call Record Agent");

            System.Windows.Forms.ContextMenu contextmenu = new System.Windows.Forms.ContextMenu();

            System.Windows.Forms.MenuItem miTitle = new System.Windows.Forms.MenuItem();
            miTitle.Index = 0;
            miTitle.Enabled = false;
            miTitle.Text = "Coretree Call Record Service";

            System.Windows.Forms.MenuItem miSeperator0 = new System.Windows.Forms.MenuItem();
            miSeperator0.Index = 1;
            miSeperator0.Text = "-";

            System.Windows.Forms.MenuItem mi0 = new System.Windows.Forms.MenuItem();
            mi0.Index = 2;
            mi0.Text = "환경설정(&P)";
            mi0.Click += delegate(object sender, EventArgs args)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
                this.ReadIni();
            };

            System.Windows.Forms.MenuItem mi1 = new System.Windows.Forms.MenuItem();
            mi1.Index = 3;
            mi1.Text = "서비스 시작(&S)";
            mi1.Click += delegate(object sender, EventArgs args)
            {
                if (servicecontroller.Status == ServiceControllerStatus.Stopped)
                {
                    servicecontroller.Start();
                }
            };

            System.Windows.Forms.MenuItem mi2 = new System.Windows.Forms.MenuItem();
            mi2.Index = 4;
            mi2.Text = "서비스 멈춤(&S)";
            mi2.Enabled = false;
            mi2.Click += delegate(object sender, EventArgs args)
            {
                if (servicecontroller.Status == ServiceControllerStatus.Running)
                {
                    servicecontroller.Stop();
                }
            };

            System.Windows.Forms.MenuItem miSeperator1 = new System.Windows.Forms.MenuItem();
            miSeperator1.Index = 5;
            miSeperator1.Text = "-";

            System.Windows.Forms.MenuItem mi3 = new System.Windows.Forms.MenuItem();
            mi3.Index = 6;
            mi3.Text = "종료(&X)";
            mi3.Click += delegate(object sender, EventArgs args)
            {
                this.trueExit = true;
                this.Close();
            };

            contextmenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] { miTitle, miSeperator0, mi0, mi1, mi2, miSeperator1, mi3 });

            this.ni.ContextMenu = contextmenu;
            this.ni.Visible = true;
            this.ni.DoubleClick +=
                delegate(object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                    this.ReadIni();
                };
            this.ni.ContextMenu.Popup += ContextMenu_Popup;
        }

        void ContextMenu_Popup(object sender, EventArgs e)
        {
            servicecontroller = new ServiceController("Coretree Call Recorder");

            System.Windows.Forms.ContextMenu contextmenu = (System.Windows.Forms.ContextMenu)sender;
            var menuitemcollection = contextmenu.MenuItems;

            try
            {
                foreach (System.Windows.Forms.MenuItem _menuitem in menuitemcollection)
                {
                    _menuitem.Enabled = true;
                }

                System.Windows.Forms.MenuItem item;
                if (servicecontroller.Status == ServiceControllerStatus.Stopped)
                {
                    item = menuitemcollection.Cast<System.Windows.Forms.MenuItem>().FirstOrDefault(x => x.Index == 3);
                    item.Enabled = true;
                    item = menuitemcollection.Cast<System.Windows.Forms.MenuItem>().FirstOrDefault(x => x.Index == 4);
                    item.Enabled = false;
                }
                else if (servicecontroller.Status == ServiceControllerStatus.Running)
                {
                    item = menuitemcollection.Cast<System.Windows.Forms.MenuItem>().FirstOrDefault(x => x.Index == 3);
                    item.Enabled = false;
                    item = menuitemcollection.Cast<System.Windows.Forms.MenuItem>().FirstOrDefault(x => x.Index == 4);
                    item.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MenuItem item;
                item = menuitemcollection.Cast<System.Windows.Forms.MenuItem>().FirstOrDefault(x => x.Index == 3);
                item.Enabled = false;
                item = menuitemcollection.Cast<System.Windows.Forms.MenuItem>().FirstOrDefault(x => x.Index == 4);
                item.Enabled = false;
            }
        }

        #region 내선관리 s
        private void innerTelList_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ListView lv = (ListView)e.Source;
            ContextMenu cm = lv.ContextMenu;

            foreach (MenuItem mi in cm.Items)
            {
                mi.IsEnabled = true;
            }

            int locSelectedIndex = lv.SelectedIndex;

            for (int i = 0; i < cm.Items.Count; i++)
            {
                MenuItem mi = (MenuItem)cm.Items[i];

                if (locSelectedIndex == -1)
                {
                    if (i == 1)
                    {
                        mi.IsEnabled = false;
                    }
                }
            }
        }

        private void innertel_menu0_Click(object sender, RoutedEventArgs e)
        {
            InnerTelWin pop = new InnerTelWin();
            pop.Owner = this;
            pop.Show();
        }

        private void innertel_menu1_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion 내선관리 e

        private void btnSelDir_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();
            txtSelDir.Text = dialog.SelectedPath;
            Options.savedir = txtSelDir.Text;

            this.SaveIni();
        }

        private void btnOpenEnvFile_Click(object sender, RoutedEventArgs e) {
            string filepath = Options.ProgramDataPath + "\\" + "CallRecorder.ini";

            if (File.Exists(filepath))
                System.Diagnostics.Process.Start(filepath);
            else
                MessageBox.Show("Environment file doesn't exist.");
        }

        private void filetype_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            Options.filetype = rb.Content.ToString().ToLower();

            this.SaveIni();
        }

        private void chk_agentautostart_Checked(object sender, RoutedEventArgs e)
        {
            Options.autostart = true;

            if (this.IsLoaded) this.SaveIni();

            ModifyRegistry reg = new ModifyRegistry(util.LoadProjectResource("REG_MAIN_RUN", "COMMONRES", "").ToString());
            string curDir = Directory.GetCurrentDirectory();
            reg.SetValue(RegKind.LocalMachine, "CallRecorderAgent", string.Format(@"{0}\CallRecorderAgent.exe", curDir));
        }

        private void chk_agentautostart_Unchecked(object sender, RoutedEventArgs e)
        {
            Options.autostart = false;

            if (this.IsLoaded) this.SaveIni();

            ModifyRegistry reg = new ModifyRegistry(util.LoadProjectResource("REG_MAIN_RUN", "COMMONRES", "").ToString());
            reg.DeleteValue(RegKind.LocalMachine, "CallRecorderAgent");
        }
    }
}
