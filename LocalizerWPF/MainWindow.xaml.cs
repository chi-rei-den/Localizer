using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LocalizerWPF
{
    public class ModTranslation
    {
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Mod { get; set; }
        public Version Version { get; set; } = new Version(1, 0, 0, 0);
        public CultureInfo Language { get; set; } = CultureInfo.GetCultureInfo("en-US");
        public List<Translation> Translations { get; set; } = new List<Translation>();
        public ModTranslation(string name) { this.Name = name; }
    }

    public class Translation
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Namespace { get; set; } = "*";
        public string Method { get; set; } = "*";
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static int Selected { get; set; }
        public static List<ModTranslation> ModTranslations1 { get; set; } = new List<ModTranslation>()
            {
                new ModTranslation("T1")
                {
                    Enabled = false,
                    Author = "test",
                    Mod = "Bluemagic",
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
                new ModTranslation("T2")
                {
                    Enabled = false,
                    Author = "凌云枫落",
                    Mod = "DBZMOD",
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
                new ModTranslation("T3")
                {
                    Enabled = true,
                    Author = "凌云枫落",
                    Mod = "ARPGLoot",
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
                new ModTranslation("T4")
                {
                    Enabled = true,
                    Author = "As a song 当歌",
                    Mod = "CalamityMod",
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
                new ModTranslation("Test 5")
                {
                    Enabled = false,
                    Author = "佚名",
                    Mod = "CalamityMod",
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
                new ModTranslation("Test 5")
                {
                    Enabled = false,
                    Author = "佚名二号",
                    Mod = "CalamityMod",
                    Language = CultureInfo.GetCultureInfo("zh-CN"),
                    Translations = new List<Translation>()
                }
            };
        public static List<KeyValuePair<string, List<ModTranslation>>> ModTranslations { get; set; } = ModTranslations1.GroupBy(i => i.Mod).ToDictionary(i => i.Key, i => i.ToList()).ToList();
        public static List<ModTranslation> SelectedModTranslations => ModTranslations[Selected].Value;
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void Setting_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void About_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
