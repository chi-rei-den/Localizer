using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using MonoMod.Utils;

namespace Localizer.Services.File
{
    public class HarmonyLdstrFileImportService : IFileImportService
    {
        private HarmonyInstance harmony;

        private static Dictionary<MethodBase, LdstrEntry> entries;
        
        public HarmonyLdstrFileImportService()
        {
            harmony = HarmonyInstance.Create("LdstrFileImport");
        }

        public void Import(IFile file, IMod mod)
        {
            if (file.GetType() != typeof(LdstrFile))
            {
                return;
            }

            entries = new Dictionary<MethodBase, LdstrEntry>();
            
            var module = mod.Code.ManifestModule;
            var entryDict = (file as LdstrFile).LdstrEntries;

            foreach (var entryPair in entryDict)
            {
                if(!HaveTranslation(entryPair.Value))
                {
                    continue;
                }
                
                var method = module.FindMethod(entryPair.Key);
                if (method == null)
                {
                    continue;
                }

                entries.Add(method, entryPair.Value);
                
                var transpiler = typeof(HarmonyLdstrFileImportService).GetMethod("Transpile", BindingFlags.NonPublic | BindingFlags.Static);

                harmony.Patch(method, null, null, new HarmonyMethod(transpiler));
            }
        }

        public IFile Merge(IFile main, IFile addition)
        {
            if (main == null || main.GetType() != addition.GetType() || main.GetType() != typeof(LdstrFile))
            {
                return null;
            }

            var mainFile = main as LdstrFile;
            var additionFile = addition as LdstrFile;

            var result = new LdstrFile();
            result.LdstrEntries = new Dictionary<string, LdstrEntry>();
            foreach (var pair in additionFile.LdstrEntries)
            {
                if (mainFile.LdstrEntries.ContainsKey(pair.Key))
                {
                    result.LdstrEntries.Add(pair.Key, Merge(mainFile.LdstrEntries[pair.Key], pair.Value));
                }
                else
                {
                    result.LdstrEntries.Add(pair.Key, pair.Value);
                }
            }

            return result;
        }

        public void Reset()
        {
            harmony.UnpatchAll("LdstrFileImport");
        }
        public LdstrEntry Merge(LdstrEntry main, LdstrEntry addition)
        {
            var result = new LdstrEntry {Instructions = new List<BaseEntry>()};

            var mainE = main;
            var addE = addition;

            foreach (var ins in mainE.Instructions)
            {
                if (!result.Instructions.Exists(i => i.Origin == ins.Origin))
                {
                    result.Instructions.Add(ins.Clone() as BaseEntry);
                }
            }

            foreach (var ins in addE.Instructions)
            {
                if (!result.Instructions.Exists(i => i.Origin == ins.Origin))
                {
                    result.Instructions.Add(ins.Clone() as BaseEntry);
                }
            }

            return result;
        }

        private bool HaveTranslation(LdstrEntry entry)
        {
            foreach (var ins in entry.Instructions)
            {
                if (!string.IsNullOrEmpty(ins.Translation))
                {
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            if(!entries.ContainsKey(original) || instructions == null || instructions.Count() == 0)
                return instructions;

            var result = instructions.ToList();
            
            var entry = entries[original];
            
            foreach (var translation in entry.Instructions)
            {
                ReplaceLdstr(translation.Origin, translation.Translation, result);
            }

            return result;
        }
        
        private static void ReplaceLdstr(string o, string n, IEnumerable<CodeInstruction> il)
        {
            if(string.IsNullOrEmpty(n))
                return;
            
            var ins = il.FirstOrDefault(i => i?.operand?.ToString() == o);
            if (ins != null)
            {
                ins.operand = n;
            }
        }

        public void Dispose()
        {
            harmony.UnpatchAll("LdstrFileImport");
        }
    }
}
