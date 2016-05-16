using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Windows.Data;

namespace Com.Huen.Converter
{
    public class FeeStatus : IValueConverter
    {

        public object Convert(object value,
                        Type targetType,
                        object parameter,
                        System.Globalization.CultureInfo culture)
        {
            string __value = value.ToString();
            string __param = parameter.ToString();

            System.Windows.Visibility __visibility = System.Windows.Visibility.Visible;

            if (__param == "1")
            {
                if (__value == "1")
                {
                    __visibility = System.Windows.Visibility.Visible;
                }
                else if (__value == "0")
                {
                    __visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else if (__param == "0")
            {
                if (__value == "1")
                {
                    __visibility = System.Windows.Visibility.Collapsed;
                }
                else if (__value == "0")
                {
                    __visibility = System.Windows.Visibility.Visible;
                }
            }

            return __visibility;
        }

        public object ConvertBack(object value,
                                    Type targetType,
                                    object parameter,
                                    System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException("ConvertBack is not supported");
        }

    }
}
