using System.Windows;
using Com.Huen.DataModel;

namespace Com.Huen.Views
{
    /// <summary>
    /// Interaction logic for InnerTelWin.xaml
    /// </summary>
    public partial class InnerTelWin : Window
    {
        public SearchProperties parentWin = null;
        public InnerTel innertel;
        private InnerTelAction _action = InnerTelAction.None;

        public InnerTelWin()
        {
            InitializeComponent();

            this.Loaded += InnerTelWin_Loaded;
        }

        void InnerTelWin_Loaded(object sender, RoutedEventArgs e)
        {
            parentWin = (SearchProperties)this.Owner;

            if (innertel == null)
            {
                _action = InnerTelAction.Add;
                btnAdd.Content = "추가";
            }
            else
            {
                txtTelnum.Text = innertel.Telnum;
                txtTellername.Text = innertel.TellerName;

                _action = InnerTelAction.Modify;
                txtTelnum.IsReadOnly = true;
                btnAdd.Content = "수정";
            }

            txtTelnum.Focus();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_action == InnerTelAction.Add)
            {
                InnerTel __innertel = new InnerTel();
                __innertel.Telnum = txtTelnum.Text.Trim();
                __innertel.TellerName = txtTellername.Text.Trim();
                parentWin.innertels.Add(__innertel);

                __innertel.Telnum = txtTelnum.Text.Trim();
                __innertel.TellerName = txtTellername.Text.Trim();
                this.txtTelnum.Focus();
            }
            else if (_action == InnerTelAction.Modify)
            {
                innertel.Telnum = txtTelnum.Text.Trim();
                innertel.TellerName = txtTellername.Text.Trim();
                parentWin.innertels.Modify(innertel);
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        enum InnerTelAction
        {
            None,
            Add,
            Modify,
        }
    }
}
