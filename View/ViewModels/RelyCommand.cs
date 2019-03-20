using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace View.ViewModels
{
    public class RelayCommand : ICommand
    {
        private Action mAction;

        public RelayCommand(Action mAction)
        {
            this.mAction = mAction;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            mAction();
        }

        public event EventHandler CanExecuteChanged;
    }
    public class RelayCommand<T> : ICommand
    {
        private Action<T> mAction;

        public RelayCommand(Action<T> execute)
        {
            mAction = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public virtual void Execute(object parameter)
        {
            mAction((T)(object)int.Parse((string)parameter));
        }
    }
}
