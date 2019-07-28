using Localizer.DataModel;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.RuntimeDetour.HookGen;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader;
using static Localizer.Utils;

namespace Localizer.DataExport
{
    public class LdstrExporter : Exporter
    {
        private static readonly List<MethodBase> _blackList1 = new List<MethodBase>
        {
            GetMethodBase<ModTranslation>("System.Void Terraria.ModLoader.ModTranslation::SetDefault(System.String)"),
            GetMethodBase<ModTranslation>("System.String Terraria.ModLoader.ModTranslation::GetTranslation(System.String)"),
            GetMethodBase<ModTranslation>("System.Void Terraria.ModLoader.ModTranslation::AddTranslation(System.Int32,System.String)"),
            GetMethodBase<ModTranslation>("System.Void Terraria.ModLoader.ModTranslation::AddTranslation(System.String,System.String)"),
            GetMethodBase<ModTranslation>("System.Void Terraria.ModLoader.ModTranslation::AddTranslation(Terraria.Localization.GameCulture,System.String)"),
            GetMethodBase<Mod>("Microsoft.Xna.Framework.Graphics.Texture2D Terraria.ModLoader.Mod::GetTexture(System.String)"),
            GetMethodBase<Mod>("Microsoft.Xna.Framework.Audio.SoundEffect Terraria.ModLoader.Mod::GetSound(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.Audio.Music Terraria.ModLoader.Mod::GetMusic(System.String)"),
            GetMethodBase<Mod>("ReLogic.Graphics.DynamicSpriteFont Terraria.ModLoader.Mod::GetFont(System.String)"),
            GetMethodBase<Mod>("Microsoft.Xna.Framework.Graphics.Effect Terraria.ModLoader.Mod::GetEffect(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.Config.ModConfig Terraria.ModLoader.Mod::GetConfig(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.GlobalProjectile Terraria.ModLoader.Mod::GetGlobalProjectile(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModNPC Terraria.ModLoader.Mod::GetNPC(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::NPCType(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.GlobalNPC Terraria.ModLoader.Mod::GetGlobalNPC(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModPlayer Terraria.ModLoader.Mod::GetPlayer(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModBuff Terraria.ModLoader.Mod::GetBuff(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::BuffType(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.GlobalBuff Terraria.ModLoader.Mod::GetGlobalBuff(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModMountData Terraria.ModLoader.Mod::GetMount(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::MountType(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModWorld Terraria.ModLoader.Mod::GetModWorld(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModUgBgStyle Terraria.ModLoader.Mod::GetUgBgStyle(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModSurfaceBgStyle Terraria.ModLoader.Mod::GetSurfaceBgStyle(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::GetSurfaceBgStyleSlot(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.GlobalBgStyle Terraria.ModLoader.Mod::GetGlobalBgStyle(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModWaterStyle Terraria.ModLoader.Mod::GetWaterStyle(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModWaterfallStyle Terraria.ModLoader.Mod::GetWaterfallStyle(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::GetWaterfallStyleSlot(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::GetGoreSlot(System.String)"),
            GetMethodBase<Mod>("System.Void Terraria.ModLoader.Mod::AddBackgroundTexture(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::GetBackgroundSlot(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModTranslation Terraria.ModLoader.Mod::CreateTranslation(System.String)"),
            GetMethodBase<Mod>("System.Byte[] Terraria.ModLoader.Mod::GetFileBytes(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModItem Terraria.ModLoader.Mod::GetItem(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::ItemType(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.GlobalItem Terraria.ModLoader.Mod::GetGlobalItem(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModPrefix Terraria.ModLoader.Mod::GetPrefix(System.String)"),
            GetMethodBase<Mod>("System.Byte Terraria.ModLoader.Mod::PrefixType(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModDust Terraria.ModLoader.Mod::GetDust(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::DustType(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModTile Terraria.ModLoader.Mod::GetTile(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::TileType(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.GlobalTile Terraria.ModLoader.Mod::GetGlobalTile(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModTileEntity Terraria.ModLoader.Mod::GetTileEntity(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::TileEntityType(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModWall Terraria.ModLoader.Mod::GetWall(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::WallType(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.GlobalWall Terraria.ModLoader.Mod::GetGlobalWall(System.String)"),
            GetMethodBase<Mod>("Terraria.ModLoader.ModProjectile Terraria.ModLoader.Mod::GetProjectile(System.String)"),
            GetMethodBase<Mod>("System.Int32 Terraria.ModLoader.Mod::ProjectileType(System.String)"),
            GetMethodBase<ModContent>("Microsoft.Xna.Framework.Audio.SoundEffect Terraria.ModLoader.ModContent::GetSound(System.String)"),
            GetMethodBase<ModContent>("System.Byte[] Terraria.ModLoader.ModContent::GetFileBytes(System.String)"),
            GetMethodBase<ModContent>("System.Boolean Terraria.ModLoader.ModContent::FileExists(System.String)"),
            GetMethodBase<ModContent>("Microsoft.Xna.Framework.Graphics.Texture2D Terraria.ModLoader.ModContent::GetTexture(System.String)"),
            GetMethodBase<ModContent>("System.Boolean Terraria.ModLoader.ModContent::TextureExists(System.String)"),
            GetMethodBase<ModContent>("Microsoft.Xna.Framework.Audio.SoundEffect Terraria.ModLoader.ModContent::GetSound(System.String)"),
            GetMethodBase<ModContent>("System.Boolean Terraria.ModLoader.ModContent::SoundExists(System.String)"),
            GetMethodBase<ModContent>("Terraria.ModLoader.Audio.Music Terraria.ModLoader.ModContent::GetMusic(System.String)"),
            GetMethodBase<ModContent>("System.Boolean Terraria.ModLoader.ModContent::MusicExists(System.String)"),
            GetMethodBase<ModContent>("System.Int32 Terraria.ModLoader.ModContent::GetModBossHeadSlot(System.String)"),
            GetMethodBase<ModContent>("System.Int32 Terraria.ModLoader.ModContent::GetModHeadSlot(System.String)"),
            GetMethodBase<ModContent>("System.Int32 Terraria.ModLoader.ModContent::GetModBackgroundSlot(System.String)"),
            GetMethodBase<ModContent>("System.Boolean Terraria.ModLoader.ModContent::SoundExists(System.String)"),
            GetMethodBase<ModContent>("System.Boolean Terraria.ModLoader.ModContent::SoundExists(System.String)"),
            GetMethodBase<ModContent>("System.Boolean Terraria.ModLoader.ModContent::SoundExists(System.String)"),
            GetMethodBase<ModRecipe>("System.Void Terraria.ModLoader.ModRecipe::AddTile(Terraria.ModLoader.Mod,System.String)"),
        };

        private static readonly List<MethodBase> _blackList2 = new List<MethodBase>
        {
            GetMethodBase<ModRecipe>("System.Void Terraria.ModLoader.ModRecipe::AddIngredient(Terraria.ModLoader.Mod,System.String,System.Int32)"),
        };

        public LdstrExporter(ExportConfig config)
        {
            this.Config = config;
            this.logger = new LocalizerLogger(this.dirPath);
        }

        protected override Type fileType => typeof(LdstrFile);

        protected override File Extract()
        {
            if (this.Config.Package?.Mod == null)
            {
                return null;
            }

            var asm = this.Config.Package.Mod.Code;

            var file = new LdstrFile();
            var config = (LdstrExportConfig) this.Config;

            file.LdstrEntries = new Dictionary<string, LdstrEntry>();

            foreach (var type in asm.GetTypes())
            {
                /* The return value of GetTypes() and other methods will include types they derived from.
                 So we should check the namespace to ensure it belongs to the assembly, but there still are
                 some issues. */
                if (type.Namespace == null || !type.Namespace.StartsWith(config.Package.Mod.Name))
                {
                    continue;
                }

                foreach (var method in type.GetMethods())
                {
                    if (method.DeclaringType.Namespace == null || !method.DeclaringType.Namespace.StartsWith(config.Package.Mod.Name) || method.IsAbstract)
                    {
                        continue;
                    }

                    try
                    {
                        var entry = this.GetEntryFromMethod(method);
                        if (entry != null && !file.LdstrEntries.ContainsKey(method.GetFindableID()))
                        {
                            file.LdstrEntries.Add(method.GetFindableID(), entry);
                        }
                    }
                    catch (Exception e)
                    {
                        this.logger.Log(e.ToString());
                    }
                }
            }

            return file;
        }

        protected LdstrEntry GetEntryFromMethod(MethodInfo method)
        {
            var dmd = new DynamicMethodDefinition(method, HookEndpointManager.GenerateCecilModule);
            var instructions = dmd.Definition.Body?.Instructions;

            if (instructions == null)
            {
                return null;
            }

            var entry = new LdstrEntry
            {
                Instructions = new List<BaseEntry>()
            };
            for (var i = 0; i < instructions.Count; i++)
            {
                var ins = instructions[i];
                if (ins.OpCode == OpCodes.Ldstr && !string.IsNullOrWhiteSpace(ins.Operand.ToString()))
                {
                    // Filter methods in blacklist1
                    if (i < instructions.Count - 1)
                    {
                        if (ins.Next.OpCode == OpCodes.Call || ins.Next.OpCode == OpCodes.Calli ||
                            ins.Next.OpCode == OpCodes.Callvirt)
                        {
                            if (_blackList1.Any(m => (ins.Next.Operand as MethodReference).Is(m)))
                            {
                                continue;
                            }
                        }
                    }
                    // Filter methods in blacklist2
                    if (i < instructions.Count - 2)
                    {
                        var afterNext = ins.Next.Next;
                        if (afterNext.OpCode == OpCodes.Call || afterNext.OpCode == OpCodes.Calli ||
                            afterNext.OpCode == OpCodes.Callvirt)
                        {
                            if (_blackList2.Any(m => (afterNext.Operand as MethodReference).Is(m)))
                            {
                                continue;
                            }
                        }
                    }
                    // No need to add a same string
                    if (entry.Instructions.Exists(e => e.Origin == ins.Operand.ToString()))
                    {
                        continue;
                    }

                    entry.Instructions.Add(new BaseEntry
                    {
                        Origin = ins.Operand.ToString(),
                        Translation = ""
                    });
                }
            }

            return entry.Instructions.Count == 0 ? null : entry;
        }

        protected override void UpdateEntry(string key, IEntry oldEntry, IEntry newEntry)
        {
            if (!(oldEntry is LdstrEntry oldE) || !(newEntry is LdstrEntry newE))
            {
                return;
            }

            foreach (var newIns in newE.Instructions)
            {
                if (oldE.Instructions.Exists(oi => oi.Origin == newIns.Origin))
                {
                    continue;
                }

                oldE.Instructions.Add(newIns);
                this.logger.TextUpdateLog($"New instruction of {key}: [{newIns}]");
            }
        }
    }
}