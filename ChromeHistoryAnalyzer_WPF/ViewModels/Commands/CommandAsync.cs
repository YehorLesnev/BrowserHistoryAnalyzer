using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace BrowserHistoryAnalyzer_WPF.ViewModels.Commands
{
    public abstract class CommandAsync : ICommandAsync
    {
        private ObservableCollection<Task> runningTasks;

        protected CommandAsync()
        {
            runningTasks = new ObservableCollection<Task>();
            runningTasks.CollectionChanged += OnRunningTaskChanged;
        }

        private void OnRunningTaskChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public IEnumerable<Task> RunningTasks => runningTasks;

        bool ICommand.CanExecute(object? parameter)
        {
            return CanExecute();
        }

        async void ICommand.Execute(object? parameter)
        {
            Task runningTask = ExecuteAsync();

            runningTasks.Add(runningTask);

            try
            {
                await runningTask;
            }
            finally  
            {
                runningTasks.Remove(runningTask);
            }
        }

        public abstract bool CanExecute();
        public abstract Task ExecuteAsync();
    }
}
