using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using System.Data;

using Com.Huen.Libs;
using Com.Huen.Sql;

namespace Com.Huen.Converter
{
    public class Tel2CustInfo : IValueConverter
    {
        public object Convert(object value,
                        Type targetType,
                        object parameter,
                        System.Globalization.CultureInfo culture)
        {
            string _tel = value.ToString();

            if (!string.IsNullOrEmpty(_tel))
            {
                DataTable __dt = null;
                StringBuilder _sql = new StringBuilder();

                _sql.AppendFormat(" select company from VW_DRUGMEMBER where replace(phone, '-', '') = '{0}' ", _tel.Trim());

                using (MSDBHelper db = new MSDBHelper(_sql.ToString(), util.strDBConn))
                {
                    try
                    {
                        __dt = db.GetDataTable();
                    }
                    catch (System.Data.SqlClient.SqlException __se)
                    {

                    }
                }

                if (__dt.Rows.Count > 0)
                {
                    _tel = string.Format("{0}({1})", __dt.Rows[0][0].ToString(), _tel);
                }
            }

            return _tel;
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
