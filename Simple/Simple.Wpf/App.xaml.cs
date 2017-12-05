using System.Windows;
using GalaSoft.MvvmLight.Threading;
using Ninject;
using NLog;

namespace Simple.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DispatcherHelper.Initialize();
            Bootstrapper.Resolve<MainWindow>().Show();
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var logger = Bootstrapper.Resolve<ILogger>();
            logger.Fatal(e.Exception, "unhandled exception occured");
        }
    }
}
