using Squid;

namespace Localizer.UIs.Components
{
    public class LabelListBoxItem : ListBoxItem
    {
        public Label Label
        {
            get;
            set;
        }

        public new string Text => Label.Text;

        public LabelListBoxItem(string text)
        {
            base.Margin = new Margin(0, 5, 0, 5);
            Label = new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                Style = "label"
            };
        }
    }
}
