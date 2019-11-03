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
            
            On.Terraria.Item.ctor += (orig, self) =>
            {
                orig(self);
                items.Add(new WeakReference<Item>(self));
            };
        }

        public void Refresh()
        {
            ModContent.RefreshModLanguage(LanguageManager.Instance.ActiveCulture);

            var deads = new List<WeakReference<Item>>();
            foreach (var wr in items)
            {
                if (wr.TryGetTarget(out var i))
                {
                    i.RebuildTooltip();
                }
                else
                {
                    deads.Add(wr);
                }
            }
            deads.ForEach(i => items.Remove(i));
        }
        
        public void Dispose()
        {
        }
    }
}
