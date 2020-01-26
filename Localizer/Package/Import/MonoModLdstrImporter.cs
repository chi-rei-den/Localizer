using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using Terraria.ModLoader;

namespace Localizer.Package.Import
{
    public class MonoModLdstrImporter : LdstrImporterBase
    {
        private Dictionary<MethodBase, ILContext.Manipulator> modifications;

        public MonoModLdstrImporter()
        {
            modifications = new Dictionary<MethodBase, ILContext.Manipulator>();
        }

        protected override void ImportInternal(LdstrFile file, IMod mod, CultureInfo culture)
        {
            Terraria.ModLoader.ContentInstance.Register(Utils.GetModByName(mod.Name));
            var module = mod.Code.ManifestModule;

            var entryDict = file.LdstrEntries;

            foreach (var entryPair in entryDict)
            {
                var method = Utils.FindMethodByID(module, entryPair.Key);
                if (method == null)
                {
                    continue;
                }

                var e = entryPair.Value;

                if (!HaveTranslation(e))
                {
                    continue;
                }

                var modification = new ILContext.Manipulator(il =>
                {
                    foreach (var instruction in il.Instrs)
                    {
                        var ins = e.Instructions.FirstOrDefault(i => instruction.MatchLdstr(i.Origin));
                        if (ins == null || string.IsNullOrEmpty(ins.Translation))
                        {
                            continue;
                        }

                        instruction.Operand = ins.Translation;

                        foreach (var label in il.Labels)
                        {
                            if (label.Target.MatchLdstr(ins.Origin))
                            {
                                label.Target = instruction;
                            }
                        }
                    }
                });

                if (!modifications.ContainsKey(method))
                {
                    HookEndpointManager.Modify(method, modification);
                    modifications.Add(method, modification);
                }
            }
        }

        public override void Reset()
        {
            return;
        }

        protected override void DisposeUnmanaged()
        {
            modifications = null;
        }
    }
}
