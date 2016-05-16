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

using System.Data;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;

using Com.Huen.DataModel;
using Com.Huen.Libs;
using Com.Huen.Sql;

namespace Com.Huen.UserControls
{
    /// <summary>
    /// Interaction logic for CustGroupTreeViewControl.xaml
    /// </summary>
    public partial class CustGroupTreeViewControl : UserControl
    {
        public delegate void CustGroupSelectedItemChangedHandler(CustGroupViewModel selectedItem);
        public event CustGroupSelectedItemChangedHandler SelectedCustGroupViewModelChanged;

        public int itemsCount = 0;

        private CustGroupViewModel _selectedItem;
        private CustGroupTreeViewModel _tree;
        private int _curSelectedIdx = 0;

        private Storyboard _textField_ON;
        private Storyboard _textField_OFF;
        private bool _sb_textFiled_began = false;
        private GroupAction _curAction = GroupAction.None;

        private CustGroup root;
        public CustGroupTreeViewControl()
        {
            InitializeComponent();

            // Get raw family tree data from a database.
            root = GetSubTree();

            // Create UI-friendly wrappers around the 
            // raw data objects (i.e. the view-model).
            _tree = new CustGroupTreeViewModel(root);

            // Let the UI bind to the view-model.
            base.DataContext = _tree;

            CustGroupTreeView.SelectedItemChanged += CustGroupTreeView_SelectedItemChanged;

            _textField_ON = (Storyboard)this.Resources["TextFiled_ON"];
            _textField_OFF = (Storyboard)this.Resources["TextFiled_OFF"];

            txtGroupName.KeyDown += txtGroupName_KeyDown;
            txtGroupName.KeyUp += txtGroupName_KeyUp;
        }

        void txtGroupName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty(txtGroupName.Text.Trim()))
                {
                    MessageBox.Show("그룹명을 입력해 주세요.");
                    return;
                }
            }
        }

        void txtGroupName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (_sb_textFiled_began)
                {
                    _textField_OFF.Begin();
                    _sb_textFiled_began = false;
                }
                txtGroupName.Text = string.Empty;
            }
            else if (e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty(txtGroupName.Text.Trim()))
                    return;

                _textField_OFF.Begin();
                _sb_textFiled_began = false;

                if (_curAction == GroupAction.Create)
                {
                    CustGroup __custgroup = new CustGroup()
                    {
                        Name = txtGroupName.Text.Trim()
                    };

                    DataTable dt = util.MakeDataTable2Proc();
                    DataRow dr = dt.NewRow();
                    dr["DataName"] = "@i_cstg_name";
                    dr["DataValue"] = __custgroup.Name;
                    dt.Rows.Add(dr);

                    dr = dt.NewRow();
                    dr["DataName"] = "@i_cstg_parent";
                    dr["DataValue"] = _curSelectedIdx;
                    dt.Rows.Add(dr);

                    dr = dt.NewRow();
                    dr["DataName"] = "@i_com_idx";
                    dr["DataValue"] = util.Userinfo.COM_IDX;
                    dt.Rows.Add(dr);

                    string __idx = string.Empty;
                    using (FirebirdDBHelper db = new FirebirdDBHelper(util.strDBConn))
                    {
                        try
                        {
                            db.BeginTran();
                            __idx = db.GetDataSP("INS_CUSTGROUP", dt).ToString();
                            db.Commit();

                            __custgroup.Idx = __idx;

                            root.Children.Add(__custgroup);
                            root.Children.Sort(new CustGroupComparer());
                            _tree = new CustGroupTreeViewModel(root);
                            base.DataContext = _tree;

                            _selectedItem.IsExpanded = true;
                            _selectedItem.IsSelected = true;

                            _curAction = GroupAction.None;
                        }
                        catch (FirebirdSql.Data.FirebirdClient.FbException fe)
                        {
                            db.Rollback();
                            throw fe;
                        }
                    }
                }
                else if (_curAction == GroupAction.Modify)
                {
                    DataTable dt = util.MakeDataTable2Proc();
                    DataRow dr = dt.NewRow();
                    dr["DataName"] = "@i_chk_bhv";
                    dr["DataValue"] = "MODIFY";
                    dt.Rows.Add(dr);

                    dr = dt.NewRow();
                    dr["DataName"] = "@i_cstg_idx";
                    dr["DataValue"] = __selectedItem.Idx;
                    dt.Rows.Add(dr);

                    dr = dt.NewRow();
                    dr["DataName"] = "@i_cstg_name";
                    dr["DataValue"] = txtGroupName.Text.Trim();
                    dt.Rows.Add(dr);

                    using (FirebirdDBHelper db = new FirebirdDBHelper(util.strDBConn))
                    {
                        try
                        {
                            db.BeginTran();
                            db.ExcuteSP("UDT_CUSTGROUP", dt);
                            db.Commit();

                            var __item = root.Children.FirstOrDefault(x => x.Idx == __selectedItem.Idx);
                            var __idx = root.Children.IndexOf(__item);
                            __item.Name = txtGroupName.Text.Trim();

                            root.Children[__idx] = __item;
                            root.Children.Sort(new CustGroupComparer());
                            __idx = root.Children.IndexOf(__item);
                            _tree = new CustGroupTreeViewModel(root);
                            base.DataContext = _tree;

                            ((CustGroupViewModel)CustGroupTreeView.Items[0]).IsExpanded = true;
                            //((CustGroupViewModel)CustGroupTreeView.Items[0]).IsSelected = true;

                            ((CustGroupViewModel)CustGroupTreeView.Items[0]).Children[__idx].IsSelected = true;

                            _curAction = GroupAction.None;

                        }
                        catch (FirebirdSql.Data.FirebirdClient.FbException fe)
                        {
                            db.Rollback();
                            throw fe;
                        }
                    }
                }

                txtGroupName.Text = string.Empty;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_curSelectedIdx == 0)
            {
                if (CustGroupTreeView.Items.Count > 0)
                {
                    ((CustGroupViewModel)CustGroupTreeView.Items[0]).IsExpanded = true;
                    ((CustGroupViewModel)CustGroupTreeView.Items[0]).IsSelected = true;
                }
                _curSelectedIdx = int.Parse(((CustGroupViewModel)CustGroupTreeView.Items[0]).Idx);
                //_curSelectedIdx = "0";
            }
        }

        void CustGroupTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView selectedTV = (TreeView)e.OriginalSource;

            if (selectedTV.SelectedItem.GetType() != typeof(Com.Huen.DataModel.CustGroupViewModel))
                return;

            CustGroupViewModel selectedTVItem = (CustGroupViewModel)selectedTV.SelectedItem;
            _selectedItem = selectedTVItem;
            _curSelectedIdx = int.Parse(selectedTVItem.Idx);

            if (SelectedCustGroupViewModelChanged != null)
                SelectedCustGroupViewModelChanged(selectedTVItem);
        }

        private CustGroup GetSubTree()
        {
            DataTable dt = util.MakeDataTable2Proc();
            DataRow dr = dt.NewRow();
            dr["DataName"] = "@i_com_idx";
            dr["DataValue"] = util.Userinfo.COM_IDX;
            dt.Rows.Add(dr);

            using (FirebirdDBHelper db = new FirebirdDBHelper(util.strDBConn))
            {
                try
                {
                    dt = db.GetDataTableSP("GET_CUSTGROUP_LIST", dt);
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
            }

            var value1 = from myRow in dt.AsEnumerable()
                         //where myRow["CSTG_DEPTH"].ToString() == "1"
                         select myRow;

            CustGroup departSub = new CustGroup()
            {
                Idx = "0",
                Name = util.LoadProjectResource("TEXT_TREE_GROUP_ROOT", "COMMONRES", "").ToString(),
            };

            List<CustGroup> depart1List = new List<CustGroup>();
            foreach (var row1 in value1)
            {
                CustGroup depart1 = new CustGroup();
                depart1.Idx = row1["CSTG_IDX"].ToString();
                depart1.Name = row1["CSTG_NAME"].ToString();

                var value2 = from myRow2 in dt.AsEnumerable()
                             where myRow2["CSTG_DEPTH"].ToString() == "2" && myRow2["CSTG_PARENT"].ToString() == row1["CSTG_IDX"].ToString()
                             select myRow2;
                List<CustGroup> depart2List = new List<CustGroup>();
                foreach (var row2 in value2)
                {
                    CustGroup depart2 = new CustGroup();
                    depart2.Idx = row2["CSTG_IDX"].ToString();
                    depart2.Name = row2["CSTG_NAME"].ToString();

                    var value3 = from myRow3 in dt.AsEnumerable()
                                 where myRow3["CSTG_DEPTH"].ToString() == "3" && myRow3["CSTG_PARENT"].ToString() == row2["CSTG_IDX"].ToString()
                                 select myRow3;
                    List<CustGroup> depart3List = new List<CustGroup>();
                    foreach (var row3 in value3)
                    {
                        CustGroup depart3 = new CustGroup();
                        depart3.Idx = row3["CSTG_IDX"].ToString();
                        depart3.Name = row3["CSTG_NAME"].ToString();

                        depart3List.Add(depart3);
                    }
                    depart2.Children = depart3List;
                    depart2List.Add(depart2);
                }
                depart1.Children = depart2List;
                depart1List.Add(depart1);
            }

            departSub.Children = depart1List;

            return departSub;
        }

        private void menuitem0_Click(object sender, RoutedEventArgs e)
        {
            _textField_ON.Begin();
            txtGroupName.Focus();
            _sb_textFiled_began = true;

            _curAction = GroupAction.Create;
        }

        private CustGroupViewModel __selectedItem;
        private void menuitem1_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.Source;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            TreeView lv = (TreeView)contextMenu.PlacementTarget;
            __selectedItem = (CustGroupViewModel)lv.SelectedItem;

            _textField_ON.Begin();
            txtGroupName.Text = __selectedItem.Name;
            txtGroupName.Focus();
            txtGroupName.SelectAll();
            _sb_textFiled_began = true;

            _curAction = GroupAction.Modify;
        }

        private void menuitem2_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.Source;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            TreeView lv = (TreeView)contextMenu.PlacementTarget;
            __selectedItem = (CustGroupViewModel)lv.SelectedItem;

            if (itemsCount > 0)
            {
                MessageBox.Show(string.Format("\"{0}\" 그룹에 종속된 데이터가 있습니다.\n\r종속 데이터를 삭제하거나 다른 그룹으로 이동 후 \"{0}\" 그룹을 삭제하세요.", _selectedItem.Name));
                return;
            }

            DataTable dt = util.MakeDataTable2Proc();
            DataRow dr = dt.NewRow();
            dr["DataName"] = "@i_chk_bhv";
            dr["DataValue"] = "REMOVE";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["DataName"] = "@i_cstg_idx";
            dr["DataValue"] = __selectedItem.Idx;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["DataName"] = "@i_cstg_name";
            dr["DataValue"] = "";
            dt.Rows.Add(dr);

            using (FirebirdDBHelper db = new FirebirdDBHelper(util.strDBConn))
            {
                try
                {
                    db.BeginTran();
                    db.ExcuteSP("UDT_CUSTGROUP", dt);
                    db.Commit();

                    var __item = root.Children.FirstOrDefault(x => x.Idx == __selectedItem.Idx);
                    root.Children.Remove(__item);
                    root.Children.Sort(new CustGroupComparer());
                    _tree = new CustGroupTreeViewModel(root);
                    base.DataContext = _tree;

                    ((CustGroupViewModel)CustGroupTreeView.Items[0]).IsExpanded = true;
                    ((CustGroupViewModel)CustGroupTreeView.Items[0]).IsSelected = true;
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException fe)
                {
                    db.Rollback();
                    throw fe;
                }
            }
        }

        private void CustGroupTreeView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            TreeView lv = (TreeView)e.Source;
            ContextMenu cm = lv.ContextMenu;

            foreach (MenuItem mi in cm.Items)
            {
                mi.IsEnabled = true;
            }

            CustGroupViewModel dv = (CustGroupViewModel)lv.SelectedItem;

            string __locSelectedIndex = ((CustGroupViewModel)lv.SelectedItem).Idx;

            for (int i = 0; i < cm.Items.Count; i++)
            {
                MenuItem mi = (MenuItem)cm.Items[i];

                if (__locSelectedIndex.Equals("0"))
                {
                    if (i == 1 || i == 2)
                        mi.IsEnabled = false;
                }
                else
                {
                    if (i == 0)
                        mi.IsEnabled = false;
                }
            }
        }
    }
}
