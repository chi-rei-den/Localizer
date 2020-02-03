using Localizer.UIs.Views;
using Squid;

namespace Localizer.UIs
{
    public class UIDesktop : Desktop
    {
        public TestView TestView;
        public ReloadPluginView ReloadPluginView;
        public UIDesktop()
        {
            //            TestView = new TestView();
            //            TestView.Parent = this;

            //ReloadPluginView = new ReloadPluginView
            //{
            //Parent = this
            //};
        }

        public void AddWindow(Window window)
        {
            window.Parent = this;
        }
    }
}
