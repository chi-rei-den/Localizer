using Squid;

namespace Localizer.UIs.Components
{
    public class TitleBar : Label
    {
        public Button Button { get; private set; }

        public TitleBar()
        {
            Button = new Button();
            Button.Size = new Point(30, 30);
            Button.Style = "button";
            Button.Text = "X";
            Button.TextAlign = Alignment.MiddleCenter;
            Button.Tooltip = "Close Window";
            Button.Dock = DockStyle.Right;
            Button.Margin = new Margin(0, 8, 8, 8);
            Elements.Add(Button);
        }
    }
}
