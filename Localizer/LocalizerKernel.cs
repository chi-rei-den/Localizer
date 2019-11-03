using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Localizer.Modules;
using Ninject;
using Ninject.Modules;

namespace Localizer
{
    public sealed class LocalizerKernel : StandardKernel
    {
        public List<LocalizerPlugin> Plugins { get; private set; }
        
        public Dictionary<string, bool> PluginEnableStatus { get; private set; }

        private static readonly string InternalPluginDirPath = "Plugins/";
        private static readonly string ExternalPluginDirPath = "./Localizer/Plugins/";
        
        private static readonly string[] InternalPlugins =
        {
            "LocalizerWPF.dll",
            "ModBrowserMirror.dll",
        };
        
        public LocalizerKernel()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveHandler;
            
            Plugins = new List<LocalizerPlugin>();
            PluginEnableStatus = new Dictionary<string, bool>();
            
            Load(new NinjectModule[]
            {
                new DefaultPackageModule(), new DefaultFileExportModule(),
                new DefaultFileUpdateModule(), new DefaultFileImportModule(),
                new DefaultNetworkModule(),
            });

            LoadInternalPlugin();
        }

        public override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolveHandler;

                if (disposing)
                {
                    foreach (var plugin in Plugins)
                    {
                        plugin.Dispose();
                    }

                    Plugins = null;
                }
            }
            base.Dispose(disposing);
        }

        internal void LoadInternalPlugin()
        {
            var internalPlugins = new List<Assembly>();
#if DEBUG
            var eacFileInfo = new FileInfo(Localizer.GetEacPath());
            var pluginDir = new DirectoryInfo(Path.Combine(eacFileInfo.Directory.Parent.Parent.FullName, "Plugins"));
            foreach (var pluginFileName in InternalPlugins)
            {
                internalPlugins.Add(Assembly.Load(File.ReadAllBytes(Path.Combine(pluginDir.FullName, pluginFileName))));
            }
#else
            foreach (var pluginFileName in InternalPlugins)
            {
                internalPlugins.Add(Assembly.Load(GetInternalPluginFileBytes(pluginFileName)));
            }
#endif
            foreach (var plugin in internalPlugins)
            {
                LoadPlugin(plugin);
            }
        }

        internal void LoadPlugin(Assembly asm)
        {
            try
            {
                foreach (var type in asm.GetExportedTypes())
                {
                    if (type.IsSubclassOf(typeof(LocalizerPlugin)) && type.IsPublic && !type.IsAbstract)
                    {
                        var instance = (LocalizerPlugin)Activator.CreateInstance(type);
                        Plugins.Add(instance);
                        instance.Initialize(this);
                        Utils.LogInfo($"Plugin [{instance.Name}] loaded.");
                    }
                }
            }
            catch (Exception e)
            {
                Utils.LogError($"Error occured when loading Plugin [{asm.GetName()}]. \nError: {e}");
            }
        }

        internal void UnloadPlugin(string pluginName)
        {
            if(string.IsNullOrWhiteSpace(pluginName))
            {
                throw new ArgumentException(nameof(pluginName));
            }
            
            var plugin = Plugins.SingleOrDefault(p => p.Name == pluginName) 
                ?? throw new Exception($"Cannot find plugin [{pluginName}]");
            
            UnloadPlugin(plugin);
        }
        
        internal void UnloadPlugin(LocalizerPlugin plugin)
        {
            if (plugin is null)
            {
                Utils.LogWarn($"Plugin cannot be unloaded because it's null");
                return;
            }

            if (Plugins.Contains(plugin))
            {
                Utils.LogWarn($"Plugin [{plugin.Name}] doesn't exist");
                return;
            }
            
            plugin.Dispose();

            Plugins.Remove(plugin);
            
            Utils.LogInfo($"Plugin [{plugin.Name}] successfully unloaded.");
        }
        
        private static Assembly AssemblyResolveHandler(object sender, ResolveEventArgs args)
        {
            try
            {
                var asmName = new AssemblyName(args.Name);

                if (asmName.Name == "Localizer")
                {
                    return Localizer.Instance.Code;
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
                return null;
            }
        }

        private static byte[] GetExternalPluginFileBytes(string fileName)
        {            
            var path = Path.Combine(ExternalPluginDirPath, fileName);

            if (!File.Exists(path))
                return null;
            
            return File.ReadAllBytes(path);
        }

        private static byte[] GetInternalPluginFileBytes(string fileName)
        {
            if (Localizer.TmodFile.IsOpen)
            {
                return Localizer.Instance?.GetFileBytes($"{InternalPluginDirPath}{fileName}");
            }
            
            using (Localizer.TmodFile.Open())
            {
                return Localizer.Instance?.GetFileBytes($"{InternalPluginDirPath}{fileName}");
            }
        }
    }
}
