﻿using System;
using System.Windows.Input;

namespace ViewModel
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

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67
    }
    public class RelayCommand<T> : ICommand
    {
        private Action<T> mAction;

        public RelayCommand(Action<T> execute)
        {
            mAction = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public virtual void Execute(object parameter)
        {
            mAction((T)(object)int.Parse((string)parameter));
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67
    }
}
