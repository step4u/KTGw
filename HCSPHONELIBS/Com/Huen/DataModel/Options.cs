using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Com.Huen.DataModel
{
    public class Options
    {
        private static string _companyname = string.Empty;
        private static string _appname = string.Empty;
        private static string _usersdefaultpath = string.Empty;
        private static string _usersdatapath = string.Empty;
        public static string recserverip { get; set; }
        private static string _dbserverip = "127.0.0.1";
        public static string pbxip { get; set; }
        private static string _savedir = string.Empty;
        public static string filetype { get; set; }
        public static bool autostart { get; set; }

        public static string companyname
        {
            get
            {
                if (string.IsNullOrEmpty(_companyname)) _companyname = "Coretree";
                return _companyname;
            }
            set
            {
                _companyname = value;
            }
        }

        public static string appname
        {
            get
            {
                if (string.IsNullOrEmpty(_appname)) _appname = "CallRecorder";
                return _appname;
            }
            set
            {
                _appname = value;
            }
        }

        public static string usersdefaultpath
        {
            get
            {
                if (string.IsNullOrEmpty(_usersdefaultpath)) _usersdefaultpath = string.Format(@"{0}\{1}", @"C:\Users\Default\AppData\Roaming", Options.companyname);

                if (!Directory.Exists(_usersdefaultpath))
                    Directory.CreateDirectory(_usersdefaultpath);

                return _usersdefaultpath;
            }
            set
            {
                if (!Directory.Exists(value))
                    Directory.CreateDirectory(value);

                _usersdefaultpath = value;
            }
        }

        public static string usersdatapath
        {
            get
            {
                if (string.IsNullOrEmpty(_usersdatapath)) _usersdatapath = string.Format(@"{0}\{1}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Options.companyname);

                if (!Directory.Exists(_usersdatapath))
                    Directory.CreateDirectory(_usersdatapath);

                return _usersdatapath;
            }
            set
            {
                if (!Directory.Exists(value))
                    Directory.CreateDirectory(value);

                _usersdatapath = value;
            }
        }

        public static string dbserverip
        {
            get
            {
                return _dbserverip;
            }
            set
            {
                _dbserverip = value;
            }
        }

        public static string savedir
        {
            get
            {
                if (string.IsNullOrEmpty(_savedir)) _savedir = string.Format(@"{0}\RecFiles", usersdatapath);

                return _savedir;
            }
            set
            {
                _savedir = value;
            }
        }
    }
}
