using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using System.Diagnostics;
using System.Collections.ObjectModel;

using Com.Huen.DataModel;
using Com.Huen.Libs;

namespace Com.Huen.Views
{
    /// <summary>
    /// Interaction logic for SearchProperties.xaml
    /// </summary>
    public partial class SearchProperties : Window
    {
        private SearchNintercept parentWin = null;
        public ObservableCollection<InnerTel> listTel = null;
        public InnerTels innertels;

        public SearchProperties()
        {
            InitializeComponent();

            this.ReadIni();

            //this.dr_inntertel.CanUserDeleteRows = false;
            //this.dr_inntertel.RowEditEnding += dr_inntertel_RowEditEnding;
            //this.dr_inntertel.PreviewKeyDown += dr_inntertel_PreviewKeyDown;

            this.Loaded += SearchProperties_Loaded;
            this.Closed += SearchProperties_Closed;
        }

        private void ReadIni()
        {
            // Ini ini = new Ini(string.Format(@"{0}\{1}\{2}.ini", Options.usersdatapath, Options.appname));
            Ini ini = new Ini(string.Format(@"{0}\{1}.ini", Options.usersdatapath, Options.appname));

            txtRecSrvIP.Text = string.IsNullOrEmpty(ini.IniReadValue("SERVER", "rec")) == false ? ini.IniReadValue("SERVER", "rec") : "127.0.0.1";
            txtDBSrvIP.Text = string.IsNullOrEmpty(ini.IniReadValue("SERVER", "db")) == false ? ini.IniReadValue("SERVER", "db") : "127.0.0.1";
            txtPbxSrvIP.Text = string.IsNullOrEmpty(ini.IniReadValue("SERVER", "pbx")) == false ? ini.IniReadValue("SERVER", "pbx") : "127.0.0.1";
            txtSelDir.Text = string.IsNullOrEmpty(ini.IniReadValue("ETC", "savedir")) == false ? ini.IniReadValue("ETC", "savedir") : string.Format(@"{0}\{1}", Options.usersdatapath, "RecFiles");
        }

        private void SaveIni()
        {
            // Ini ini = new Ini(string.Format(@"{0}\{1}\{2}.ini", Options.usersdatapath, Options.appname));
            Ini ini = new Ini(string.Format(@"{0}\{1}.ini", Options.usersdatapath, Options.appname));

            ini.IniWriteValue("SERVER", "rec", txtRecSrvIP.Text.Trim());
            ini.IniWriteValue("SERVER", "db", txtDBSrvIP.Text.Trim());
            ini.IniWriteValue("SERVER", "pbx", txtPbxSrvIP.Text.Trim());
            ini.IniWriteValue("ETC", "savedir", Options.savedir);

            Options.recserverip = txtRecSrvIP.Text.Trim();
            Options.dbserverip = txtDBSrvIP.Text.Trim();
            Options.pbxip = txtPbxSrvIP.Text.Trim();
        }

        void SearchProperties_Closed(object sender, EventArgs e)
        {
            this.SaveIni();
        }

        void SearchProperties_Loaded(object sender, RoutedEventArgs e)
        {
            innertels = new InnerTels();
            lv_innertel.ItemsSource = innertels.GetList;
        }

        void datagrid1_PreviewDeleteCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {

        }

        void dr_inntertel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataGrid __datagrid = (DataGrid)sender;

            if (e.Key == Key.Delete)
            {
                
            }            
        }

        void dr_inntertel_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            Debug.WriteLine("DataGrid RowEditing : " + e.EditAction.ToString());
        }

        #region Tab ContextMenu click s
        private void tab0ContextMenuAdd_Click(object sender, RoutedEventArgs e)
        {
            InnerTelWin pop = new InnerTelWin();
            pop.Owner = this;
            pop.Show();
        }

        private void tab0ContextMenuModi_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.Source;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            ListView lv = (ListView)contextMenu.PlacementTarget;
            InnerTel obj = (InnerTel)lv.SelectedItem;

            InnerTelWin pop = new InnerTelWin();
            pop.Owner = this;
            pop.innertel = obj;
            pop.Show();
        }

        private void tab0ContextMenuDel_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.Source;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            ListView lv = (ListView)contextMenu.PlacementTarget;
            InnerTel obj = (InnerTel)lv.SelectedItem;

            innertels.Remove(obj);
        }

        private void lv_innertel_ContextMenuOpening(object sender, ContextMenuEventArgs e)
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

                if (locSelectedIndex < 0)
                {
                    if (i == 1 || i == 2)
                        mi.IsEnabled = false;
                }
            }
        }
        #endregion Tab ContextMenu click e

        #region 환경설정 s
        private void btnSelDir_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();

            if (string.IsNullOrEmpty(dialog.SelectedPath))
            {
                txtSelDir.Text = @"C:\temp";
            }
            else
            {
                txtSelDir.Text = dialog.SelectedPath;
            }

            Options.savedir = txtSelDir.Text;

            Ini ini = new Ini(@".\properties.ini");
            ini.IniWriteValue("ETC", "savedir", txtSelDir.Text);
        }
        #endregion 환경설정 e

        private void chbInnertelList_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox _chb = (CheckBox)e.Source;
            parentWin._option.UsingInnertelCol = _chb.IsChecked == true ? true : false;

            if (parentWin._option.UsingInnertelCol)
            {
                if (parentWin._option.GridColDef2.Value == 0.0d)
                {
                    parentWin.grid0.ColumnDefinitions[1].Width = new GridLength(1d, GridUnitType.Star);
                }
                else
                {
                    parentWin.grid0.ColumnDefinitions[1].Width = parentWin._option.GridColDef1;
                }
            }
            else
            {
                parentWin.grid0.ColumnDefinitions[1].Width = new GridLength(1d, GridUnitType.Star);
            }
        }

        private void txtSrvIP_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox _txtbox = (TextBox)sender;
            //MaskedTextBox _txtbox = (MaskedTextBox)sender;

            //if (e.Key == Key.OemPeriod)
            //{
            //    int _assignPos = _txtbox.MaskedTextProvider.AssignedEditPositionCount;

            //    if (_assignPos > 12)
            //    {
            //        return;
            //    }
            //    else
            //    {
            //        if (_assignPos < 4)
            //            _txtbox.MaskedTextProvider.IsEditPosition(4);
            //        else if (_assignPos >= 4 || _assignPos < 7)
            //            _txtbox.MaskedTextProvider.IsEditPosition(7);
            //    }
            //}

            //if (!_txtbox.IsMaskCompleted) return;
            if (_txtbox.Text.Length < 15) return;

            string __pattern = @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";
            if (!System.Text.RegularExpressions.Regex.IsMatch(_txtbox.Text, __pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("IP 주소 패턴이 아닙니다.");
                return;
            }
        }

        private void txtSrvIP_KeyUp_1(object sender, KeyEventArgs e)
        {
            TextBox _txtbox = (TextBox)sender;
            if (_txtbox.Text.Length < 15) return;

            string __pattern = @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b";
            if (!System.Text.RegularExpressions.Regex.IsMatch(_txtbox.Text, __pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("IP 주소 패턴이 아닙니다.");
                return;
            }
        }
    }
}
