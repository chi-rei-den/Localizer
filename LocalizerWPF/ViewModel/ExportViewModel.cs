using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Localizer;
using Localizer.DataExport;
using Localizer.DataModel;
using Terraria.ModLoader;

namespace LocalizerWPF.ViewModel
{
    public class ExportViewModel : ViewModelBase
    {
        public ObservableCollection<Mod> Mods
        {
            get
            {
                var mods = ModLoader.Mods?.ToList();
                if(mods == null)
                    return new ObservableCollection<Mod>();
                return new ObservableCollection<Mod>(mods);
            }
        }

        public Mod SelectedMod { get; set; }
        
        public string PackageName { get; set; }
        
        public CultureInfo SelectedLanguage { get; set; }
        
        public ObservableCollection<CultureInfo> Languages { get; set; }
        
        public bool MakeBackup { get; set; }
        
        public bool ForceOverride { get; set; }
        
        public bool WithTranslation { get; set; }
        
        public RelayCommand ExportCommand { get; private set; }
        
        public ExportViewModel()
        {
            PackageName = "PleaseEnterPackageName";

            Languages = new ObservableCollection<CultureInfo>(
                CultureInfo.GetCultures(CultureTypes.AllCultures));
            
            SelectedLanguage = CultureInfo.CurrentCulture;

            MakeBackup = true;
            ForceOverride = false;
            WithTranslation = true;
            
            ExportCommand = new RelayCommand(Export);
        }

        private void Export()
        {
            try
            {
                if (SelectedMod == null)
                {
                    MessageBox.Show("Please Select Mod!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(PackageName))
                {
                    MessageBox.Show("Please Enter Package Name!");
                    return;
                }

                PackageManager.Export(SelectedMod, PackageName, SelectedLanguage, new Dictionary<Type, ExportConfig>
                {
                    [typeof(BasicItemFile)] = new BasicExportConfig
                    {
                        ForceOverride = ForceOverride,
                        MakeBackup = MakeBackup,
                        WithTranslation = WithTranslation
                    },
                    [typeof(BasicNPCFile)] = new BasicExportConfig
                    {
                        ForceOverride = ForceOverride,
                        MakeBackup = MakeBackup,
                        WithTranslation = WithTranslation
                    },
                    [typeof(BasicBuffFile)] = new BasicExportConfig
                    {
                        ForceOverride = ForceOverride,
                        MakeBackup = MakeBackup,
                        WithTranslation = WithTranslation
                    },
                    [typeof(BasicCustomFile)] = new BasicExportConfig
                    {
                        ForceOverride = ForceOverride,
                        MakeBackup = MakeBackup,
                        WithTranslation = WithTranslation
                    },
                    [typeof(LdstrFile)] = new LdstrExportConfig
                    {
                        ForceOverride = ForceOverride,
                        MakeBackup = MakeBackup,
                    },
                });

                MessageBox.Show($"{PackageName} Exported!");
            }
            catch (Exception e)
            {
                Localizer.Localizer.Log.Error(e);
            }
        }
    }
}