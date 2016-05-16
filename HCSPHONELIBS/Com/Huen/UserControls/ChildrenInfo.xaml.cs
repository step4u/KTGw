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

using System.Collections.ObjectModel;
using System.Windows.Media.Animation;
using Com.Huen.DataModel;

namespace Com.Huen.UserControls
{
    /// <summary>
    /// Interaction logic for ChildrenInfo.xaml
    /// </summary>
    /// 
    public partial class ChildrenInfo : UserControl
    {
        public delegate void AddChildrenEventHandler(object sender);
        public event AddChildrenEventHandler AddAttentionEvent;

        //private Storyboard SB_Attention_ON = null;
        //private Storyboard SB_Attention_OFF = null;

        public string CH_Idx { get; set; }

        #region dependency property

        /// <summary>
        /// Description
        /// </summary>
        public static readonly DependencyProperty CHIDXProperty =
            DependencyProperty.Register("CHIDX",
                                        typeof(string),
                                        typeof(ChildrenInfo),
                                        new FrameworkPropertyMetadata(""));

        /// <summary>
        /// A property wrapper for the <see cref="CHIDXProperty"/>
        /// dependency property:<br/>
        /// Description
        /// </summary>
        public string CHIDX
        {
            get { return (string)GetValue(CHIDXProperty); }
            set { SetValue(CHIDXProperty, value); }
        }


        public static readonly DependencyProperty CHNAMEProperty =
            DependencyProperty.Register("CHNAME",
                                        typeof(string),
                                        typeof(ChildrenInfo),
                                        new FrameworkPropertyMetadata(""));

        /// <summary>
        /// A property wrapper for the <see cref="CHNAMEProperty"/>
        /// dependency property:<br/>
        /// Description
        /// </summary>
        public string CHNAME
        {
            get { return (string)GetValue(CHNAMEProperty); }
            set { SetValue(CHNAMEProperty, value); }
        }


        public static readonly DependencyProperty CHSEXProperty =
            DependencyProperty.Register("CHSEX",
                                        typeof(string),
                                        typeof(ChildrenInfo),
                                        new FrameworkPropertyMetadata(""));

        /// <summary>
        /// A property wrapper for the <see cref="CHSEXProperty"/>
        /// dependency property:<br/>
        /// Description
        /// </summary>
        public string CHSEX
        {
            get { return (string)GetValue(CHSEXProperty); }
            set { SetValue(CHSEXProperty, value); }
        }


        public static readonly DependencyProperty CHBIRTHProperty =
            DependencyProperty.Register("CHBIRTH",
                                        typeof(DateTime),
                                        typeof(ChildrenInfo),
                                        new FrameworkPropertyMetadata(DateTime.Now));

        /// <summary>
        /// A property wrapper for the <see cref="CHBIRTHProperty"/>
        /// dependency property:<br/>
        /// Description
        /// </summary>
        public DateTime CHBIRTH
        {
            get { return (DateTime)GetValue(CHBIRTHProperty); }
            set { SetValue(CHBIRTHProperty, value); chBirth.SelectedDate = value; }
        }


        public static readonly DependencyProperty CHENTProperty =
            DependencyProperty.Register("CHENT",
                                        typeof(DateTime),
                                        typeof(ChildrenInfo),
                                        new FrameworkPropertyMetadata(DateTime.Now));

        /// <summary>
        /// A property wrapper for the <see cref="CHBIRTHProperty"/>
        /// dependency property:<br/>
        /// Description
        /// </summary>
        public DateTime CHENT
        {
            get { return (DateTime)GetValue(CHENTProperty); }
            set { SetValue(CHENTProperty, value); chEnter.SelectedDate = value; }
        }


        public static readonly DependencyProperty CHGRADUATEProperty =
            DependencyProperty.Register("CHGRADUATE",
                                        typeof(DateTime),
                                        typeof(ChildrenInfo),
                                        new FrameworkPropertyMetadata(DateTime.Now));

        /// <summary>
        /// A property wrapper for the <see cref="CHGRADUATEProperty"/>
        /// dependency property:<br/>
        /// Description
        /// </summary>
        public DateTime CHGRADUATE
        {
            get { return (DateTime)GetValue(CHGRADUATEProperty); }
            set { SetValue(CHGRADUATEProperty, value); chGraduate.SelectedValue = value; }
        }
        #endregion

        private ObservableCollection<EventSchedule> _attentionList = new ObservableCollection<EventSchedule>();

        public EventSchedule AddAttention
        {
            set
            {
                _attentionList.Add((EventSchedule)value);
            }
        }

        public ChildrenInfo()
        {
            InitializeComponent();

            this.Loaded += ChildrenInfo_Loaded;
        }

        void ChildrenInfo_Loaded(object sender, RoutedEventArgs e)
        {
            //SB_Attention_ON = (Storyboard)FindResource("gridAttention_ON");
            //SB_Attention_ON.Completed += SB_Attention_ON_Completed;

            //SB_Attention_OFF = (Storyboard)FindResource("gridAttention_OFF");
            //SB_Attention_OFF.Completed += SB_Attention_OFF_Completed;

            listAttention.ItemsSource = _attentionList;
        }

        //void SB_Attention_OFF_Completed(object sender, EventArgs e)
        //{
        //    buttonPlus2.IsEnabled = true;
        //}

        //void SB_Attention_ON_Completed(object sender, EventArgs e)
        //{
        //    buttonPlus2.IsEnabled = false;
        //}

        //private void txtAttention_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Escape)
        //    {
        //        SB_Attention_OFF.Begin();
        //    }
        //    else if (e.Key == Key.Enter)
        //    {
        //        // listAttention 입력
        //    }
        //}

        //private void txtAttention_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        if (string.IsNullOrEmpty(txtAttention.Text.Trim()))
        //        {
        //            MessageBox.Show("주의 사항을 입력하세요.");
        //            txtAttention.Focus();
        //            return;
        //        }
        //    }
        //}

        private void buttonPlus2_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (AddAttentionEvent != null)
                AddAttentionEvent(this);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)e.Source;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            ListView lv = (ListView)contextMenu.PlacementTarget;
            EventSchedule obj = (EventSchedule)lv.SelectedItem;

            if (string.IsNullOrEmpty(obj.EVT_IDX))
            {
                _attentionList.Remove(obj);
            }
            else
            {
                // DB 에서도 제거

                _attentionList.Remove(obj);
            }
        }

        private void listAttention_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ListView lv = (ListView)e.Source;
            ContextMenu cm = lv.ContextMenu;

            if (!lv.HasItems)
            {
                foreach (MenuItem item in cm.Items)
                {
                    item.IsEnabled = false;
                }
            }
        }
    }
}
