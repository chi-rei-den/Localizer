using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Harmony;
using Localizer.Attributes;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Localizer.Package.Export;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Utils;
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
            _harmony.Patch("Terraria.ModLoader.Core.AssemblyManager", "GetModAssembly",
                           postfix: new HarmonyMethod(typeof(CecilLdstrImporter).Method(nameof(PostGetModAssembly))));
        }

        public static void PostGetModAssembly(ref byte[] __result)
        {
            try
            {
                var instance = Localizer.Kernel.Get<CecilLdstrImporter>();
                if (!instance.Importing || !Localizer.Config.ImportLdstr)
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

        private static void ReplaceLdstr(string o, string n, List<Instruction> il)
        {
            if (string.IsNullOrEmpty(n))
            {
                return;
            }


            for (var i = 0; i < il.Count(); i++)
            {
                var ins = il[i];
                if (ins.OpCode == OpCodes.Ldstr && !string.IsNullOrWhiteSpace(ins.Operand.ToString()))
                {
                    // Filter methods in blacklist1
                    if (i < il.Count() - 1)
                    {
                        var next = il[i + 1];
                        var operandId = "";
                        if (next.OpCode == OpCodes.Call || next.OpCode == OpCodes.Callvirt)
                        {
                            operandId = (next.Operand as MethodReference).GetID();
                        }
                        else if (next.OpCode == OpCodes.Calli)
                        {
                            operandId = (next.Operand as CallSite).GetID();
                        }
                        if (!string.IsNullOrWhiteSpace(operandId) && LdstrFileExport._blackList1.Any(m => operandId == m?.GetID()))
                        {
                            continue;
                        }
                    }

                    // Filter methods in blacklist2
                    if (i < il.Count() - 2)
                    {
                        var afterNext = il[i + 2];
                        var operandId = "";
                        if (afterNext.OpCode == OpCodes.Call || afterNext.OpCode == OpCodes.Callvirt)
                        {
                            operandId = (afterNext.Operand as MethodReference).GetID();
                        }
                        else if (afterNext.OpCode == OpCodes.Calli)
                        {
                            operandId = (afterNext.Operand as CallSite).GetID();
                        }
                        if (!string.IsNullOrWhiteSpace(operandId) && LdstrFileExport._blackList2.Any(m => operandId == m?.GetID()))
                        {
                            continue;
                        }
                    }

                    if (ins.OpCode == OpCodes.Ldstr && ins?.Operand?.ToString() == o)
                    {
                        ins.Operand = n;
                    }
                }
            }
        }

        protected override void DisposeUnmanaged()
        {
            _harmony.UnpatchAll(nameof(CecilLdstrImporter));
        }
    }
}
