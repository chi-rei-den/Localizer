/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace LocalizerWPF.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SettingViewModel>();
            SimpleIoc.Default.Register<ManagerViewModel>();
            SimpleIoc.Default.Register<MakeViewModel>();
            SimpleIoc.Default.Register<BrowserViewModel>();
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        
        public SettingViewModel Setting => ServiceLocator.Current.GetInstance<SettingViewModel>();
        
        public ManagerViewModel Manager => ServiceLocator.Current.GetInstance<ManagerViewModel>();
        
        public MakeViewModel Make => ServiceLocator.Current.GetInstance<MakeViewModel>();
        
        public BrowserViewModel Browser => ServiceLocator.Current.GetInstance<BrowserViewModel>();
        
        
        public static void Cleanup()
        {            
            SimpleIoc.Default.Unregister<MainViewModel>();
            SimpleIoc.Default.Unregister<SettingViewModel>();
            SimpleIoc.Default.Unregister<ManagerViewModel>();
            SimpleIoc.Default.Unregister<MakeViewModel>();
            SimpleIoc.Default.Unregister<BrowserViewModel>();
        }
    }
}
