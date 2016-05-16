using System;
using System.Collections.Generic;
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

using Com.Huen.DataModel;
using Com.Huen.Libs;

namespace Com.Huen.UserControls
{
	/// <summary>
	/// Interaction logic for divEventControl.xaml
	/// </summary>
	public partial class divEventControl : UserControl
	{
        public delegate void DivEventAddEventListHandler(Object sender);
        public event DivEventAddEventListHandler AddEventList;
        public event DivEventAddEventListHandler Close;

        #region dependency property

        /// <summary>
        /// Description
        /// </summary>
        public static readonly DependencyProperty EVTITLEProperty =
            DependencyProperty.Register("EVTITLE",
                                        typeof(string),
                                        typeof(divEventControl),
                                        new FrameworkPropertyMetadata("주의사항"));

        /// <summary>
        /// A property wrapper for the <see cref="CHIDXProperty"/>
        /// dependency property:<br/>
        /// Description
        /// </summary>
        public string EVTITLE
        {
            get { return (string)GetValue(EVTITLEProperty); }
            set { SetValue(EVTITLEProperty, value); }
        }
        #endregion dependency property

        public EventSchedule Evt = null;

		public divEventControl()
		{
			this.InitializeComponent();

            this.Loaded += divEventControl_Loaded;
		}

        void divEventControl_Loaded(object sender, RoutedEventArgs e)
        {
            evtTitle.Focus();
        }

        private void ButtonX_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close(this);
        }

        private void evtBtn0_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(evtTitle.Text.Trim()))
            {
                evtTitle.Focus();
                return;
            }

            Evt = new EventSchedule() {
                COM_IDX = util.Userinfo.COM_IDX
                , EVT_TITLE = evtTitle.Text.Trim()
                , EVT_PLACE = evtPlace.Text.Trim()
                , EVT_REPEAT = evtRepeat.IsChecked == true ? true : false
                , EVT_SDATE = new DateTime(evtSdate.SelectedDate.Value.Year, evtSdate.SelectedDate.Value.Month, evtSdate.SelectedDate.Value.Day, evtStime.Value.Value.Hour, evtStime.Value.Value.Minute, evtStime.Value.Value.Second)
                , EVT_EDATE = new DateTime(evtEdate.SelectedDate.Value.Year, evtEdate.SelectedDate.Value.Month, evtEdate.SelectedDate.Value.Day, evtEtime.Value.Value.Hour, evtEtime.Value.Value.Minute, evtEtime.Value.Value.Second)
                , EVT_MEMO = evtMemo.Text.Trim()
            };

            for (int i = 0; i < grid_RepeatDay.Children.Count; i++)
            {
                CheckBox __chk = ((CheckBox)grid_RepeatDay.Children[i]);
                int __weekday = i + 1;

                if (__weekday == 7) __weekday = 0;

                if (__chk.IsChecked == true ? true : false)
                {
                    if (string.IsNullOrEmpty(Evt.EVT_REPEATDAY))
                        Evt.EVT_REPEATDAY += string.Format("{0}", __weekday);
                    else
                        Evt.EVT_REPEATDAY += string.Format(",{0}", __weekday);
                }
            }

            AddEventList(this);
        }

        private void evtBtn1_Click(object sender, RoutedEventArgs e)
        {
            Close(this);
        }
	}
}