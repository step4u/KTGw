using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;

namespace Com.Huen.DataModel
{
    public class Options
    {
        private static string _companyname = string.Empty;
        private static string _appname = string.Empty;
        private static string _userdefaultpath = string.Empty;
        private static string _userappdatapath = string.Empty;
        private static string _programdatapath = string.Empty;
        public static string recserverip { get; set; }
        private static string _dbserverip = "127.0.0.1";
        public static string pbxip { get; set; }
        private static string _savedir = string.Empty;
        public static string filetype { get; set; }
        public static bool autostart { get; set; }
        public static String[] recextensions;

        public static string CompanyName
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

        public static string AppName
        {
            get
            {
                if (string.IsNullOrEmpty(_appname))
                    _appname = "CallRecorder";

                return _appname;
            }
            set
            {
                _appname = value;
            }
        }

        public static string UserDefaultPath
        {
            get
            {
                if (string.IsNullOrEmpty(_userdefaultpath))
                {
                    using (RegistryKey profileListKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList"))
                    {
                        _userdefaultpath = string.Format(@"{0}\AppData\Roaming\{2}", profileListKey.GetValue("Default").ToString(), CompanyName);
                    }
                }

                if (!Directory.Exists(_userdefaultpath))
                    Directory.CreateDirectory(_userdefaultpath);

                return _userdefaultpath;
            }
            set
            {
                if (!Directory.Exists(value))
                    Directory.CreateDirectory(value);

                _userdefaultpath = value;
            }
        }

        public static string UserAppdataPath
        {
            get
            {
                if (string.IsNullOrEmpty(_userappdatapath))
                    _userappdatapath = string.Format(@"{0}\{1}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), CompanyName);

                if (!Directory.Exists(_userappdatapath))
                    Directory.CreateDirectory(_userappdatapath);

                return _userappdatapath;
            }
            set
            {
                if (!Directory.Exists(value))
                    Directory.CreateDirectory(value);

                _userappdatapath = value;
            }
        }

        public static string ProgramDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(_programdatapath))
                {
                    using (RegistryKey profileListKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList"))
                    {
                        _programdatapath = string.Format(@"{0}\{1}", profileListKey.GetValue("ProgramData").ToString(), CompanyName);
                    }
                }

                if (!Directory.Exists(_programdatapath))
                    Directory.CreateDirectory(_programdatapath);

                return _programdatapath;
            }
            set
            {
                if (!Directory.Exists(value))
                    Directory.CreateDirectory(value);

                _programdatapath = value;
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
                if (string.IsNullOrEmpty(_savedir))
                    _savedir = string.Format(@"{0}\RecFiles", UserAppdataPath);

                return _savedir;
            }
            set
            {
                _savedir = value;
            }
        }
    }
}
