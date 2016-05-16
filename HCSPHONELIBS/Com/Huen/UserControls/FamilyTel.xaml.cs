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

namespace Com.Huen.UserControls
{
    /// <summary>
    /// Interaction logic for FamilyTel.xaml
    /// </summary>
    public partial class FamilyTel : UserControl
    {
        public delegate void FamilyTelDeleteHandler(object sender);
        public event FamilyTelDeleteHandler FamTelDelete;

        public FamilyTel()
        {
            InitializeComponent();

            ///base.DataContext = new Com.Huen.DataModel.FamilyRoles();
        }

        private void ButtonMinus_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (FamTelDelete != null)
                FamTelDelete(this);
        }
    }
}
