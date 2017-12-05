using Autofac;
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
        private readonly static IContainer Container;

        static Bootstrapper()
        {
            var builder = new ContainerBuilder();
            var dialogService = new DialogService();
            var mainWindow = new MainWindow(Messenger.Default, dialogService);
            dialogService.Intialize(mainWindow.DialogContainer);

            builder.RegisterInstance(mainWindow).As<MainWindow>().ExternallyOwned();
            builder.RegisterInstance(dialogService).As<IDialogService>().ExternallyOwned();
            builder.RegisterInstance(Messenger.Default).As<IMessenger>().ExternallyOwned();
            builder.Register(_ => LogManager.GetLogger("Main")).As<ILogger>();
            builder.RegisterType<EntityService>().As<IEntityService>();
            builder.RegisterType<EntityMasterDetailViewModel>().As<EntityMasterDetailViewModel>().ExternallyOwned();
            builder.RegisterType<MainViewModel>().As<MainViewModel>().ExternallyOwned();

            Container = builder.Build();

            // Ninject
            //Container.Bind<MainWindow>().ToConstant(mainWindow);
            //Container.Bind<IDialogService>().ToConstant(dialogService);
            //Container.Bind<ILogger>().ToMethod((ctx) => LogManager.GetLogger("Main"));
            //Container.Bind<IMessenger>().ToConstant(Messenger.Default);
            //Container.Bind<IEntityService>().To<EntityService>();
            //Container.Bind<EntityMasterDetailViewModel>().To<EntityMasterDetailViewModel>().In;
            //Container.Bind<MainViewModel>().To<MainViewModel>().InSingletonScope();
        }

        public static TEntity Resolve<TEntity>()
        {
            return Container.Resolve<TEntity>();
        }
    }
}