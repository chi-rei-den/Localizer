using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using MonoMod.Utils;
using Terraria.ModLoader;

namespace Localizer.DataModel
{
    public class LdstrEntry : IEntry
    {
        public List<BaseEntry> Instructions { get; set; }
        
        public IEntry Clone()
        {
            var entry = new LdstrEntry
            {
                Instructions = new List<BaseEntry>()
            };

            foreach (var ins in Instructions)
            {
                entry.Instructions.Add(ins.Clone() as BaseEntry);
            }

            return entry;
        }
    }
    
    public class LdstrFile : File
    {
        public Dictionary<string, LdstrEntry> LdstrEntries { get; set; }

        public override List<string> GetKeys() => LdstrEntries.Keys.ToList();
        public override IEntry GetValue(string key) => LdstrEntries[key];
        
        public override void AddEntry(string key, IEntry entry)
        {
            LdstrEntries.Add(key, entry as LdstrEntry);
        }
    }
}