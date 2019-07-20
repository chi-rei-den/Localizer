using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        protected LdstrBatcher()
        {
            this.files = new List<File>();

            this.entries = new Dictionary<string, IEntry>();
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
            var methodInfo = this.mod.Code.ManifestModule.FindMethod(key);
            if (methodInfo == null)
            {
                return;
            }

            var method = MethodBase.GetMethodFromHandle(methodInfo.MethodHandle);

            var e = entry as LdstrEntry;

            if (!this.HaveTranslation(e))
            {
                return;
            }

            try
            {
                HookEndpointManager.Modify(method, new ILContext.Manipulator(il =>
                {
                    var c = new ILCursor(il);
                    while (c.TryGotoNext(i => i.Match(OpCodes.Ldstr)))
                    {
                        var instr = il.Instrs[c.Index];
                        var ins = e.Instructions.FirstOrDefault(i => instr.MatchLdstr(i.Origin));
                        if (ins == null || string.IsNullOrEmpty(ins.Translation))
                        {
                            continue;
                        }

                        c.Remove();
                        c.Emit(OpCodes.Ldstr, ins.Translation);
                    }
                }));
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