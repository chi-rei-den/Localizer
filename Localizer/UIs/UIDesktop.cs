using Localizer.UIs.Components;
using Localizer.UIs.Views;
using Squid;

namespace Localizer.UIs
{
    public class UIDesktop : Desktop
    {
        public TestView TestView;
        
        public UIDesktop()
        {
            TestView = new TestView();
            TestView.Parent = this;
        }
    }
}
