using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Squid;
using Terraria;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace Localizer.UIs
{
    public class UIHost : Disposable
    {
        public UIDesktop Desktop { get; private set; }

        private int _lastScroll;

        private GraphicsDevice gd;

        public UIHost()
        {
            gd = Main.graphics.GraphicsDevice;

            Gui.Renderer = new UIRenderer();

            Desktop = new UIDesktop { Name = "localizer" };
            Desktop.ShowCursor = true;
            Desktop.Skin = Stylesheet.GetSkin();
            Desktop.Size = new Squid.Point(gd.Viewport.Width, gd.Viewport.Height);
        }

        internal void Update(GameTime time)
        {
            Gui.TimeElapsed = (float)time.ElapsedGameTime.TotalMilliseconds;

            if (Main.graphics.GraphicsDevice.Viewport.Width != Desktop.Size.x || Main.graphics.GraphicsDevice.Viewport.Height != Desktop.Size.y)
            {
                Desktop.Size = new Squid.Point(gd.Viewport.Width, gd.Viewport.Height);
            }

            if (!Main.hasFocus)
            {
                return;
            }

            // Mouse
            var mouseState = Mouse.GetState();

            var wheel = mouseState.ScrollWheelValue > _lastScroll ? -1 : (mouseState.ScrollWheelValue < _lastScroll ? 1 : 0);
            _lastScroll = mouseState.ScrollWheelValue;

            Gui.SetMouse(mouseState.X, mouseState.Y, wheel);
            Gui.SetButtons(mouseState.LeftButton == ButtonState.Pressed, mouseState.RightButton == ButtonState.Pressed);

            //TODO: Keyboard Input Support
        }

        internal void Draw(GameTime time)
        {
            Desktop.Update();
            Desktop.Draw();
        }

        protected override void DisposeUnmanaged()
        {
            Gui.Renderer = null;
        }
    }
}
