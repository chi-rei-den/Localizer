using System.Collections.Generic;
using Squid;

namespace Localizer.UIs
{
    public class Stylesheet
    {
        public static Skin GetSkin()
        {
            var skin = new Skin();

            foreach (var s in GetControlStyles())
            {
                skin.Add(s.Key, s.Value);
            }
            
            return skin;
        }

        public static CursorCollection GetCursorSet()
        {
            Point cursorSize = new Point(32, 32);
            Point halfSize = cursorSize / 2;
            return new CursorCollection()
            {
                {CursorNames.Default, new Cursor { Texture = GetTexturePath("Cursors/Arrow"), Size = cursorSize, HotSpot = Point.Zero }},
                {CursorNames.Link, new Cursor { Texture = GetTexturePath("Cursors/Link"), Size = cursorSize, HotSpot = Point.Zero }},
                {CursorNames.Move, new Cursor { Texture = GetTexturePath("Cursors/Move"), Size = cursorSize, HotSpot = halfSize }},
                {CursorNames.Select, new Cursor { Texture = GetTexturePath("Cursors/Select"), Size = cursorSize, HotSpot = halfSize }},
                {CursorNames.SizeNS, new Cursor { Texture = GetTexturePath("Cursors/SizeNS"), Size = cursorSize, HotSpot = halfSize }},
                {CursorNames.SizeWE, new Cursor { Texture = GetTexturePath("Cursors/SizeWE"), Size = cursorSize, HotSpot = halfSize }},
                {CursorNames.HSplit, new Cursor { Texture = GetTexturePath("Cursors/SizeNS"), Size = cursorSize, HotSpot = halfSize }},
                {CursorNames.VSplit, new Cursor { Texture = GetTexturePath("Cursors/SizeWE"), Size = cursorSize, HotSpot = halfSize }},
                {CursorNames.SizeNESW, new Cursor { Texture = GetTexturePath("Cursors/SizeNESW"), Size = cursorSize, HotSpot = halfSize }},
                {CursorNames.SizeNWSE, new Cursor { Texture = GetTexturePath("Cursors/SizeNWSE"), Size = cursorSize, HotSpot = halfSize }},
            };
        }
        private static Dictionary<string, ControlStyle> GetControlStyles()
        {
            var baseStyle = new ControlStyle()
            {
                Tiling = TextureMode.Grid,
                Grid = new Margin(3),
                Texture = GetTexturePath("button_hot"),
                Default =
                {
                    Texture = GetTexturePath("button_default"),
                },
                Pressed =
                {
                    Texture = GetTexturePath("button_down"),
                },
                Selected =
                {
                    Texture = GetTexturePath("button_down"),
                },
                Focused =
                {
                    Texture = GetTexturePath("button_down"),
                },
                SelectedPressed =
                {
                    Texture = GetTexturePath("button_down"),
                },
                SelectedHot =
                {
                    Texture = GetTexturePath("button_down"),
                },
            };
            
            return new Dictionary<string, ControlStyle>()
            {
                {"item", new ControlStyle(baseStyle)
                {
                    TextPadding = new Margin(8, 0, 8, 0),
                    TextAlign = Alignment.MiddleLeft,
                }},
                {"button", new ControlStyle(baseStyle)
                {
                    TextPadding = new Margin(0),
                    TextAlign = Alignment.MiddleCenter,
                }},
                {"tooltip", new ControlStyle(baseStyle)
                {
                    TextPadding = new Margin(8),
                    TextAlign = Alignment.TopLeft,
                }},
                {"textbox", new ControlStyle(baseStyle)
                {
                    Texture = GetTexturePath("input_default"),
                    TextPadding = new Margin(8),
                    Tiling = TextureMode.Grid,
                    Grid = new Margin(3),
                    Hot =
                    {
                        Texture = GetTexturePath("input_focused")
                    },
                    Focused =
                    {
                        Texture = GetTexturePath("input_focused"),
                        Tint = ColorInt.ARGB(1, 1, 0, 0),
                    },
                }},
                {"window", new ControlStyle()
                {
                    Tiling = TextureMode.Grid,
                    Grid = new Margin(9),
                    Texture = GetTexturePath("window"),
                }},
                {"frame", new ControlStyle()
                {
                    Tiling = TextureMode.Grid,
                    Grid = new Margin(4),
                    Texture = GetTexturePath("frame"),
                    TextPadding = new Margin(8),
                }},
                {"vscrollTrack", new ControlStyle()
                {
                    Tiling = TextureMode.Grid,
                    Grid = new Margin(3),
                    Texture = GetTexturePath("vscroll_track"),
                }},
                {"vscrollButton", new ControlStyle()
                {
                    Tiling = TextureMode.Grid,
                    Grid = new Margin(3),
                    Texture = GetTexturePath("vscroll_button"),
                    Hot =
                    {
                        Texture = GetTexturePath("vscroll_button_hot"),
                    },
                    Pressed =
                    {
                        Texture = GetTexturePath("vscroll_button_down"),
                    },
                }},
                {"vscrollUp", new ControlStyle()
                {
                    Default =
                    {
                        Texture = GetTexturePath("vscrollUp_default"),
                    },
                    Hot =
                    {
                        Texture = GetTexturePath("vscrollUp_hot"),
                    },
                    Pressed =
                    {
                        Texture = GetTexturePath("vscrollUp_down"),
                    },
                    Focused =
                    {
                        Texture = GetTexturePath("vscrollUp_hot"),
                    },
                }},
                {"hscrollTrack", new ControlStyle()
                {
                    Tiling = TextureMode.Grid,
                    Grid = new Margin(3),
                    Texture = GetTexturePath("hscroll_track"),
                }},
                {"hscrollButton", new ControlStyle()
                {
                    Tiling = TextureMode.Grid,
                    Grid = new Margin(3),
                    Texture = GetTexturePath("hscroll_button"),
                    Hot =
                    {
                        Texture = GetTexturePath("hscroll_button_hot"),
                    },
                    Pressed =
                    {
                        Texture = GetTexturePath("hscroll_button_down"),
                    },
                }},
                {"hscrollUp", new ControlStyle()
                {
                    Default =
                    {
                        Texture = GetTexturePath("hscrollUp_default"),
                    },
                    Hot =
                    {
                        Texture = GetTexturePath("hscrollUp_hot"),
                    },
                    Pressed =
                    {
                        Texture = GetTexturePath("hscrollUp_down"),
                    },
                    Focused =
                    {
                        Texture = GetTexturePath("hscrollUp_hot"),
                    },
                }},
                {"checkBox", new ControlStyle()
                {
                    Default =
                    {
                        Texture = GetTexturePath("checkbox_default"),
                    },
                    Hot =
                    {
                        Texture = GetTexturePath("checkbox_hot"),
                    },
                    Pressed =
                    {
                        Texture = GetTexturePath("checkbox_down"),
                    },
                    Checked =
                    {
                        Texture = GetTexturePath("checkbox_checked"),
                    },
                    CheckedFocused =
                    {
                        Texture = GetTexturePath("checkbox_checked_hot"),
                    },
                    CheckedHot =
                    {
                        Texture = GetTexturePath("checkbox_checked_hot"),
                    },
                    CheckedPressed =
                    {
                        Texture = GetTexturePath("checkbox_down"),
                    },
                }},
                {"comboLabel", new ControlStyle()
                {
                    TextPadding = new Margin(10, 0, 0, 0),
                    Tiling = TextureMode.Grid,
                    Grid = new Margin(3, 0, 0, 0),
                    Default =
                    {
                        Texture = GetTexturePath("combo_default"),
                    },
                    Hot =
                    {
                        Texture = GetTexturePath("combo_hot"),
                    },
                    Pressed =
                    {
                        Texture = GetTexturePath("combo_down"),
                    },
                    Focused =
                    {
                        Texture = GetTexturePath("combo_hot"),
                    },
                }},
                {"comboButton", new ControlStyle()
                {
                    Default =
                    {
                        Texture = GetTexturePath("combo_button_default"),
                    },
                    Hot =
                    {
                        Texture = GetTexturePath("combo_button_hot"),
                    },
                    Pressed =
                    {
                        Texture = GetTexturePath("combo_button_down"),
                    },
                    Focused =
                    {
                        Texture = GetTexturePath("combo_button_hot"),
                    },
                }},
                {"multiline", new ControlStyle()
                {
                    TextPadding = new Margin(8),
                    TextAlign = Alignment.TopLeft,
                }},
                {"label", new ControlStyle()
                {
                    TextPadding = new Margin(8, 0, 8, 0),
                    TextAlign = Alignment.MiddleLeft,
                    TextColor = ColorInt.ARGB(1, .8f, .8f, .8f),
                    BackColor = 0,
                }},
            };
        }
        
        private static string GetTexturePath(string name)
        {
            return $"UIs/Assets/{name}";
        }
    }
}
