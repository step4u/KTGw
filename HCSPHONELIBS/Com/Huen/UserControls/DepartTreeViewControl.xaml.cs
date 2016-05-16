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
    /// Interaction logic for DepartTreeViewControl.xaml
    /// </summary>
    public partial class DepartTreeViewControl : UserControl
    {
        public delegate void DepartmentSelectedItemChangedHandler(DepartmentViewModel selectedItem);
        public event DepartmentSelectedItemChangedHandler SelectedDepartmentViewModelChanged;

        public int itemsCount = 0;
        
        private DepartmentViewModel _selectedItem;
        private DepartmentTreeViewModel _departmenttree;
        private int _curSelectedIdx = 0;

        private Storyboard _textField_ON;
        private Storyboard _textField_OFF;
        private bool _sb_textFiled_began = false;
        private GroupAction _curAction = GroupAction.None;

        private Department rootDepartment;
        public DepartTreeViewControl()
        {
            InitializeComponent();

            // Get raw family tree data from a database.
            rootDepartment = GetSubDepartTree();

            // Create UI-friendly wrappers around the 
            // raw data objects (i.e. the view-model).
            _departmenttree = new DepartmentTreeViewModel(rootDepartment);

            // Let the UI bind to the view-model.
            base.DataContext = _departmenttree;

            DepartmentTreeView.SelectedItemChanged += DepartmentTreeView_SelectedItemChanged;

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
                    Department __depart = new Department()
                    {
                        Name = txtGroupName.Text.Trim()
                    };

                    DataTable dt = util.MakeDataTable2Proc();
                    DataRow dr = dt.NewRow();
                    dr["DataName"] = "@i_part_name";
                    dr["DataValue"] = __depart.Name;
                    dt.Rows.Add(dr);

                    dr = dt.NewRow();
                    dr["DataName"] = "@i_part_parent";
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
                            __idx = db.GetDataSP("INS_DEPARTMENT", dt).ToString();
                            db.Commit();

                            __depart.Idx = __idx;

                            rootDepartment.Children.Add(__depart);
                            rootDepartment.Children.Sort(new DepartComparer());
                            _departmenttree = new DepartmentTreeViewModel(rootDepartment);
                            base.DataContext = _departmenttree;

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
                    dr["DataName"] = "@i_part_idx";
                    dr["DataValue"] = __selectedItem.Idx;
                    dt.Rows.Add(dr);

                    dr = dt.NewRow();
                    dr["DataName"] = "@i_part_name";
                    dr["DataValue"] = txtGroupName.Text.Trim();
                    dt.Rows.Add(dr);

                    using (FirebirdDBHelper db = new FirebirdDBHelper(util.strDBConn))
                    {
                        try
                        {
                            db.BeginTran();
                            db.ExcuteSP("UDT_DEPARTMENT", dt);
                            db.Commit();

                            var __item = rootDepartment.Children.FirstOrDefault(x => x.Idx == __selectedItem.Idx);
                            var __idx = rootDepartment.Children.IndexOf(__item);
                            __item.Name = txtGroupName.Text.Trim();

                            rootDepartment.Children[__idx] = __item;
                            rootDepartment.Children.Sort(new DepartComparer());
                            __idx = rootDepartment.Children.IndexOf(__item);
                            _departmenttree = new DepartmentTreeViewModel(rootDepartment);
                            base.DataContext = _departmenttree;

                            ((DepartmentViewModel)DepartmentTreeView.Items[0]).IsExpanded = true;
                            //((DepartmentViewModel)DepartmentTreeView.Items[0]).IsSelected = true;

                            ((DepartmentViewModel)DepartmentTreeView.Items[0]).Children[__idx].IsSelected = true;
                            
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
                if (DepartmentTreeView.Items.Count > 0)
                {
                    ((DepartmentViewModel)DepartmentTreeView.Items[0]).IsExpanded = true;
                    ((DepartmentViewModel)DepartmentTreeView.Items[0]).IsSelected = true;
                }
                _curSelectedIdx = int.Parse(((DepartmentViewModel)DepartmentTreeView.Items[0]).Idx);
                //_curSelectedIdx = "0";
            }
        }

        void DepartmentTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView selectedTV = (TreeView)e.OriginalSource;

            if (selectedTV.SelectedItem.GetType() != typeof(Com.Huen.DataModel.DepartmentViewModel))
                return;

            DepartmentViewModel selectedTVItem = (DepartmentViewModel)selectedTV.SelectedItem;
            _selectedItem = selectedTVItem;
            _curSelectedIdx = int.Parse(selectedTVItem.Idx);

            if (SelectedDepartmentViewModelChanged != null)
                SelectedDepartmentViewModelChanged(selectedTVItem);
        }

        private Department GetSubDepartTree()
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
                    dt = db.GetDataTableSP("GET_DEPARTMENT_LIST", dt);
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    throw e;
                }
            }

            var value1 = from myRow in dt.AsEnumerable()
                         //where myRow["PART_DEPTH"].ToString() == "1"
                         select myRow;

            Department departSub = new Department() {
                Idx = "0",
                Name = util.LoadProjectResource("TEXT_TREE_DEPART_ROOT", "COMMONRES", "").ToString(),
            };

            List<Department> depart1List = new List<Department>();
            foreach (var row1 in value1)
            {
                Department depart1 = new Department();
                depart1.Idx = row1["PART_IDX"].ToString();
                depart1.Name = row1["PART_NAME"].ToString();

                var value2 = from myRow2 in dt.AsEnumerable()
                             where myRow2["PART_DEPTH"].ToString() == "2" && myRow2["PART_PARENT"].ToString() == row1["PART_IDX"].ToString()
                             select myRow2;
                List<Department> depart2List = new List<Department>();
                foreach (var row2 in value2)
                {
                    Department depart2 = new Department();
                    depart2.Idx = row2["PART_IDX"].ToString();
                    depart2.Name = row2["PART_NAME"].ToString();

                    var value3 = from myRow3 in dt.AsEnumerable()
                                 where myRow3["PART_DEPTH"].ToString() == "3" && myRow3["PART_PARENT"].ToString() == row2["PART_IDX"].ToString()
                                 select myRow3;
                    List<Department> depart3List = new List<Department>();
                    foreach (var row3 in value3)
                    {
                        Department depart3 = new Department();
                        depart3.Idx = row3["PART_IDX"].ToString();
                        depart3.Name = row3["PART_NAME"].ToString();

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

        private DepartmentViewModel __selectedItem;
        private void menuitem1_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.Source;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            TreeView lv = (TreeView)contextMenu.PlacementTarget;
            __selectedItem = (DepartmentViewModel)lv.SelectedItem;

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
            __selectedItem = (DepartmentViewModel)lv.SelectedItem;

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
            dr["DataName"] = "@i_part_idx";
            dr["DataValue"] = __selectedItem.Idx;
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr["DataName"] = "@i_part_name";
            dr["DataValue"] = "";
            dt.Rows.Add(dr);

            using (FirebirdDBHelper db = new FirebirdDBHelper(util.strDBConn))
            {
                try
                {
                    db.BeginTran();
                    db.ExcuteSP("UDT_DEPARTMENT", dt);
                    db.Commit();

                    var __item = rootDepartment.Children.FirstOrDefault(x => x.Idx == __selectedItem.Idx);
                    rootDepartment.Children.Remove(__item);
                    rootDepartment.Children.Sort(new DepartComparer());
                    _departmenttree = new DepartmentTreeViewModel(rootDepartment);
                    base.DataContext = _departmenttree;

                    ((DepartmentViewModel)DepartmentTreeView.Items[0]).IsExpanded = true;
                    ((DepartmentViewModel)DepartmentTreeView.Items[0]).IsSelected = true;
                }
                catch (FirebirdSql.Data.FirebirdClient.FbException fe)
                {
                    db.Rollback();
                    throw fe;
                }
            }
        }

        private void DepartmentTreeView_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            TreeView lv = (TreeView)e.Source;
            ContextMenu cm = lv.ContextMenu;

            foreach (MenuItem mi in cm.Items)
            {
                mi.IsEnabled = true;
            }

            DepartmentViewModel dv = (DepartmentViewModel)lv.SelectedItem;

            string __locSelectedIndex = ((DepartmentViewModel)lv.SelectedItem).Idx;

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
