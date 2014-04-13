namespace Vido.Parking.Ms.Ui.Wpf.Commands
{
  using System;
  using System.Diagnostics;
  using System.Windows.Input;
  public class RelayCommand<T>:ICommand
  {
    #region Data Members
    private readonly Action<T> execute = null;
    private readonly Predicate<T> canExecute = null;
    #endregion

    #region Constructors
    public RelayCommand(Action<T> execute)
      :this(execute, null)
    { }

    /// <summary>
    /// Creates a new command.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public RelayCommand(Action<T> execute, Predicate<T> canExecute)
    {
      if (execute == null)
        throw new ArgumentNullException("execute");
      
      this.execute = execute;
      this.canExecute = canExecute;
    }
    #endregion

    #region ICommand Members
    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
      return (canExecute == null ? true : canExecute((T)parameter));
    }
    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }
    public void Execute(object parameter)
    {
      execute((T)parameter);
    }
    #endregion // ICommand Members
  }

  public class RelayCommand:ICommand
  {
    #region Data Members
    private readonly Action<object> execute = null;
    private readonly Predicate<object> canExecute = null;
    #endregion

    #region Constructors
    public RelayCommand(Action<object> execute)
      :this(execute, null)
    { }

    /// <summary>
    /// Creates a new command.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public RelayCommand(Action<object> execute, Predicate<object> canExecute)
    {
      if (execute == null)
        throw new ArgumentNullException("execute");
      
      this.execute = execute;
      this.canExecute = canExecute;
    }
    #endregion

    #region ICommand Members
    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
      return (canExecute == null ? true : canExecute(parameter));
    }
    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }
    public void Execute(object parameter)
    {
      execute(parameter);
    }
    #endregion // ICommand Members
  }
}
