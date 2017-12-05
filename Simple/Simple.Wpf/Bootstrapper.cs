using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Ninject;
using NLog;
using Simple.Wpf.Service;
using Simple.Wpf.ViewModel;

namespace Simple.Wpf
{
    /// <summary>
    /// This is mainly used to manage dependencies.
    /// </summary>
    public class Bootstrapper
    {
        public static IKernel Container;

        static Bootstrapper()
        {
            Container = new StandardKernel();

            var dialogService = new DialogService();
            var mainWindow = new MainWindow(Messenger.Default, dialogService);
            dialogService.Intialize(mainWindow.DialogContainer);

            Container.Bind<MainWindow>().ToConstant(mainWindow);
            Container.Bind<IDialogService>().ToConstant(dialogService);
            Container.Bind<ILogger>().ToMethod((ctx) => LogManager.GetLogger("Main"));
            Container.Bind<IMessenger>().ToConstant(Messenger.Default);
            Container.Bind<IEntityService>().To<EntityService>();
            Container.Bind<EntityMasterDetailViewModel>().ToSelf().InSingletonScope();
            Container.Bind<MainViewModel>().ToSelf().InSingletonScope();
        }
    }
}