using System;
using System.Reflection;

namespace LocalizerWPF
{
    internal class Program
    {
        [STAThread]
        public static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (object _, ResolveEventArgs sargs) =>
            {
                var fileName = new AssemblyName(sargs.Name).Name + ".dll";
                var name = Array.Find(typeof(Program).Assembly.GetManifestResourceNames(), (string element) => element.EndsWith(fileName));
                if (name == null)
                {
                    return null;
                }
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
                {
                    var array = new byte[stream.Length];
                    stream.Read(array, 0, array.Length);
                    return Assembly.Load(array);
                }
            };
            App.Main();
        }
    }
}
