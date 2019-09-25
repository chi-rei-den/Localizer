using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using MonoMod.Utils;

namespace Localizer.Services.File
{
    public class LdstrFileImportService : IFileImportService
    {
        private Dictionary<MethodBase, ILContext.Manipulator> modifications;
        
        public LdstrFileImportService()
        {
            HookEndpointManager.OnModify += RedirectDMD;
            modifications = new Dictionary<MethodBase, ILContext.Manipulator>();
        }

        public void Import(IFile file, IMod mod)
        {
            if (file.GetType() != typeof(LdstrFile))
            {
                return;
            }

            var module = HookEndpointManager.GenerateCecilModule(mod.Code.GetName());

            var entryDict = (file as LdstrFile).LdstrEntries;

            foreach (var entryPair in entryDict)
            {
                var md = module.FindMethod(entryPair.Key);
                if (md == null)
                {
                    continue;
                }

                var method = MethodBase.GetMethodFromHandle(md.ResolveReflection().MethodHandle);

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

        private bool RedirectDMD(MethodBase methodBase, Delegate callback)
        {
            var endPoint = GetHookEndPoint(methodBase);

            var dmd = new DynamicMethodDefinition(methodBase);
            dmd.Reload(null, true);
            ReplaceDMD(endPoint, dmd);

            return true;
        }

        private object GetHookEndPoint(MethodBase method)
        {
            return typeof(HookEndpointManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                                              .FirstOrDefault(m => m.Name.Contains("GetEndpoint"))
                                              ?.Invoke(null, new[] {method});
        }

        private void ReplaceDMD(object endPoint, DynamicMethodDefinition dmd)
        {
            typeof(HookEndpointManager).Module.GetType("MonoMod.RuntimeDetour.HookGen.HookEndpoint")
                                       ?.GetField("_DMD", BindingFlags.Instance | BindingFlags.NonPublic)
                                       ?.SetValue(endPoint, dmd);
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
    }
}
