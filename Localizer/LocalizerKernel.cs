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

        private static readonly string ExternalPluginDirPath = "./Localizer/Plugins/";
        
        public LocalizerKernel()
        {
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveHandler;
            
            Plugins = new List<LocalizerPlugin>();
            PluginEnableStatus = new Dictionary<string, bool>();
        }

        public void Init()
        {
            Load(new NinjectModule[]
            {
                new DefaultPackageModule(),
                new DefaultNetworkModule(),
            });
            
            LoadPlugins();
        }

        public override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolveHandler;

                if (disposing)
                {
                    UnloadAllPlugins();
                    Plugins = null;
                }
            }
            base.Dispose(disposing);
        }

        internal void UnloadAllPlugins()
        {
            foreach (var plugin in Plugins)
            {
                plugin.Dispose();
            }

            Plugins.Clear();
        }
        
        internal void LoadPlugins()
        {
            Utils.EnsureDir(ExternalPluginDirPath);
            
            var dirInfo = new DirectoryInfo(ExternalPluginDirPath);
            foreach (var file in dirInfo.GetFiles("*.dll"))
            {
                var a = Assembly.Load(File.ReadAllBytes(file.FullName));
                LoadPlugin(a);
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
                                            .FirstOrDefault(a => a.Name.StartsWith($"!Localizer_{asmName.Name}_"));
                if (referencedAsm != null)
                {
                    return Assembly.Load(referencedAsm);
                }

                var fileName = asmName.Name + ".dll";

                var asmFile = GetExternalPluginFileBytes(fileName);

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
    }
}
