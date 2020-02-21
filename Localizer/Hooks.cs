using Microsoft.Xna.Framework;

namespace Localizer
{
    public class Hooks
    {
        public delegate void BeforeModCtorHandler(object mod);
        public static event BeforeModCtorHandler BeforeModCtor;
        internal static void InvokeBeforeModCtor(object mod)
        {
            BeforeModCtor?.Invoke(mod);
        }

        public delegate void BeforeLoadHandler();
        public static event BeforeLoadHandler BeforeLoad;
        internal static void InvokeBeforeLoad()
        {
            BeforeLoad?.Invoke();
        }

        public delegate void BeforeSetupContentHandler();
        public static event BeforeSetupContentHandler BeforeSetupContent;
        internal static void InvokeBeforeSetupContent()
        {
            BeforeSetupContent?.Invoke();
        }

        public delegate void PostSetupContentHandler();
        public static event PostSetupContentHandler PostSetupContent;
        internal static void InvokePostSetupContent()
        {
            PostSetupContent?.Invoke();
        }

        public delegate void GameUpdateHandler(GameTime gameTime);
        public static event GameUpdateHandler GameUpdate;
        internal static void InvokeOnGameUpdate(GameTime gameTime)
        {
            GameUpdate?.Invoke(gameTime);
        }

        public delegate void PostDrawHandler(GameTime gameTime);
        public static event PostDrawHandler PostDraw;
        internal static void InvokeOnPostDraw(GameTime gameTime)
        {
            PostDraw?.Invoke(gameTime);
        }
    }
}
