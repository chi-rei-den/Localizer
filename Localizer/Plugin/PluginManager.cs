using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Localizer
{
    public class PluginManager
    {
        private static readonly string LocailzerBuildPathFilePath = "localizer_build_path";

        private static string InternalPluginDirPath = "Plugins/";
        private static readonly string ExternalPluginDirPath = "./Localizer/Plugins/";

        private static readonly string[] InternalPlugins =
        {
            "LocalizerWPF.dll",
            "ModBrowserMirror.dll",
        };

        public static List<Plugin> Plugins { get; set; }

        public static void Init()
        {
            Utils.CreateDirectory(ExternalPluginDirPath);
            AddResolve();
        }

        public static void LoadPlugins()
        {
#if DEBUG
            InternalPluginDirPath =
                File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                              LocailzerBuildPathFilePath));
#endif
            Plugins = new List<Plugin>();

            LoadInternalPlugins();
            LoadExternalPlugins();
        }

        public static void UnloadPlugins()
        {
            foreach (var p in Plugins)
            {
                try
                {
                    p.Dispose();
                }
                catch (Exception e)
                {
                    Utils.LogError($"Error occured when unloading {p.GetType()}: {e}");
                }
            }

            Plugins = null;
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
        
        private static void LoadExternalPlugins()
        {
            var dirInfo = new DirectoryInfo(ExternalPluginDirPath);

            foreach (var file in dirInfo.GetFiles("*.dll", SearchOption.TopDirectoryOnly))
            {
                LoadPlugin(Assembly.Load(GetExternalPluginFileBytes(file.Name)));
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
                        Utils.LogInfo($"Loading plugin [{asm.GetName()}]...");
                        var instance = (Plugin)Activator.CreateInstance(type);
                        Plugins.Add(instance);
                        instance.Initialize();
                        Utils.LogInfo($"Plugin[{asm.GetName()}] loaded.");
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogError($"Error when loading Plugin [{asm.GetName()}]. \nError: {e}");
            }
        }

        private static byte[] GetInternalPluginFileBytes(string fileName)
        {
#if DEBUG
            var path = $"{InternalPluginDirPath}{Path.GetFileName(fileName)}";
            if (!path.EndsWith(".dll"))
                path += ".dll";
            
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }

            return null;
#else
            return Localizer.Instance?.GetFileBytes($"{InternalPluginDirPath}{fileName}");
#endif
        }
        
        private static byte[] GetExternalPluginFileBytes(string fileName)
        {
            var path = Path.Combine(ExternalPluginDirPath, fileName);

            if (!File.Exists(path))
                return null;
            
            return File.ReadAllBytes(path);
        }


        private static void AddResolve()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (_, sargs) =>
            {
                try
                {
                    var asmName = new AssemblyName(sargs.Name);

                    if (asmName.Name == "Localizer")
                    {
                        return Assembly.GetExecutingAssembly();
                    }

                    var referencedAsm = Assembly.GetExecutingAssembly().GetReferencedAssemblies()
                                                .FirstOrDefault(a => a.Name.StartsWith($"Localizer_{asmName.Name}_"));
                    if (referencedAsm != null)
                    {
                        return Assembly.Load(referencedAsm);
                    }

                    var fileName = asmName.Name + ".dll";

                    var asmFile = GetInternalPluginFileBytes(fileName);

                    if (asmFile != null && asmFile.Length != 0)
                    {
                        return Assembly.Load(asmFile);
                    }

                    asmFile = GetExternalPluginFileBytes(fileName);

                    if (asmFile != null && asmFile.Length != 0)
                    {
                        return Assembly.Load(asmFile);
                    }
                    
                    return null;
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
