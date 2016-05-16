using System.Windows;
using System.Windows.Input;

namespace Com.Huen.Commands
{
  /// <summary>
  /// Hides the main window.
  /// </summary>
    public class HideCallRecorderAgentCommand : CommandBase<HideCallRecorderAgentCommand>
  {

    public override void Execute(object parameter)
    {
        GetTaskbarWindow(parameter).Hide();
        CommandManager.InvalidateRequerySuggested();
    }


    public override bool CanExecute(object parameter)
    {
      Window win = GetTaskbarWindow(parameter);
      return win != null && win.IsVisible;
    }


  }
}
