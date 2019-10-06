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
            SimpleIoc.Default.Register<AboutViewModel>();
            SimpleIoc.Default.Register<ManagerViewModel>();
            SimpleIoc.Default.Register<MakeViewModel>();
            SimpleIoc.Default.Register<BrowserViewModel>();
        }

        public static MainViewModel Main
        {
            get
            {
                if (_main == null)
                {
                    _main = ServiceLocator.Current.GetInstance<MainViewModel>();
                }

                return _main;
            }
            set => _main = value;
        }

        public static SettingViewModel Setting
        {
            get
            {
                if (_setting == null)
                {
                    _setting = ServiceLocator.Current.GetInstance<SettingViewModel>();
                }

                return _setting;
            }
            set => _setting = value;
        }

        public static AboutViewModel About
        {
            get
            {
                if (_about == null)
                {
                    _about = ServiceLocator.Current.GetInstance<AboutViewModel>();
                }

                return _about;
            }
            set => _about = value;
        }
        
        public static ManagerViewModel Manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = ServiceLocator.Current.GetInstance<ManagerViewModel>();
                }

                return _manager;
            }
            set => _manager = value;
        }

        public static MakeViewModel Make
        {
            get
            {
                if (_make == null)
                {
                    _make = ServiceLocator.Current.GetInstance<MakeViewModel>();
                }

                return _make;
            }
            set => _make = value;
        }

        public static BrowserViewModel Browser
        {
            get
            {
                if (_browser == null)
                {
                    _browser = ServiceLocator.Current.GetInstance<BrowserViewModel>();
                }

                return _browser;
            }
            set => _browser = value;
        }

        private static MainViewModel _main;

        private static SettingViewModel _setting;
        
        private static AboutViewModel _about;

        private static ManagerViewModel _manager;

        private static MakeViewModel _make;

        private static BrowserViewModel _browser;

        public static void Cleanup()
        {
            _main?.Cleanup();
            _setting?.Cleanup();
            _manager?.Cleanup();
            _make?.Cleanup();
            _browser?.Cleanup();
        }
    }
}
