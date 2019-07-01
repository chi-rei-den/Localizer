using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Localizer
{
    public class Localizer : Mod
    {
		public readonly string WPFPath = "WPF/";

		private List<ExternalModule> modules = new List<ExternalModule>();

		public Localizer()
		{
			AddResolve();
		}

        public override void Load()
        {
			var asm = Assembly.Load(GetWPFFileBytes("LocalizerUI.dll"));

			foreach (var type in asm.GetExportedTypes())
			{
				if (type.IsSubclassOf(typeof(ExternalModule)) && type.IsPublic && !type.IsAbstract)
				{
					var instance = (ExternalModule)Activator.CreateInstance(type);
					modules.Add(instance);
					instance.Run();
				}
			}
		}

		private void AddResolve()
		{
			AppDomain.CurrentDomain.AssemblyResolve += (object _, ResolveEventArgs sargs) =>
			{
				if (new AssemblyName(sargs.Name).Name == "Localizer")
				{
					return Assembly.GetExecutingAssembly();
				}

				var fileName = new AssemblyName(sargs.Name).Name + ".dll";

				var asmFile = GetWPFFileBytes(fileName);

				if (asmFile != null)
					return Assembly.Load(asmFile);

				return null;
			};
		}

		private byte[] GetWPFFileBytes(string fileName)
		{
			return GetFileBytes($"{WPFPath}{fileName}");
		}
    }
}
