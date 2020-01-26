using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Harmony;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Localizer.Package.Import
{
    public sealed class RefreshLanguageService : Disposable
    {
        private static RefreshLanguageService _instance;

        private List<WeakReference> items;

        private bool _rebuilding = false;
        private bool _cleaning = false;

        private HarmonyInstance _harmony;

        private int _cleanUpCounter = 0;

        private bool _firstRun = true;

        public RefreshLanguageService()
        {
            _instance = this;
            items = new List<WeakReference>();

            _harmony = HarmonyInstance.Create(nameof(RefreshLanguageService));
            _harmony.Patch(typeof(ModItem).GetConstructors()[0], null, new HarmonyMethod(NoroHelper.MethodInfo(() => OnModItemCtor(null))));
        }

        private static void OnModItemCtor(ModItem __instance)
        {
            if (!Localizer.Config.RebuildTooltips)
            {
                return;
            }

            if (Localizer.Config.RebuildTooltipsOnce && !_instance._firstRun)
            {
                return;
            }

            if (!_instance._rebuilding && !_instance._cleaning)
            {
                _instance.items.Add(new WeakReference(__instance));
                _instance._cleanUpCounter++;

                if (_instance._cleanUpCounter > 20000 && !_instance._firstRun)
                {
                    Task.Run(() =>
                    {
                        _instance.CleanUpItems();
                    });

                    _instance._cleanUpCounter = 0;
                }
            }
        }

        public void Refresh()
        {
            Utils.SafeWrap(() =>
            {
                if (Localizer.State != OperationTiming.BeforeModCtor)
                {
                    ModContent.RefreshModLanguage(LanguageManager.Instance.ActiveCulture);
                }

                if (Localizer.Config.RebuildTooltips && Localizer.State == OperationTiming.PostContentLoad)
                {
                    _rebuilding = true;
                    CleanUpItems();
                    var stopWatch = new Stopwatch();
                    Utils.LogInfo($"Rebuilding tooltips, count: {items.Count}");
                    stopWatch.Start();
                    foreach (var i in items)
                    {
                        (i.Target as ModItem)?.item.RebuildTooltip();
                    }

                    stopWatch.Stop();
                    Utils.LogInfo(
                        $"Rebuilding completed. count: {items.Count}, take {stopWatch.Elapsed.TotalSeconds} seconds");
                    _rebuilding = false;
                }
            });
        }

        private void CleanUpItems()
        {
            var deads = new List<WeakReference>();
            _cleaning = true;
            var stopWatch = new Stopwatch();
            Utils.LogInfo($"Cleaning item caches, count: {items.Count}");
            stopWatch.Start();
            lock (items)
            {
                foreach (var wr in items)
                {
                    if (!wr.IsAlive)
                    {
                        deads.Add(wr);
                    }
                }

                foreach (var d in deads)
                {
                    items.Remove(d);
                }
            }
            stopWatch.Stop();
            Utils.LogInfo($"Item caches cleaned. count: {items.Count}, take {stopWatch.Elapsed.TotalSeconds} seconds");
            if (_firstRun)
            {
                _firstRun = false;
            }

            _cleaning = false;
        }

        protected override void DisposeUnmanaged()
        {
            _instance = null;
            _harmony.UnpatchAll(nameof(RefreshLanguageService));
        }
    }
}
