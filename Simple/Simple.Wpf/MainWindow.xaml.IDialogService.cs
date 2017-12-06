using System;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;

namespace Simple.Wpf
{
    /// <summary>
    /// This implementation uses the wpf toolkit message box.
    /// Which behaves more like a standalone window.
    /// In order to make the interaction asynchronous we have to resort to this https://sriramsakthivel.wordpress.com/2015/04/19/asynchronous-showdialog/
    /// </summary>
    /// <seealso cref="GalaSoft.MvvmLight.Views.IDialogService" />
    /// <seealso cref="System.IDisposable" />
    public partial class MainWindow : IDialogService
    {
        public Task ShowError(Exception error, string title, string buttonText, Action afterHideCallback)
        {
            return ShowError(error.ToString(), title, buttonText, afterHideCallback);
        }

        public Task ShowError(string message, string title, string buttonText, Action afterHideCallback)
        {
            return DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
            {
                DialogBox.OkButtonContent = buttonText;
                DialogBox.ShowMessageBox(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            })).Task;
        }

        public Task ShowMessage(string message, string title)
        {
            return ShowMessage(message, title, "Ok", null);
        }

        public Task ShowMessage(string message, string title, string buttonText, Action afterHideCallback)
        {
            return DispatcherHelper.UIDispatcher.BeginInvoke(new Action(() =>
            {
                DialogBox.OkButtonContent = buttonText;
                DialogBox.ShowMessageBox(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            })).Task;
        }

        public Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText, Action<bool> afterHideCallback)
        {
            // async context
            var context = new MessageBoxAsyncContext
            {
                AsyncCompletion = new TaskCompletionSource<bool>(),
                AfterHideCallback = afterHideCallback
            };

            if (DialogBox.Tag is MessageBoxAsyncContext)
            {
                // cancel unfinished business
                ((MessageBoxAsyncContext)DialogBox.Tag).AsyncCompletion.TrySetCanceled();
                ((MessageBoxAsyncContext)DialogBox.Tag).AfterHideCallback?.Invoke(false);
            }

            DialogBox.Tag = context;

            // UI
            DispatcherHelper.CheckBeginInvokeOnUI(() => {
                DialogBox.YesButtonContent = buttonConfirmText ?? "Yes";
                DialogBox.NoButtonContent = buttonCancelText ?? "No";
                DialogBox.ShowMessageBox(
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

            DialogBox.Closed += handler;

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