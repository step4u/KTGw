using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using Com.Huen.Libs;
using Com.Huen.DataModel;


namespace Com.Huen.Converter
{
    public class IndexConverter4Paging : IMultiValueConverter
    {
        public object Convert(object[] value,
                        Type targetType,
                        object parameter,
                        System.Globalization.CultureInfo culture)
        {
            ListViewItem item = (ListViewItem)value[0];
            Interview __item = (Interview)item.Content;
            ListView __lv = (ListView)value[1];
            ObservableCollection<Interview> __sourcecollection = (ObservableCollection<Interview>)((CollectionView)__lv.ItemsSource).SourceCollection;
            //ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            //int index = listView.Items.Count - listView.ItemContainerGenerator.IndexFromContainer(item);
            int index = __sourcecollection.Count - __sourcecollection.IndexOf(__item);
            //int index = __sourcecollection.Count - 1;
            return index.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
