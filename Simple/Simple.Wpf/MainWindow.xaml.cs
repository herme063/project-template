using System.Windows;
using GalaSoft.MvvmLight.Views;
using Simple.Wpf.ViewModel;

namespace Simple.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ExitMenuClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            IDialogService dialogService = ViewModelLocator.Current.Resolve<IDialogService>();
            bool confirmedExit = await dialogService.ShowMessage("Are you sure to exit?", "Confirm Exit", "Yes", "No", null);
            if (confirmedExit)
            {
                ViewModelLocator.Current.Cleanup();
                Application.Current.Shutdown();
            }
        }
    }
}