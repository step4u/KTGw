using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Media;

namespace Com.Huen.Libs
{
    public class DllResources
    {
        public DllResources() { }

        public static string strConn
        {
            get
            {
                return (string)util.LoadProjectResource("DBCONSTR_MSSQL", "COMMONRES", "");
            }
        }

        public static ImageSource notifyicon
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("icon", "COMMONRES", "");
            }
        }

        public static ImageSource icon
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("icon", "COMMONRES", "ico");
            }
        }

        public static ImageSource ic_refresh
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_refresh", "COMMONRES", "png");
            }
        }

        public static ImageSource ic_play
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_play", "COMMONRES", "png");
            }
        }

        public static ImageSource ic_pause
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_pause", "COMMONRES", "png");
            }
        }

        public static ImageSource ic_statis
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_statis", "COMMONRES", "png");
            }
            
        }

        public static ImageSource ic_completed
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_completed", "COMMONRES", "png");
            }
        }

        public static ImageSource ic_props
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_props", "COMMONRES", "png");
            }
        }

        public static ImageSource ic_plus2
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_plus2", "COMMONRES", "png");
            }
        }

        public static ImageSource ic_close
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_close", "COMMONRES", "png");
            }
        }

        public static ImageSource ic_info
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_info", "COMMONRES", "png");
            }
        }

        public static ImageSource ic_props2
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_props2", "COMMONRES", "png");
            }
        }

        public static ImageSource ic_smses
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_smses", "COMMONRES", "png");
            }
        }

        public static ImageSource ic_birthday
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_birthday", "COMMONRES", "png");
            }
        }

        public static ImageSource ic_fees
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_fees", "COMMONRES", "png");
            }
        }

        public static ImageSource ic_attention
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_attention", "COMMONRES", "png");
            }
        }

        public static ImageSource ic_options
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_options", "COMMONRES", "png");
            }
        }

        public static ImageSource ic_att
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_att", "COMMONRES", "png");
            }
        }

        public static ImageSource ic_cake
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_cake", "COMMONRES", "png");
            }
        }

        public static ImageSource ic_options1
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_options1", "COMMONRES", "png");
            }
        }

        public static ImageSource ic_smses1
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_smses1", "COMMONRES", "png");
            }
        }

        public static ImageSource ic_won
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_won", "COMMONRES", "png");
            }
        }

        public static ImageSource ico_sms
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ico_sms", "COMMONRES", "png");
            }
        }

        public static ImageSource ico_currency
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ico_currency", "COMMONRES", "png");
            }
        }

        public static ImageSource ico_gear
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ico_gear", "COMMONRES", "png");
            }
        }

        public static ImageSource ico_cake
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ico_cake", "COMMONRES", "png");
            }
        }

        public static ImageSource ico_warning
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ico_warning", "COMMONRES", "png");
            }
        }

        public static ImageSource ico_attention
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ico_attention", "COMMONRES", "png");
            }
        }

        public static ImageSource smartphone1
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("smartphone1", "COMMONRES", "png");
            }
        }

        public static ImageSource smartphone2
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("smartphone2", "COMMONRES", "png");
            }
        }

        public static ImageSource smartphone3
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("smartphone3", "COMMONRES", "png");
            }
        }

        public static ImageSource ic_sms
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("ic_sms", "COMMONRES", "png");
            }
        }

        public static ImageSource eraser0
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("eraser0", "COMMONRES", "png");
            }
        }

        public static ImageSource eraser1
        {
            get
            {
                return (ImageSource)util.LoadProjectResource("eraser1", "COMMONRES", "png");
            }
        }

        public static string TEXT_MAIN_TITLE
        {
            get
            {
                return (string)util.LoadProjectResource("TEXT_MAIN_TITLE", "COMMONRES", "");
            }
        }

        public static string TEXT_MAINPROPS_TITLE
        {
            get
            {
                return (string)util.LoadProjectResource("TEXT_MAINPROPS_TITLE", "COMMONRES", "");
            }
        }

        public static string LABEL_MAINPROPS_PBXIP
        {
            get
            {
                return (string)util.LoadProjectResource("LABEL_MAINPROPS_PBXIP", "COMMONRES", "");
            }
        }

        public static string LABEL_MAINPROPS_PBXPORT
        {
            get
            {
                return (string)util.LoadProjectResource("LABEL_MAINPROPS_PBXPORT", "COMMONRES", "");
            }
        }

        public static string LABEL_MAINPROPS_USERID
        {
            get
            {
                return (string)util.LoadProjectResource("LABEL_MAINPROPS_USERID", "COMMONRES", "");
            }
        }

        public static string LABEL_MAINPROPS_AUTOSTART
        {
            get
            {
                return (string)util.LoadProjectResource("LABEL_MAINPROPS_AUTOSTART", "COMMONRES", "");
            }
        }

        public static string BTN_SUBMIT_TEXT
        {
            get
            {
                return (string)util.LoadProjectResource("BTN_SUBMIT_TEXT", "COMMONRES", "");
            }
        }

        public static string BTN_CANCEL_TEXT
        {
            get
            {
                return (string)util.LoadProjectResource("BTN_CANCEL_TEXT", "COMMONRES", "");
            }
        }

        public static string BTN_CLOSE_TEXT
        {
            get
            {
                return (string)util.LoadProjectResource("BTN_CLOSE_TEXT", "COMMONRES", "");
            }
        }

        public static string DEFAULT_PBX_IPADDRESS
        {
            get
            {
                return (string)util.LoadProjectResource("DEFAULT_PBX_IPADDRESS", "COMMONRES", "");
            }
        }

        public static string DEFAULT_PBX_PORT
        {
            get
            {
                return (string)util.LoadProjectResource("DEFAULT_PBX_PORT", "COMMONRES", "");
            }
        }
    }
}
