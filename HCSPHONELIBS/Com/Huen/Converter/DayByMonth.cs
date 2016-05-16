using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Windows.Data;

namespace Com.Huen.Converter
{
    public class DayByMonth : IValueConverter
    {

        public object Convert(object value,
                        Type targetType,
                        object parameter,
                        System.Globalization.CultureInfo culture)
        {
            int selyy = int.Parse((string)parameter);
            int selmm = int.Parse((string)value);

            DateTime date = new DateTime(selyy, selmm, DateTime.Now.Day);
            date = date.AddMonths(1);
            date = date.AddDays(-date.Day);
            int monthdays = date.Day;

            DataTable dt = new DataTable();
            dt.Columns.Add("txtDay", typeof(string));
            dt.Columns.Add("valDay", typeof(string));

            for (int i = 1; i <= monthdays; i++)
            {
                string day = i.ToString();

                if (day.Length == 1)
                    day = string.Format("{0}{1}", "0", day);

                dt.Rows.Add(day + "일", day);
            }

            return dt;
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
