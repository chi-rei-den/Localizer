using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Harmony;
using Localizer.Attributes;
using Localizer.DataModel;
using Localizer.DataModel.Default;

namespace Localizer.Services.File
{
    [OperationTiming(OperationTiming.BeforeContentLoad | OperationTiming.PostContentLoad)]
    public sealed class HarmonyLdstrFileImport : LdstrFileImportBase, IFileImportService<LdstrFile>
    {
        private HarmonyInstance harmony;

        private static Dictionary<MethodBase, LdstrEntry> entries;

        public HarmonyLdstrFileImport()
        {
            harmony = HarmonyInstance.Create("LdstrFileImport");
        }

        public void Import(LdstrFile file, IMod mod, CultureInfo culture)
        {
            entries = new Dictionary<MethodBase, LdstrEntry>();

            var module = mod.Code.ManifestModule;
            var entryDict = file.LdstrEntries;

            foreach (var entryPair in entryDict)
            {
                if (!HaveTranslation(entryPair.Value))
                {
                    continue;
                }

                var method = module.FindMethod(entryPair.Key);
                if (method == null)
                {
                    continue;
                }

                entries.Add(method, entryPair.Value);

                var transpiler = typeof(HarmonyLdstrFileImport).GetMethod("Transpile", BindingFlags.NonPublic | BindingFlags.Static);

                harmony.Patch(method, null, null, new HarmonyMethod(transpiler));
            }
        }

        public void Reset()
        {
            harmony.UnpatchAll("LdstrFileImport");
        }
        
        private static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instructions, MethodBase original)
        {
            if (!entries.ContainsKey(original) || instructions == null || instructions.Count() == 0)
            {
                return instructions;
            }

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
            if (string.IsNullOrEmpty(n))
            {
                return;
            }

            var ins = il.FirstOrDefault(i => i?.operand?.ToString() == o);
            if (ins != null)
            {
                ins.operand = n;
            }
        }

        public void Dispose()
        {
            entries = null;
            harmony.UnpatchAll("LdstrFileImport");
        }
    }
}
