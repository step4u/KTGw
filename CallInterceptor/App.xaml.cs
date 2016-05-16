using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;

namespace CallInterceptor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Mutex mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            string mutexName = "HuenCallInterceptor";

            try
            {
                mutex = new Mutex(false, mutexName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Current.Shutdown();
            }

            if (mutex.WaitOne(0, false))
            {
                base.OnStartup(e);

                this.StartupUri = new Uri("pack://application:,,,/HCSPHONELIBS;component/Views/SearchNintercept.xaml", UriKind.RelativeOrAbsolute);
            }
            else
            {
                //MessageBox.Show("Application aleady started");
                Application.Current.Shutdown();
            }
        }


        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    base.OnStartup(e);

        //    this.StartupUri = new Uri("pack://application:,,,/HCSPHONELIBS;component/Views/SearchNintercept.xaml", UriKind.RelativeOrAbsolute);
        //}
    }
}
