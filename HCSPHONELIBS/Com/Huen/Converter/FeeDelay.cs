using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Windows.Data;

namespace Com.Huen.Converter
{
    public class FeeDelay : IValueConverter
    {

        public object Convert(object value,
                        Type targetType,
                        object parameter,
                        System.Globalization.CultureInfo culture)
        {
            string __outStr = string.Empty;
            int __value = int.Parse(value.ToString());
            //string __param = parameter.ToString();

            if (__value > 0)
            {
                __outStr = string.Format("{0}개월 연체", __value);
            }
            else
            {
                __outStr = "연체없음";
            }

            return __outStr;
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
