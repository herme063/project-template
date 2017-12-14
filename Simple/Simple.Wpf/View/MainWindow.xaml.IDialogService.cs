using MahApps.Metro.Controls.Dialogs;
using Simple.Wpf.Resource;
using Simple.Wpf.Service;
using System;
using System.Threading.Tasks;

namespace Simple.Wpf.View
{
    /// <summary>
    /// This implementation uses the mahapps.metro style message box.
    /// </summary>
    /// <seealso cref="Simple.Wpf.Service.IDialogServiceEx" />
    public partial class MainWindow : IDialogServiceEx
    {
        public readonly IDialogCoordinator _dialogCoordinator;
        public ProgressDialogController _progressDialogController;

        public async Task HideBusy()
        {
            if (_progressDialogController != null)
            {
                await _progressDialogController.CloseAsync();
                _progressDialogController = null;
            }
        }

        public async Task ShowBusy()
        {
            await ShowBusy(Strings.Message_Loading);
        }

        public async Task ShowBusy(string message)
        {
            _progressDialogController = await _dialogCoordinator.ShowProgressAsync(this, message, string.Empty);
            _progressDialogController.SetIndeterminate();
        }

        public async Task ShowError(Exception error, string title, string buttonText, Action afterHideCallback)
        {
            await ShowError(error.ToString(), title, buttonText, afterHideCallback);
        }

        public async Task ShowError(string message, string title, string buttonText, Action afterHideCallback)
        {
            await _dialogCoordinator.ShowMessageAsync(this, title, message, MessageDialogStyle.Affirmative, new MetroDialogSettings
            {
                AffirmativeButtonText = buttonText,
                ColorScheme = MetroDialogColorScheme.Accented
            });
        }

        public async Task ShowMessage(string message, string title)
        {
            await ShowMessage(message, title, Strings.Button_Ok, null);
        }

        public async Task ShowMessage(string message, string title, string buttonText, Action afterHideCallback)
        {
            await _dialogCoordinator.ShowMessageAsync(this, title, message, MessageDialogStyle.Affirmative, new MetroDialogSettings
            {
                AffirmativeButtonText = buttonText ?? Strings.Button_Ok,
                ColorScheme = MetroDialogColorScheme.Theme
            });
        }

        public async Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText, Action<bool> afterHideCallback)
        {
            MessageDialogResult result = await _dialogCoordinator.ShowMessageAsync(this, title, message, MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings
            {
                AffirmativeButtonText = buttonConfirmText ?? Strings.Button_Yes,
                NegativeButtonText = buttonCancelText ?? Strings.Button_No,
                ColorScheme = MetroDialogColorScheme.Theme
            });

            bool affirmative = result == MessageDialogResult.Affirmative;
            afterHideCallback?.Invoke(affirmative);

            return affirmative;
        }

        public Task ShowMessageBox(string message, string title)
        {
            return ShowMessage(message, title);
        }
    }
}