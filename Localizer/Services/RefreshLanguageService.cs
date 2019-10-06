using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Localizer.Services
{
    public sealed class RefreshLanguageService : IService
    {
        private List<WeakReference<Item>> items;
        
        public RefreshLanguageService()
        {
            items = new List<WeakReference<Item>>();
            
            On.Terraria.Item.SetDefaults += (orig, self, type, check) =>
            {
                items.Add(new WeakReference<Item>(self));
                orig(self, type, check);
            };
        }

        public void Refresh()
        {
            ModContent.RefreshModLanguage(LanguageManager.Instance.ActiveCulture);

            foreach (var wr in items)
            {
                if (wr.TryGetTarget(out var i))
                {
                    i.RebuildTooltip();
                }
            }
        }
        
        public void Dispose()
        {
        }
    }
}
