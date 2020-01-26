using Localizer.UIs.Components;
using Squid;

namespace Localizer.UIs.Views
{
    public class TestView : BasicWindow
    {
        public TestView()
        {
            Size = new Point(440, 340);
            Position = new Point(40, 40);
            Titlebar.Text = "[color=FfFfFf00]BBCode[/color]中文测试";
            Resizable = true;

            var label1 = new Label
            {
                Text = "Username:",
                Size = new Point(122, 35),
                Position = new Point(60, 100),
                Parent = this
            };

            label1.MousePress += (sender, args) =>
            {
            };

            var textbox1 = new TextBox { Name = "textbox" };
            textbox1.Text = "username";
            textbox1.Size = new Point(222, 35);
            textbox1.Position = new Point(180, 100);
            textbox1.Style = "textbox";
            textbox1.Parent = this;
            textbox1.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            var label2 = new Label
            {
                Text = "Password:",
                Size = new Point(122, 35),
                Position = new Point(60, 140),
                Parent = this
            };

            var textbox2 = new TextBox { Name = "textbox" };
            textbox2.PasswordChar = char.Parse("*");
            textbox2.IsPassword = true;
            textbox2.Text = "password";
            textbox2.Size = new Point(222, 35);
            textbox2.Position = new Point(180, 140);
            textbox2.Style = "textbox";
            textbox2.Parent = this;
            textbox2.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            var button = new Button
            {
                Size = new Point(157, 35),
                Position = new Point(437 - 192, 346 - 52),
                Text = "Login",
                Style = "button",
                Parent = this,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Cursor = CursorNames.Link
            };
            button.MouseClick += (sender, args) =>
            {
            };

            var combo = new DropDownList
            {
                Size = new Point(222, 35),
                Position = new Point(180, 180),
                Parent = this
            };
            combo.Label.Style = "comboLabel";
            combo.Button.Style = "comboButton";
            combo.Listbox.Margin = new Margin(0, 6, 0, 0);
            combo.Listbox.Style = "frame";
            combo.Listbox.ClipFrame.Margin = new Margin(8, 8, 8, 8);
            combo.Listbox.Scrollbar.Margin = new Margin(0, 4, 4, 4);
            combo.Listbox.Scrollbar.Size = new Point(14, 10);
            combo.Listbox.Scrollbar.ButtonUp.Style = "vscrollUp";
            combo.Listbox.Scrollbar.ButtonUp.Size = new Point(10, 20);
            combo.Listbox.Scrollbar.ButtonDown.Style = "vscrollUp";
            combo.Listbox.Scrollbar.ButtonDown.Size = new Point(10, 20);
            combo.Listbox.Scrollbar.Slider.Margin = new Margin(0, 2, 0, 2);
            combo.Listbox.Scrollbar.Slider.Style = "vscrollTrack";
            combo.Listbox.Scrollbar.Slider.Button.Style = "vscrollButton";
            combo.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            for (var i = 0; i < 10; i++)
            {
                var item = new ListBoxItem();
                var label = new Label()
                {
                    Text = "listboxitem",
                    Size = new Point(100, 35),
                    Margin = new Margin(0, 0, 0, 4),
                    Style = "item",
                };
                item.Container.Controls.Add(label);
                combo.Items.Add(item);
                if (i == 3)
                {
                    item.Selected = true;
                }
            }

            var box = new CheckBox
            {
                Size = new Point(157, 26),
                Position = new Point(180, 220),
                Text = "Remember me",
                Parent = this
            };
            box.Button.Style = "checkBox";
            box.Button.Size = new Point(26, 26);
            box.Button.Cursor = CursorNames.Link;
        }
    }
}
