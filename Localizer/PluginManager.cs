using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;

namespace Localizer
{
    public class PluginManager
    {
        private static readonly string LocailzerBuildPathFilePath = "localizer_build_path";

        private static string InternalPluginDirPath = "Plugins/";
        private static readonly string ExternalPluginDirPath = Main.SavePath + "Localizer/Plugins/";

        private static readonly string[] InternalPlugins = new string[]
        {
            "LocalizerWPF.dll",
        };

        public static List<Plugin> Plugins { get; set; }

        public static void Init()
        {
            AddResolve();
        }

        public static void LoadPlugins()
        {
#if DEBUG
            InternalPluginDirPath = File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), LocailzerBuildPathFilePath));
#endif
            Plugins = new List<Plugin>();

            LoadInternalPlugins();
        }

        public static void UnloadPlugins()
        {
            foreach (var p in Plugins)
            {
                p.Dispose();
            }
        }

        private static void LoadInternalPlugins()
        {
            var pluginFiles = InternalPlugins;
            foreach (var file in pluginFiles)
            {
                var bytes = GetInternalPluginFileBytes(file);
                if (bytes?.Length == null)
                {
                    return;
                }

                LoadPlugin(Assembly.Load(bytes));
            }

        }

        private static void LoadPlugin(Assembly asm)
        {
            try
            {
                foreach (var type in asm.GetExportedTypes())
                {
                    if (type.IsSubclassOf(typeof(Plugin)) && type.IsPublic && !type.IsAbstract)
                    {
                        Localizer.Log.Info($"Loading plugin [{asm.GetName()}]...");
                        var instance = (Plugin) Activator.CreateInstance(type);
                        Plugins.Add(instance);
                        instance.Initialize();
                        Localizer.Log.Info($"Plugin[{asm.GetName()}] loaded.");
                    }
                }
            }
            catch (Exception e)
            {
                Localizer.Log.Error($"Error when loading Plugin [{asm.GetName()}]. \nError: {e}");
            }
        }

        private static byte[] GetInternalPluginFileBytes(string fileName)
        {
#if DEBUG
            var path = $"{InternalPluginDirPath}{Path.GetFileName(fileName)}";
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }

            return null;
#else
            return Localizer.Instance.GetFileBytes($"{InternalPluginDirPath}{fileName}");
#endif
        }

        private static void AddResolve()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (_, sargs) =>
            {
                try
                {
                    if (new AssemblyName(sargs.Name).Name == "Localizer")
                    {
                        return Assembly.GetExecutingAssembly();
                    }

                    var fileName = new AssemblyName(sargs.Name).Name + ".dll";

                    var asmFile = GetInternalPluginFileBytes(fileName);

                    return asmFile != null && asmFile.Length != 0 ? Assembly.Load(asmFile) : null;
                }
                catch (Exception)
                {
                    //Localizer.Log.Error(e);
                    return null;
                }
            };
        }
    }
}
