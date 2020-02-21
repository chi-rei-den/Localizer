using Localizer.UIs.Components;
using Squid;

namespace Localizer.UIs.Views
{
    public class ReloadPluginView : BasicWindow
    {
        public ReloadPluginView()
        {
            Size = new Point(40, 40);
            Position = new Point(0, 0);
            Resizable = false;

            var reloadPlugins = new Button()
            {
                Size = new Point(40, 40),
                Style = "button",
            };

            reloadPlugins.MouseClick += (sender, args) =>
            {
                Localizer.Kernel.UnloadAllPlugins();
                Localizer.Kernel.LoadPlugins();
            };

            Controls.Add(reloadPlugins);
        }
    }
}
