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
    public class CallforToImage : IValueConverter
    {
        public object Convert(object value,
                        Type targetType,
                        object parameter,
                        System.Globalization.CultureInfo culture)
        {
            ImageSource imgsrc = null;

            switch (value.ToString())
            {
                case "I":
                    imgsrc = (ImageSource)util.LoadProjectResource("ic_outgoing", "COMMONRES", "png");
                    break;
                case "O":
                    imgsrc = (ImageSource)util.LoadProjectResource("ic_incomming", "COMMONRES", "png");
                    break;
                default:
                    imgsrc = (ImageSource)util.LoadProjectResource("ic_incomming", "COMMONRES", "png");
                    break;
            }

            return imgsrc;
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
