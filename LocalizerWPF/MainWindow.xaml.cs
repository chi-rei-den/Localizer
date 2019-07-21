using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Principal;

namespace LocalizerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static List<ModTranslationPackage> ModTranslations1 { get; set; } = new List<ModTranslationPackage>()
            {
                new ModTranslationPackage("T1", "Bluemagic")
                {
                    Enabled = false,
                    Author = "test",
                    Language = CultureInfo.GetCultureInfo("zh-CN"),
                    Translations = new List<Translation>
                    {
                        new Translation
                        {
                            From = "Test from 1",
                            To = "Test to 1"
                        }
                    }
                },
                new ModTranslationPackage("T2", "DBZMOD")
                {
                    Enabled = false,
                    Author = "凌云枫落",
                    Language = CultureInfo.GetCultureInfo("zh-CN"),
                    Translations = new List<Translation>
                    {
                        new Translation
                        {
                            From = "Test from 2",
                            To = "Test to 2"
                        },
                        new Translation
                        {
                            From = "Test from 2.1",
                            To = "Test to 2.1"
                        }
                    }
                },
                new ModTranslationPackage("T3", "ARPGLoot")
                {
                    Enabled = true,
                    Author = "凌云枫落",
                    Language = CultureInfo.GetCultureInfo("zh-CN"),
                    Translations = new List<Translation>
                    {
                        new Translation
                        {
                            From = "Test from 3",
                            To = "Test to 3"
                        },
                        new Translation
                        {
                            From = "Test from 3.2",
                            To = "Test to 3.2"
                        },
                        new Translation
                        {
                            From = "Test from 3.3",
                            To = "Test to 3.3"
                        }
                    }
                },
                new ModTranslationPackage("T4", "CalamityMod")
                {
                    Enabled = true,
                    Author = "As a song 当歌",
                    Language = CultureInfo.GetCultureInfo("zh-CN"),
                    Translations = new List<Translation>
                    {
                        new Translation
                        {
                            From = "Test from 4",
                            To = "Test to 4"
                        },
                        new Translation
                        {
                            From = "Test from 4.4",
                            To = "Test to 4.4"
                        },
                        new Translation
                        {
                            From = "Test from 4.5",
                            To = "Test to 4.5"
                        },
                        new Translation
                        {
                            From = "Test from 4.6",
                            To = "Test to 4.6"
                        }
                    }
                },
                new ModTranslationPackage("Test 5", "CalamityMod")
                {
                    Enabled = false,
                    Author = "佚名",
                    Language = CultureInfo.GetCultureInfo("zh-CN"),
                    Translations = new List<Translation>
                    {
                        new Translation
                        {
                            From = "Test from 4",
                            To = "Test to 4"
                        }
                    }
                },
                new ModTranslationPackage("Test 6", "CalamityMod")
                {
                    Enabled = false,
                    Author = "佚名二号",
                    Language = CultureInfo.GetCultureInfo("zh-CN"),
                    Translations = new List<Translation>()
                }
            };
        public MainWindow()
        {
            this.InitializeComponent();
            this.LocalMTM.ModTranslations = ModTranslations1.GroupBy(i => i.Mod).Select(i => new ModTranslation(i.Key) { ModTranslations = new ObservableCollection<ModTranslationPackage>(i) }).ToList();
            this.OnlineMTM.ModTranslations = ModTranslations1.GroupBy(i => i.Mod).Select(i => new ModTranslation(i.Key) { ModTranslations = new ObservableCollection<ModTranslationPackage>(i) }).ToList();
        }

        private void Setting_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void About_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
