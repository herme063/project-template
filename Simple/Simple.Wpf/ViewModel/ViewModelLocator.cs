/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Simple.Wpf"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using Ninject;
using NLog;
using Simple.Wpf.View;
using Simple.Wpf.Service;

namespace Simple.Wpf.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        public static ViewModelLocator Current => (ViewModelLocator)System.Windows.Application.Current.Resources["Locator"];

        public MainViewModel Main => Resolve<MainViewModel>();
        public EntityMasterDetailViewModel EntityMasterDetail => Resolve<EntityMasterDetailViewModel>();

        private readonly IKernel _niContainer;

        public ViewModelLocator()
        {
            var mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
            _niContainer = new StandardKernel();
            _niContainer.Bind<MainWindow>().ToConstant(mainWindow);
            _niContainer.Bind<IDialogService>().ToConstant(mainWindow);
            _niContainer.Bind<ILogger>().ToMethod(_ => LogManager.GetLogger("Main"));
            _niContainer.Bind<IMessenger>().ToConstant(Messenger.Default);
            _niContainer.Bind<IEntityService>().To<EntityService>();
            _niContainer.Bind<EntityMasterDetailViewModel>().ToSelf().InSingletonScope();
            _niContainer.Bind<MainViewModel>().ToSelf().InSingletonScope();
        }

        public TEntity Resolve<TEntity>()
        {
            return _niContainer.Get<TEntity>();
        }

        public void Cleanup()
        {
            Resolve<MainViewModel>().Cleanup();
            Resolve<EntityMasterDetailViewModel>().Cleanup();
        }
    }
}