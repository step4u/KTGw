using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Windows.Data;
using System.Windows.Media;

namespace Com.Huen.Converter
{
    public class FeeDelayColor : IValueConverter
    {

        public object Convert(object value,
                        Type targetType,
                        object parameter,
                        System.Globalization.CultureInfo culture)
        {
            SolidColorBrush __out = new SolidColorBrush(Colors.Black);
            int __value = int.Parse(value.ToString());
            //string __param = parameter.ToString();

            if (__value > 0)
            {
                __out = new SolidColorBrush(Colors.Red);
            }
            else
            {
                __out = new SolidColorBrush(Colors.Green);
            }

            return __out;
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
