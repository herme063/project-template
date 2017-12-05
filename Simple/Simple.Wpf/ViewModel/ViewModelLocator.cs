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

using Ninject;

namespace Simple.Wpf.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        public MainViewModel Main => Bootstrapper.Container.Get<MainViewModel>();
        public EntityMasterDetailViewModel EntityMasterDetail => Bootstrapper.Container.Get<EntityMasterDetailViewModel>();

        public static void Cleanup()
        {
            Bootstrapper.Container.Get<MainViewModel>().Cleanup();
            Bootstrapper.Container.Get<EntityMasterDetailViewModel>().Cleanup();
        }
    }
}