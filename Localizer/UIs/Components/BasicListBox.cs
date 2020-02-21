using Squid;

namespace Localizer.UIs.Components
{
    public class BasicListBox : ListBox
    {
        public BasicListBox()
        {
            base.Dock = DockStyle.Fill;
            base.Scrollbar.Size = new Point(14, 10);
            base.Scrollbar.Slider.Style = "vscrollTrack";
            base.Scrollbar.Slider.Button.Style = "vscrollButton";
            base.Scrollbar.ButtonUp.Style = "vscrollUp";
            base.Scrollbar.ButtonUp.Size = new Point(10, 20);
            base.Scrollbar.ButtonDown.Style = "vscrollUp";
            base.Scrollbar.ButtonDown.Size = new Point(10, 20);
        }
    }
}
