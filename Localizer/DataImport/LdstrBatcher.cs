using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using MonoMod.Utils;

namespace Localizer.DataModel
{
    public class LdstrBatcher : Batcher
    {
        public static LdstrBatcher Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LdstrBatcher();
                }

                return _instance;
            }
        }

        protected static LdstrBatcher _instance;

        protected Dictionary<MethodBase, ILContext.Manipulator> modifications;

        protected LdstrBatcher()
        {
            this.files = new List<File>();

            this.entries = new Dictionary<string, IEntry>();
            
            this.modifications = new Dictionary<MethodBase, ILContext.Manipulator>();
            
            HookEndpointManager.OnModify += RedirectDMD;
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

        protected override void MergeEntry(string key, IEntry toMerge)
        {
            var e = this.entries[key] as LdstrEntry;
            var newEntry = toMerge as LdstrEntry;
            foreach (var ins in newEntry.Instructions)
            {
                if (!e.Instructions.Exists(i => i.Origin == ins.Origin))
                {
                    e.Instructions.Add(ins.Clone() as BaseEntry);
                }
            }
        }

        protected override void ApplyEntry(string key, IEntry entry)
        {
            var module = HookEndpointManager.GenerateCecilModule(mod.Code.GetName());
            var md = module.FindMethod(key);
            if (md == null)
            {
                return;
            }

            var method = MethodBase.GetMethodFromHandle(md.ResolveReflection().MethodHandle);

            var e = entry as LdstrEntry;

            if (!this.HaveTranslation(e))
            {
                return;
            }

            try
            {
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
                            if(label.Target.MatchLdstr(ins.Origin))
                                label.Target = instruction;
                        }
                    }
                });
                if (!modifications.ContainsKey(method))
                {
                    HookEndpointManager.Modify(method, modification);
                    modifications.Add(method, modification);
                }
            }
            catch (Exception ex)
            {
                Localizer.Log.Error($"Error when apply {key}: {ex}");
            }
        }

        protected override void Undo()
        {
            try
            {
                foreach (var m in modifications)
                {
                    try
                    {
                        HookEndpointManager.Unmodify(m.Key, m.Value);
                        HookEndpointManager.Remove(m.Key, m.Value);
                    }
                    catch (Exception ex)
                    {
                        Localizer.Log.Error(ex);
                    }
                }
            
                modifications.Clear();
            }
            catch (Exception ex)
            {
                Localizer.Log.Error(ex);
            }
        }

        protected bool HaveTranslation(LdstrEntry entry)
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