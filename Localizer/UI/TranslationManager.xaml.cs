using System.Collections.Generic;

namespace Localizer
{
    public class TestItem
    {
        public bool Enabled { get; set; }
        public string Name { get; set; }
        // Binding="{Binding RelativeSource={RelativeSource AncestorType=DataGridRow}
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TranslationManager
    {
        public TranslationManager()
        {
            this.InitializeComponent();
            // TODO: Fix tMod build since it didn't include the xaml file
            // Error Compiling Localizer.FNA.dll failed with 2 errors
            // Localizer\UI\TranslationManager.xaml.cs(17, 17) : error CS1061:
            // 'TranslationManager' does not contain a definition for 'InitializeComponent' and no accessible extension method 'InitializeComponent' accepting a first argument of type 'TranslationManager' could be found(are you missing a using directive or an assembly reference ?)
            this.DataContext = new List<TestItem>()
            {
                new TestItem { Enabled = false, Name = "name1" },
                new TestItem { Enabled = false, Name = "nameq" },
                new TestItem { Enabled = true, Name = "namea" }
            };
        }
    }
}
