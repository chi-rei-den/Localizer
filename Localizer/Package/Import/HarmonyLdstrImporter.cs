using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Harmony;
using Localizer.DataModel;
using Localizer.DataModel.Default;

namespace Localizer.Package.Import
{
    public sealed class HarmonyLdstrImporter : LdstrImporterBase
    {
        private HarmonyInstance harmony;

        private static Dictionary<MethodBase, LdstrEntry> entries;

        public HarmonyLdstrImporter()
        {
            harmony = HarmonyInstance.Create("LdstrFileImport");
        }

        protected override void ImportInternal(LdstrFile file, IMod mod, CultureInfo culture)
        {
            entries = new Dictionary<MethodBase, LdstrEntry>();

            var module = mod.Code.ManifestModule;
            var entryDict = file.LdstrEntries;

            foreach (var entryPair in entryDict)
            {
                Utils.SafeWrap(() =>
                {
                    if (!HaveTranslation(entryPair.Value))
                    {
                        return;
                    }

                    Utils.LogDebug($"Finding method: [{entryPair.Key}]");
                    var method = Utils.FindMethodByID(module, entryPair.Key);
                    if (method == null)
                    {
                        Utils.LogDebug($"Cannot find.");
                        return;
                    }

                    entries.Add(method, entryPair.Value);

                    harmony.Patch(method, transpiler: new HarmonyMethod(NoroHelper.MethodInfo(() => Transpile(null, null))));

                    Utils.LogDebug($"Patched: {entryPair.Key}");
                });
            }
        }

        public override void Reset()
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

        protected override void DisposeUnmanaged()
        {
            entries = null;
            harmony.UnpatchAll("LdstrFileImport");
        }
    }
}
