using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Squid;
using Terraria;
using Terraria.GameInput;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace Localizer.UIs
{
    public class UIHost
    {
        private int _lastScroll;
        
        private Desktop desktop;
        private GraphicsDevice gd;
        
        public UIHost()
        {
            gd = Main.graphics.GraphicsDevice;
            
            GuiHost.Renderer = new UIRenderer();
            GuiHost.SetSkin(Stylesheet.GetSkin());
            
            desktop = new UIDesktop { Name = "localizer" };
            desktop.ShowCursor = true;
            desktop.Size = new Squid.Point(gd.Viewport.Width, gd.Viewport.Height);
        }

        internal void Update(GameTime time)
        {
            GuiHost.TimeElapsed = (float)time.ElapsedGameTime.TotalMilliseconds;
            
            // Mouse
            MouseState mouseState = Mouse.GetState();

            int wheel = mouseState.ScrollWheelValue > _lastScroll ? -1 : (mouseState.ScrollWheelValue < _lastScroll ? 1 : 0);
            _lastScroll = mouseState.ScrollWheelValue;

            GuiHost.SetMouse(mouseState.X, mouseState.Y, wheel);
            GuiHost.SetButtons(mouseState.LeftButton == ButtonState.Pressed, mouseState.RightButton == ButtonState.Pressed);
            
            //TODO: Keyboard Input Support
        }

        internal void Draw(GameTime time)
        {
            desktop.Update();
            desktop.Draw();
        }
    }
}
