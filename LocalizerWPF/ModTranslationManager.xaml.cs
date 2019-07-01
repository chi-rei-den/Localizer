using System.Collections.Generic;
using System.Windows.Controls;

namespace LocalizerWPF
{
    /// <summary>
    /// Interaction logic for ModTranslationManager.xaml
    /// </summary>
    public partial class ModTranslationManager : UserControl
    {
        private List<KeyValuePair<string, List<ModTranslation>>> _modTranslations;
        public List<KeyValuePair<string, List<ModTranslation>>> ModTranslations
        {
            get => this._modTranslations;
            set => this.DataGrid.ItemsSource = this._modTranslations = value;
        }
        public ModTranslationManager()
        {
            this.InitializeComponent();
        }
    }
}
