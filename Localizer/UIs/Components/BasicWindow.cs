using Squid;

namespace Localizer.UIs.Components
{
    public class BasicWindow : Window
    {
        public TitleBar Titlebar { get; private set; }

        public BasicWindow()
        {
            AllowDragOut = true;
            Padding = new Margin(4);
            Titlebar = new TitleBar();
            Titlebar.Dock = DockStyle.Top;
            Titlebar.Size = new Squid.Point(122, 35);
            Titlebar.MouseDown += (sender, args) => { StartDrag(); };
            Titlebar.MouseUp += (sender, args) => { StopDrag(); };
            Titlebar.Cursor = Cursors.Move;
            Titlebar.Style = "frame";
            Titlebar.Margin = new Margin(-4, -4, -4, -1);
            Titlebar.Button.MouseClick += Button_OnMouseClick;
            Titlebar.TextAlign = Alignment.MiddleLeft;
            Titlebar.BBCodeEnabled = true;
            AllowDragOut = false;

            Controls.Add(Titlebar);
        }

        void Button_OnMouseClick(Control sender, MouseEventArgs args)
        {
            Animation.Custom(FadeAndClose());
        }

        private System.Collections.IEnumerator FadeAndClose()
        {
            yield return Animation.Opacity(0, 500);
            Close();
        }
    }
}
