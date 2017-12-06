using System.Windows;
using GalaSoft.MvvmLight.Threading;
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
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var logger = LogManager.GetLogger("Main");
            logger.Fatal(e.Exception, "unhandled exception occured");
        }
    }
}
