using System;
using System.Threading;
using System.Windows;

namespace CallRecorderAgent
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Mutex mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            string mutexName = "CallRecorderAgent";

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

                this.StartupUri = new Uri("pack://application:,,,/HCSPHONELIBS;component/Views/CallRecorderAgent.xaml", UriKind.RelativeOrAbsolute);
            }
            else
            {
                //MessageBox.Show("Application aleady started");
                Application.Current.Shutdown();
            }
        }



/*
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.StartupUri = new Uri("pack://application:,,,/HCSPHONELIBS;component/Views/CallRecorderAgent.xaml", UriKind.RelativeOrAbsolute);
            //this.StartupUri = new Uri("pack://application:,,,/HCSPHONELIBS;component/Views/CallingPop.xaml", UriKind.RelativeOrAbsolute);

            //this.StartupUri = new Uri("pack://application:,,,/HCSPHONELIBS;component/Views/StandByList.xaml", UriKind.RelativeOrAbsolute);
            //this.StartupUri = new Uri("pack://application:,,,/HCSPHONELIBS;component/Views/MainCallList.xaml", UriKind.RelativeOrAbsolute);
            //this.StartupUri = new Uri("MainWindow.xaml", UriKind.RelativeOrAbsolute);
        }
*/
    }
}
