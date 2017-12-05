using System;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using Xceed.Wpf.Toolkit.Primitives;

namespace Simple.Wpf
{
    /// <summary>
    /// This implementation uses the wpf toolkit message box.
    /// Which behaves more like a standalone window.
    /// In order to make the interaction asynchronous we have to resort to this https://sriramsakthivel.wordpress.com/2015/04/19/asynchronous-showdialog/
    /// </summary>
    /// <seealso cref="GalaSoft.MvvmLight.Views.IDialogService" />
    /// <seealso cref="System.IDisposable" />
    public partial class DialogService : 
        IDialogService,
        IDisposable
    {
        private WindowContainer _container;
        private Xceed.Wpf.Toolkit.MessageBox _messageBox;

        public void Dispose()
        {
            if (_container != null)
            {
                _container.Children.Remove(_messageBox);
                _messageBox = null;
                _container = null;
            }
        }

        public void Intialize(WindowContainer container)
        {
            if (_messageBox != null)
            {
                throw new InvalidOperationException("This has to be called exactly once");
            }

            _container = container;
            _messageBox = new Xceed.Wpf.Toolkit.MessageBox();
            _container.Children.Add(_messageBox);
        }

        public Task ShowError(Exception error, string title, string buttonText, Action afterHideCallback)
        {
            return ShowError(error.ToString(), title, buttonText, afterHideCallback);
        }

        public Task ShowError(string message, string title, string buttonText, Action afterHideCallback)
        {
            throw new NotImplementedException();
        }

        public Task ShowMessage(string message, string title)
        {
            return ShowMessage(message, title, "Ok", null);
        }

        public Task ShowMessage(string message, string title, string buttonText, Action afterHideCallback)
        {
            if (_messageBox == null)
            {
                throw new InvalidOperationException("Must be initialized first");
            }

            return DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
            {
                _messageBox.OkButtonContent = buttonText;
                _messageBox.ShowMessageBox(message, title, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            })).Task;
        }

        public Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText, Action<bool> afterHideCallback)
        {
            if (_messageBox == null)
            {
                throw new InvalidOperationException("Must be initialized first");
            }

            // async context
            var context = new MessageBoxAsyncContext
            {
                AsyncCompletion = new TaskCompletionSource<bool>(),
                AfterHideCallback = afterHideCallback
            };

            if (_messageBox.Tag is MessageBoxAsyncContext)
            {
                // cancel unfinished business
                ((MessageBoxAsyncContext)_messageBox.Tag).AsyncCompletion.TrySetCanceled();
                ((MessageBoxAsyncContext)_messageBox.Tag).AfterHideCallback?.Invoke(false);
            }

            _messageBox.Tag = context;

            // UI
            DispatcherHelper.CheckBeginInvokeOnUI(() => {
                _messageBox.YesButtonContent = buttonConfirmText ?? "Yes";
                _messageBox.NoButtonContent = buttonCancelText ?? "No";
                _messageBox.ShowMessageBox(
                    message, 
                    title, 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question, 
                    MessageBoxResult.Yes);
            });

            EventHandler handler = null;
            handler = (s, e) =>
            {
                var msgBox = (s as Xceed.Wpf.Toolkit.MessageBox);
                msgBox.Closed -= handler;

                bool confirmed = msgBox.MessageBoxResult == MessageBoxResult.Yes;
                var ctx = msgBox.Tag as MessageBoxAsyncContext;
                ctx?.AfterHideCallback?.Invoke(confirmed);
                ctx?.AsyncCompletion.TrySetResult(confirmed);
            };

            _messageBox.Closed += handler;

            return context.AsyncCompletion.Task;
        }

        public Task ShowMessageBox(string message, string title)
        {
            return ShowMessage(message, title);
        }

        private class MessageBoxAsyncContext
        {
            public Action<bool> AfterHideCallback { get; set; }
            public TaskCompletionSource<bool> AsyncCompletion { get; set; }
        }
    }
}