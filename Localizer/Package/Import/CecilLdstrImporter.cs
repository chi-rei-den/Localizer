using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Harmony;
using Localizer.Attributes;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Ninject;

namespace Localizer.Package.Import
{
    [OperationTiming(OperationTiming.BeforeModCtor)]
    public class CecilLdstrImporter : LdstrImporterBase
    {
        public bool Importing = false;
        public LdstrFile ImportingFile;
        public IMod ImportingMod;

        private HarmonyInstance _harmony;

        public CecilLdstrImporter()
        {
            _harmony = HarmonyInstance.Create(nameof(CecilLdstrImporter));
            _harmony.Patch("Terraria.ModLoader.Core.AssemblyManager".Type().Method("GetModAssembly"),
                           postfix: new HarmonyMethod(typeof(CecilLdstrImporter).Method(nameof(PostGetModAssembly))));
        }

        public static void PostGetModAssembly(ref byte[] __result)
        {
            try
            {
                var instance = Localizer.Kernel.Get<CecilLdstrImporter>();
                if (!instance.Importing)
                {
                    return;
                }

                using (var ms = new MemoryStream(__result))
                {
                    var asmDef = AssemblyDefinition.ReadAssembly(ms);
                    instance.PatchAssembly(asmDef);
                    var result = new MemoryStream();
                    asmDef.Write(result);
                    __result = result.ToArray();
                }
            }
            catch
            {
            }
        }

        private void PatchAssembly(AssemblyDefinition asm)
        {
            var module = asm.MainModule;
            var entryDict = ImportingFile.LdstrEntries;

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

                    var instructions = method.Body?.Instructions;
                    if (instructions == null || !instructions.Any())
                    {
                        return;
                    }

                    var result = instructions.ToList();

                    foreach (var translation in entryPair.Value.Instructions)
                    {
                        ReplaceLdstr(translation.Origin, translation.Translation, result);
                    }

                    Utils.LogDebug($"Patched: {entryPair.Key}");
                });
            }
        }

        protected override void ImportInternal(LdstrFile file, IMod mod, CultureInfo culture)
        {
            Importing = true;
            try
            {
                ImportingFile = file;
                ImportingMod = mod;
                if ((mod as LoadedModWrapper).wrapped.TryGetTarget(out var loadedMod))
                {
                    loadedMod.Invoke("set_NeedsReload", true);
                    loadedMod.Invoke("LoadAssemblies");
                }
            }
            finally
            {
                Importing = false;
            }
        }

        private static void ReplaceLdstr(string o, string n, IEnumerable<Instruction> il)
        {
            if (string.IsNullOrEmpty(n))
            {
                return;
            }

            foreach (var i in il)
            {
                if (i.OpCode == OpCodes.Ldstr && i?.Operand?.ToString() == o)
                {
                    i.Operand = n;
                }
            }
        }

        protected override void DisposeUnmanaged()
        {
            _harmony.UnpatchAll(nameof(CecilLdstrImporter));
        }
    }
}
