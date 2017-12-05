using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Simple.Wpf.ViewModel;

namespace Simple.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDialogService _dialogService;
        private readonly IMessenger _messenger;

        public MainWindow(
            IMessenger messenger, 
            IDialogService dialogService)
        {
            InitializeComponent();
            _messenger = messenger;
            _dialogService = dialogService;
        }

        private void ExitMenuClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            bool confirmedExit = await _dialogService.ShowMessage("Are you sure to exit?", "Confirm Exit", "Yes", "No", null);
            if (confirmedExit)
            {
                ViewModelLocator.Cleanup();
                Application.Current.Shutdown();
            }
        }
    }
}