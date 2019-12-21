using Localizer.UIs.Components;
using Squid;

namespace Localizer.UIs.Views
{
    public class TestView : BasicWindow
    {
        public TestView()
        {
            this.Size = new Squid.Point(440, 340);
            this.Position = new Squid.Point(40, 40);
            this.Titlebar.Text = "[color=FfFfFf00]BBCode[/color]中文测试";
            this.Resizable = true;

            Label label1 = new Label();
            label1.Text = "Username:";
            label1.Size = new Squid.Point(122, 35);
            label1.Position = new Squid.Point(60, 100);
            label1.Parent = this;
            label1.MousePress += (sender, args) =>
            {
            };

            TextBox textbox1 = new TextBox { Name = "textbox" };
            textbox1.Text = "username";
            textbox1.Size = new Squid.Point(222, 35);
            textbox1.Position = new Squid.Point(180, 100);
            textbox1.Style = "textbox";
            textbox1.Parent = this;
            textbox1.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            Label label2 = new Label();
            label2.Text = "Password:";
            label2.Size = new Squid.Point(122, 35);
            label2.Position = new Squid.Point(60, 140);
            label2.Parent = this;

            TextBox textbox2 = new TextBox { Name = "textbox" };
            textbox2.PasswordChar = char.Parse("*");
            textbox2.IsPassword = true;
            textbox2.Text = "password";
            textbox2.Size = new Squid.Point(222, 35);
            textbox2.Position = new Squid.Point(180, 140);
            textbox2.Style = "textbox";
            textbox2.Parent = this;
            textbox2.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            Button button = new Button();
            button.Size = new Squid.Point(157, 35);
            button.Position = new Squid.Point(437 - 192, 346 - 52);
            button.Text = "Login";
            button.Style = "button";
            button.Parent = this;
            button.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button.Cursor = CursorNames.Link;
            button.MouseClick += (sender, args) =>
            {
            };

            DropDownList combo = new DropDownList();
            combo.Size = new Squid.Point(222, 35);
            combo.Position = new Squid.Point(180, 180);
            combo.Parent = this;
            combo.Label.Style = "comboLabel";
            combo.Button.Style = "comboButton";
            combo.Listbox.Margin = new Margin(0, 6, 0, 0);
            combo.Listbox.Style = "frame";
            combo.Listbox.ClipFrame.Margin = new Margin(8, 8, 8, 8);
            combo.Listbox.Scrollbar.Margin = new Margin(0, 4, 4, 4);
            combo.Listbox.Scrollbar.Size = new Squid.Point(14, 10);
            combo.Listbox.Scrollbar.ButtonUp.Style = "vscrollUp";
            combo.Listbox.Scrollbar.ButtonUp.Size = new Squid.Point(10, 20);
            combo.Listbox.Scrollbar.ButtonDown.Style = "vscrollUp";
            combo.Listbox.Scrollbar.ButtonDown.Size = new Squid.Point(10, 20);
            combo.Listbox.Scrollbar.Slider.Margin = new Margin(0, 2, 0, 2);
            combo.Listbox.Scrollbar.Slider.Style = "vscrollTrack";
            combo.Listbox.Scrollbar.Slider.Button.Style = "vscrollButton";
            combo.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            for (int i = 0; i < 10; i++)
            {
                ListBoxItem item = new ListBoxItem();
                var label = new Label()
                {
                    Text = "listboxitem",
                    Size = new Squid.Point(100, 35),
                    Margin = new Margin(0, 0, 0, 4),
                    Style = "item",
                };
                item.Container.Controls.Add(label);
                combo.Items.Add(item);
                if (i == 3)
                    item.Selected = true;
            }

            CheckBox box = new CheckBox();
            box.Size = new Squid.Point(157, 26);
            box.Position = new Squid.Point(180, 220);
            box.Text = "Remember me";
            box.Parent = this;
            box.Button.Style = "checkBox";
            box.Button.Size = new Squid.Point(26, 26);
            box.Button.Cursor = CursorNames.Link;
        }
    }
}
