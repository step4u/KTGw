using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.AccessControl;
using Microsoft.Win32;
using System.Security.Permissions;

namespace Com.Huen.Libs
{
    public class ModifyRegistry
    {
        public ModifyRegistry() { }

        public ModifyRegistry(string regsubkey)
        {
            RegSubKey = regsubkey;
        }

        private string _regsubkey = string.Empty;
        public string RegSubKey
        {
            get
            {
                if (string.IsNullOrEmpty(_regsubkey))
                {
                    if (System.Environment.Is64BitOperatingSystem)
                    {
                        _regsubkey = util.LoadProjectResource("REG_SUBKEY64", "COMMONRES", "").ToString();
                    }
                    else
                    {
                        _regsubkey = util.LoadProjectResource("REG_SUBKEY32", "COMMONRES", "").ToString();
                    }
                }
                return _regsubkey;
            }
            set
            {
                _regsubkey = value;
            }
        }


        public void SetValue(RegKind regKind, RegistryValueKind regType, string keyName, object keyValue)
        {
            try
            {
                RegistryKey key = null;

                switch (regKind)
                {
                    case RegKind.LocalMachine:
                        key = Registry.LocalMachine.OpenSubKey(RegSubKey, true);
                        if (key == null)
                        {
                            //RegistrySecurity mSecurity = new RegistrySecurity();
                            //string user = Environment.UserDomainName + "\\" + Environment.UserName;
                            //RegistryAccessRule rule = new RegistryAccessRule(
                            //    "Everyone",
                            //    RegistryRights.FullControl,
                            //    InheritanceFlags.None,
                            //    PropagationFlags.None,
                            //    AccessControlType.Allow
                            //    );
                            //mSecurity.AddAccessRule(rule);

                            //key = Registry.LocalMachine.CreateSubKey(RegSubKey, RegistryKeyPermissionCheck.ReadWriteSubTree, mSecurity);
                            key = Registry.LocalMachine.CreateSubKey(RegSubKey);
                        }

                        break;
                    case RegKind.CurrentUser:
                        key = Registry.CurrentUser.OpenSubKey(RegSubKey, true);
                        if (key == null)
                            Registry.CurrentUser.CreateSubKey(RegSubKey);
                        break;
                }

                key.SetValue(keyName, keyValue, regType);
                key.Flush();
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public void SetValue(RegKind regKind, string keyName, object keyValue)
        {
            RegistryKey key = null;

            switch (regKind)
            {
                case RegKind.LocalMachine:
                    key = Registry.LocalMachine.OpenSubKey(RegSubKey, true);
                    if (key == null)
                        Registry.LocalMachine.CreateSubKey(RegSubKey);
                    break;
                case RegKind.CurrentUser:
                    key = Registry.CurrentUser.OpenSubKey(RegSubKey, true);
                    if (key == null)
                        Registry.CurrentUser.CreateSubKey(RegSubKey);
                    break;
            }

            key.SetValue(keyName, keyValue);
            key.Flush();
        }

        public object GetValue(RegKind regKind, string keyName)
        {
            RegistryKey key = null;
            Object rtnObj = null;

            switch (regKind)
            {
                case RegKind.LocalMachine:
                    key = Registry.LocalMachine.OpenSubKey(RegSubKey, true);
                    break;
                case RegKind.CurrentUser:
                    key = Registry.CurrentUser.OpenSubKey(RegSubKey, true);
                    break;
            }

            try
            {
                rtnObj = key.GetValue(keyName);
            }
            catch(Exception e)
            {
                rtnObj = String.Empty;
            }

            return rtnObj;
        }

        public void DeleteValue(RegKind regKind, string keyName)
        {
            RegistryKey key = null;

            switch (regKind)
            {
                case RegKind.LocalMachine:
                    key = Registry.LocalMachine.OpenSubKey(RegSubKey, true);
                    break;
                case RegKind.CurrentUser:
                    key = Registry.CurrentUser.OpenSubKey(RegSubKey, true);
                    break;
            }

            if (key == null)
                return;

            key.DeleteValue(keyName, false);
        }

        public void CreateSubKey(RegKind regKind, string subKey)
        {
            switch (regKind)
            {
                case RegKind.LocalMachine:
                    Registry.LocalMachine.CreateSubKey(subKey);
                    break;
                case RegKind.CurrentUser:
                    Registry.CurrentUser.CreateSubKey(subKey);
                    break;
            }
        }

        public void DeleteSubKey(RegKind regKind, string subKey)
        {
            switch (regKind)
            {
                case RegKind.LocalMachine:
                    Registry.LocalMachine.DeleteSubKey(subKey);
                    break;
                case RegKind.CurrentUser:
                    Registry.CurrentUser.DeleteSubKey(subKey, false);
                    break;
            }
        }

        public void DeleteSubKey(RegKind regKind)
        {
            switch (regKind)
            {
                case RegKind.LocalMachine:
                    Registry.LocalMachine.DeleteSubKey(RegSubKey);
                    break;
                case RegKind.CurrentUser:
                    Registry.CurrentUser.DeleteSubKey(RegSubKey, false);
                    break;
            }
        }

        public void DeleteSubKeyTree(RegKind regKind, string subkeytree)
        {
            switch(regKind)
            {
                case RegKind.LocalMachine:
                    Registry.LocalMachine.DeleteSubKeyTree(subkeytree);
                    break;
                case RegKind.CurrentUser:
                    Registry.CurrentUser.DeleteSubKeyTree(subkeytree, false);
                    break;
            }
        }

        public void DeleteSubKeyTree(RegKind regKind)
        {
            switch (regKind)
            {
                case RegKind.LocalMachine:
                    Registry.LocalMachine.DeleteSubKeyTree(RegSubKey);
                    break;
                case RegKind.CurrentUser:
                    Registry.CurrentUser.DeleteSubKeyTree(RegSubKey, false);
                    break;
            }
        }
    }
}
