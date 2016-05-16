using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Com.Huen.Controldata
{
    public class CallListData
    {
        public string call_forward { get; set; }
        public String cus_tel { get; set; }
        public String txtmemo { get; set; }
        public DateTime regdate { get; set; }

        //public ImageSource CallForward
        //{
        //    get
        //    {
        //        if (call_forward == 1)
        //        {
        //            return new BitmapImage(new Uri("images/login_bt_05_on.gif"));
        //            //return new BitmapImage(new Uri("images/outgoing.png"));
        //        }
        //        else
        //        {
        //            return new BitmapImage(new Uri("images/login_bt_05_on.gif"));
        //            //return new BitmapImage(new Uri("images/incomming.png"));
        //        }
        //    }
        //}

        public string IconUri
        {
            get
            {
                if (call_forward == "1")
                {
                    return "images/login_bt_05_on.gif";
                }
                else
                {
                    return "images/login_bt_05_on.gif";
                }
            }
            set
            {
                call_forward = value;
            }
        }

        public string CusTel
        {
            get
            {
                return cus_tel;
            }
        }

        public string CusMemo
        {
            get
            {
                return txtmemo;
            }
        }

        public string RegDate
        {
            get
            {
                return regdate.ToString();
            }
        }
    }
}
