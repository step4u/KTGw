using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Com.Huen.Converter
{
    public class SelAllChangeBold : IValueConverter
    {

        public object Convert(object value,
                        Type targetType,
                        object parameter,
                        System.Globalization.CultureInfo culture)
        {
            var __out = FontWeights.Normal;
            string __value = value.ToString();
            //string __param = parameter.ToString();

            if (__value.Trim() == "전체")
            {
                __out = FontWeights.Bold;
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
