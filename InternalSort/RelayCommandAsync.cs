using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InternalSort
{
    internal class RelayCommandAsync : ICommand
    {
        private bool isExecuting;
        private readonly Func<Task> asyncDelegate;
        private bool IsExecuting
        {
            get => isExecuting;
            set
            {
                isExecuting = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler? CanExecuteChanged;

        public RelayCommandAsync(Func<Task> asyncDelegate)
        {
            this.asyncDelegate = asyncDelegate;
        }

        public bool CanExecute(object? parameter)
        {
            return !IsExecuting;
        }

        public async void Execute(object? parameter)
        {
            IsExecuting = true;

            await ExecuteAsync();

            IsExecuting = false;
        }

        private async Task ExecuteAsync()
        {
            await asyncDelegate();
        }
    }
}
