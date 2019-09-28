using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using Localizer;
using Localizer.DataModel;
using Newtonsoft.Json;

namespace LocalizerWPF.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Package : IPackage
    {
        public Package()
        { }

        public Package(string name, IMod mod, CultureInfo language)
        {
            Name = name;
            Mod = mod;
            ModName = mod.Name;
            Description = string.Empty;
            Language = language;

            FileList = new ObservableCollection<string>();
            Files = new ObservableCollection<IFile>();
        }

        public ObservableCollection<IFile> ObservableFiles
        {
            get => Files as ObservableCollection<IFile>;
            set => Files = value;
        }

        public bool Exported { get; set; }
        [JsonProperty] public string Name { get; set; } = "";
        [JsonProperty] public string Author { get; set; } = "";
        [JsonProperty] public string ModName { get; set; } = "";
        [JsonProperty] public string Description { get; set; } = "";
        [JsonProperty] public Version Version { get; set; } = new Version(1, 0, 0, 0);
        [JsonProperty] public CultureInfo Language { get; set; } = CultureInfo.GetCultureInfo("en-US");
        [JsonProperty] public ICollection<string> FileList { get; set; } = new ObservableCollection<string>();

        public ICollection<IFile> Files { get; set; } = new ObservableCollection<IFile>();

        public int Count => Files.Sum(f => f.GetKeys().Count());

        public bool Enabled { get; set; } = true;

        public IMod Mod { get; set; }

        public void AddFile(IFile file)
        {
            if (file == null)
            {
                return;
            }

            if (!FileList.Contains(file.GetType().Name))
            {
                FileList.Add(file.GetType().Name);
            }

            Files.Add(file);
        }

        public void RemoveFile(IFile file)
        {
            throw new NotImplementedException();
        }
    }
}
