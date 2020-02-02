using System;
using System.Collections.Generic;
using System.Reflection;
using Localizer.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Squid;
using Terraria;
using Terraria.UI.Chat;
using Point = Squid.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Localizer.UIs
{
    public class UIRenderer : ISquidRenderer
    {
        private SpriteBatch _spriteBatch;

        private Dictionary<string, int> textureIDs;
        private Dictionary<int, Texture2D> textures;
        private Dictionary<string, int> fontIDs;
        private Dictionary<int, DynamicSpriteFont> fonts;

        private Texture2D _blankTexture;
        private RasterizerState _rasterizer;
        private SamplerState _sampler;

        public UIRenderer()
        {
            _spriteBatch = new SpriteBatch(Main.graphics.GraphicsDevice);

            _blankTexture = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1);
            _blankTexture.SetData(new[] { new Color(255, 255, 255, 255) });

            textureIDs = new Dictionary<string, int>();
            textures = new Dictionary<int, Texture2D>();
            fontIDs = new Dictionary<string, int>();
            fonts = new Dictionary<int, DynamicSpriteFont>();

            var modTextures = Localizer.Instance.ValueOf<IDictionary<string, Texture2D>>("textures");
            foreach (var t in modTextures)
            {
                AddTexture(t.Key, t.Value);
            }

            AddFont("default", Main.fontMouseText);
            AddFont("mouse", Main.fontMouseText);
            AddFont("item", Main.fontItemStack);
            AddFont("death", Main.fontDeathText);

            _rasterizer = new RasterizerState
            {
                ScissorTestEnable = true
            };

            _sampler = new SamplerState
            {
                Filter = TextureFilter.Anisotropic
            };
        }

        public void AddFont(string key, DynamicSpriteFont font)
        {
            var index = fontIDs.Count;
            fontIDs.Add(key, index);
            fonts.Add(index, font);
        }

        public void AddTexture(string key, Texture2D tex)
        {
            var index = textureIDs.Count;
            textureIDs.Add(key, index);
            textures.Add(index, tex);
        }

        public void Dispose()
        {
        }

        public int GetTexture(string name)
        {
            return textureIDs[name];
        }

        public int GetFont(string name)
        {
            return fontIDs[name];
        }

        public Point GetTextSize(string text, int font)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new Point();
            }

            var f = fonts[font];
            var size = f.MeasureString(text);
            return new Point((int)size.X, (int)size.Y);
        }

        public string WordWrap(string text, int width)
        {
            var result = "";
            var line = "";
            var index = 0;
            while (index < text.Length)
            {
                line += text[index];
                if (GetTextSize(line, fontIDs["default"]).x > width)
                {
                    result += Environment.NewLine + line.Substring(0, line.Length - 1);
                    line = "";
                }
                else
                {
                    index++;
                }
            }
            return result.Trim();
        }

        public Point GetTextureSize(int texture)
        {
            var tex = textures[texture];
            return new Point(tex.Width, tex.Height);
        }

        public void Scissor(int x, int y, int w, int h)
        {
            Main.graphics.GraphicsDevice.ScissorRectangle
                = new Rectangle(Math.Max(x, 0), Math.Max(y, 0), Math.Min(w, Main.screenWidth), Math.Min(h, Main.screenHeight));
        }

        public void DrawBox(int x, int y, int w, int h, int color)
        {
            var rect = new Rectangle(x, y, w, h);
            _spriteBatch.Draw(_blankTexture, rect, rect, ColorFromtInt32(color));
        }

        private Color ColorFromtInt32(int color)
        {
            var bytes = BitConverter.GetBytes(color);
            return new Color(bytes[2], bytes[1], bytes[0], bytes[3]);
        }
        public void DrawText(string text, int x, int y, int font, int color)
        {
            var snippets = ChatManager.ParseMessage(text, ColorFromtInt32(color)).ToArray();
            Terraria.Utils.DrawBorderStringFourWay(_spriteBatch,
                                                   fonts[font], text, x, y + 3,
                                                   ColorFromtInt32(color),
                                                   Color.Black, Vector2.Zero, 1f);
            //            ChatManager.DrawColorCodedString(
            //                _spriteBatch, fonts[font], snippets, new Vector2(x, y + 3),
            //                ColorFromtInt32(color), 0, Vector2.Zero, Vector2.One, out int i, 99999);
        }
        public void DrawTexture(int texture, int x, int y, int w, int h, Squid.Rectangle rect, int color)
        {
            var destination = new Rectangle(x, y, w, h);
            var source = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height);
            _spriteBatch.Draw(textures[texture], destination, source, ColorFromtInt32(color));
        }

        public bool TranslateKey(int scancode, ref char character)
        {
            character = ' ';
            return true;
        }

        public void StartBatch()
        {
            _spriteBatch.SafeBegin();
        }

        public void EndBatch(bool final)
        {
            _spriteBatch.SafeEnd();
        }
    }
}
