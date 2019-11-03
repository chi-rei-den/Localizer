using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Localizer;
using Localizer.DataModel;
using Localizer.ServiceInterfaces.Network;
using Ninject;

namespace LocalizerWPF.ViewModel
{
    public class BrowserViewModel : ViewModelBase
    {
        public ObservableCollection<IPackage> Packages
        {
            get => _packages;
            set
            {
                _packages = value;
                RaisePropertyChanged("Packages");
            }
        }

        public IPackage SelectedPackage
        {
            get => _selectedPackage;
            set
            {
                _selectedPackage = value;
                RaisePropertyChanged("SelectedPackage");
            }
        }

        public RelayCommand RefreshCommand { get; set; }
        public RelayCommand<IPackage> DownloadCommand { get; set; }

        private IPackageBrowserService packageBrowserService;
        private IDownloadManagerService downloadManager;
        private IPackage _selectedPackage;
        private ObservableCollection<IPackage> _packages;

        private List<IPackage> downloading;

        public BrowserViewModel()
        {
            packageBrowserService = Plugin.Kernel.Get<IPackageBrowserService>();
            downloadManager = Plugin.Kernel.Get<IDownloadManagerService>();

            RefreshCommand = new RelayCommand(Refresh);
            DownloadCommand = new RelayCommand<IPackage>(Download, CanDownload);

            downloading = new List<IPackage>();
        }

        private void Refresh()
        {
            Utils.LogDebug($"Fetching package list");
            Packages = GetList();
            Utils.LogDebug($"Package list fetched");
        }

        private ObservableCollection<IPackage> GetList()
        {
            try
            {
                return new ObservableCollection<IPackage>(packageBrowserService.GetList());
            }
            catch (Exception e)
            {
                Utils.LogError(e);
            }

            return new ObservableCollection<IPackage>();
        }

        private void Download(IPackage pack)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                try
                {
                    Utils.LogDebug($"Requesting {pack.Name} download");
                    var url = packageBrowserService.GetDownloadLinkOf(pack);

                    var path = Utils.EscapePath(Path.Combine(Localizer.Localizer.DownloadPackageDirPath,
                                                             pack.Name + pack.Author + ".locpack"));

                    downloading.Add(pack);
                    downloadManager.Download(url, path);
                    downloading.Remove(pack);
                    Utils.LogDebug($"{pack.Name} is downloaded");
                }
                catch (Exception e)
                {
                    Utils.LogError(e);
                    MessageBox.Show(e.ToString());
                }
            }));
        }

        private bool CanDownload(IPackage pack)
        {
            return !downloading.Contains(pack);
        }
    }
}
