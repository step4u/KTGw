using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;

using Com.Huen.Libs;

namespace Com.Huen.Converter
{
    public class CallStatusFontWeight : IValueConverter
    {
        public object Convert(object value,
        Type targetType,
        object parameter,
        System.Globalization.CultureInfo culture)
        {
            string val = value.ToString();

            var weight = FontWeights.Normal;

            switch (val)
            {
                case "0":
                    weight = FontWeights.Normal;
                    break;
                case "1":
                    weight = FontWeights.Bold;
                    break;
            }

            return weight;
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
