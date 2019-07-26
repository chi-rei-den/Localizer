using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
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
        public List<Mod> Mods { get; private set; }

        public RelayCommand RefreshCommand { get; private set; }
        
        public RelayCommand<Mod> ExportCommand { get; private set; }
        
        public ExportViewModel()
        {
            if (IsInDesignMode)
            {
            }
            else
            {
                Mods = new List<Mod>();
            }
            
            RefreshCommand = new RelayCommand(Refresh);
        }
        
        private void Refresh()
        {
            Mods = ModLoader.Mods.ToList();
        }
    }
}