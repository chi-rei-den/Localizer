using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Terraria.ModLoader;

namespace Localizer.DataModel
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Package
    {
        [JsonProperty] public string Name { get; set; }
        [JsonProperty] public string Author { get; set; } = "";
        [JsonProperty] public string ModName { get; set; }
        [JsonProperty] public Version Version { get; set; } = new Version(1, 0, 0, 0);
        [JsonProperty] public CultureInfo Language { get; set; } = CultureInfo.GetCultureInfo("en-US");
        [JsonProperty] public List<string> FileList { get; set; } = new List<string>();

        public List<File> Files { get; set; } = new List<File>();

        public bool Enabled { get; set; } = true;
        public int Priority { get; set; } = 100;
        public Mod Mod { get; set; }

        public Package() { }

        public Package(string name, Mod mod, CultureInfo language)
        {
            this.Name = name;
            this.Mod = mod;
            this.ModName = mod.Name;
            this.Language = language;

            this.FileList = new List<string>();
            this.Files = new List<File>();
        }

        public void AddFile(File file)
        {
            if (file == null)
            {
                return;
            }

            this.Files.Add(file);
            file.Owner = this;
        }

        public void Init()
        {
            this.Mod = ModLoader.Mods.FirstOrDefault(m => m.Name == this.ModName);
            this.Files = new List<File>();
        }
    }
}
