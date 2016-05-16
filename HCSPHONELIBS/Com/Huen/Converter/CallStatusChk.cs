using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;

using Com.Huen.Libs;


namespace Com.Huen.Converter
{
    public class CallStatusChk : IValueConverter
    {
        public object Convert(object value,
                Type targetType,
                object parameter,
                System.Globalization.CultureInfo culture)
        {
            string ostr = string.Empty;
            string val = value.ToString();

            switch(val)
            {
                case "0":
                    ostr = "통화가능";
                    break;
                case "1":
                    ostr = "통화불가";
                    break;
            }

            return ostr;
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
