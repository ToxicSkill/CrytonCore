using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CrytonCore.Interfaces
{
    public interface IDialog
    {
        object DataContext { get; set; }
        bool? DialogResult { get; set; }
        void Close();
        bool? ShowDialog();
    }

    public interface IDialogService
    {
        void Register<TViewModel, TView>() where TViewModel : IDialogRequestClose
            where TView : IDialog;

        bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : IDialogRequestClose;
    }

    public interface IDialogRequestClose
    {
        event EventHandler<DialogCloseRequestEventArgs> CloseRequested;
    }

    public class DialogCloseRequestEventArgs : EventArgs
    {
        public DialogCloseRequestEventArgs(bool? dialogResult)
        {
            DialogResult = dialogResult;
        }

        public bool? DialogResult { get; }
    }

    public class DialogService : IDialogService
    {
        public DialogService()
        {
            Mappings = new Dictionary<Type, Type>();
        }
        public Dictionary<Type, Type> Mappings { get; }

        public void Register<TViewModel, TView>()
            where TViewModel : IDialogRequestClose
            where TView : IDialog
        {
            if (Mappings.ContainsKey(typeof(TViewModel)))
            {
                throw new ArgumentException($"Type {typeof(TViewModel)} is already mapped to type {typeof(TView)}");
            }

            Mappings.Add(typeof(TViewModel), typeof(TView));
        }

        public bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : IDialogRequestClose
        {
            Type viewType = Mappings[typeof(TViewModel)];

            IDialog dialog = (IDialog)Activator.CreateInstance(viewType);

            EventHandler<DialogCloseRequestEventArgs> handler = null;

            handler = (sender, e) =>
            {
                viewModel.CloseRequested -= handler;

                if (e.DialogResult.HasValue)
                {
                    dialog.DialogResult = e.DialogResult;
                }
                else
                {
                    dialog.Close();
                }
            };

            viewModel.CloseRequested += handler;

            dialog.DataContext = viewModel;

            return dialog.ShowDialog();
        }
    }
}
