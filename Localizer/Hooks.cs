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
        
        public delegate void PostSetupContentHandler();
        public static event PostSetupContentHandler PostSetupContent;
        internal static void InvokePostSetupContent()
        {
            PostSetupContent?.Invoke();
        }

        public delegate void OnGameUpdateHandler(GameTime gameTime);
        public static event OnGameUpdateHandler OnGameUpdate;
        internal static void InvokeOnGameUpdate(GameTime gameTime)
        {
            OnGameUpdate?.Invoke(gameTime);
        }
    }
}
