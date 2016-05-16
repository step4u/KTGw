using System.Windows;
using System.Windows.Input;

using Com.Huen.Views;
using System;
using System.ServiceProcess;

namespace Com.Huen.Commands
{
    public class ServiceStartCommand : CommandBase<ServiceStartCommand>
    {
        private ServiceController _controller;

        public override void Execute(object parameter)
        {
            _controller = new ServiceController("Huen Call Recorder");

            if (_controller.Status == ServiceControllerStatus.Stopped)
                _controller.Start();

            CommandManager.InvalidateRequerySuggested();
        }

        public override bool CanExecute(object parameter)
        {
            _controller = new ServiceController("Huen Call Recorder");

            bool __status = false;

            if (_controller.Status == ServiceControllerStatus.Stopped || _controller.Status == ServiceControllerStatus.StopPending)
                __status = true;
            else
                __status = false;

            return __status;
        }

        public static T Cast<T>(object o)
        {
            return (T)o;
        }
    }
}
