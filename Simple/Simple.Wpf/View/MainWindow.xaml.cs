﻿using System.Windows;
using GalaSoft.MvvmLight.Views;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Simple.Wpf.ViewModel;

namespace Simple.Wpf.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            _dialogCoordinator = DialogCoordinator.Instance;
        }

        private void ExitMenuClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            IDialogService dialogService = ViewModelLocator.Current.Resolve<IDialogService>();
            bool confirmedExit = await dialogService.ShowMessage(
                Resource.Strings.Message_ConfirmExit,
                Resource.Strings.Title_ConfirmExit,
                Resource.Strings.Button_Yes, 
                Resource.Strings.Button_No, 
                null);
            if (confirmedExit)
            {
                ViewModelLocator.Current.Cleanup();
                Application.Current.Shutdown();
            }
        }
    }
}