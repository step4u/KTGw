using System.Windows;
using System.Windows.Input;

using Com.Huen.Views;
using System;

namespace Com.Huen.Commands
{
  /// <summary>
  /// Hides the main window.
  /// </summary>
    public class ExitCallRecoderAgentCommand : CommandBase<ExitCallRecoderAgentCommand>
  {

    public override void Execute(object parameter)
    {
        var __win = Cast<CallRecorderAgent>(GetTaskbarWindow(parameter));
        __win.TrueExit = true;
        __win.Close();
        CommandManager.InvalidateRequerySuggested();
    }


    public override bool CanExecute(object parameter)
    {
      Window win = GetTaskbarWindow(parameter);
      return win != null;
    }

    public static T Cast<T>(object o)
    {
        return (T)o;
    }
  }
}
