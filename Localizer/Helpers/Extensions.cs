using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Localizer.Attributes;
using Localizer.DataModel.Default;
using Mono.Cecil;
using MonoMod.Utils;
using Terraria.ModLoader;

namespace Localizer
{
    public static class Extensions
    {
        #region ModTranslation

        public static void Import(this ModTranslation modTranslation, BaseEntry entry, CultureInfo culture)
        {
            if (entry != null)
            {
                if (entry.Origin != modTranslation.GetDefault()
                    && modTranslation.GetDefault() != modTranslation.Key
                    && !string.IsNullOrWhiteSpace(modTranslation.GetDefault())
                    && !string.IsNullOrWhiteSpace(entry.Origin))
                {
                    Utils.LogWarn(
                        $"Mismatch origin text when importing \"{modTranslation.Key}\", Origin in mod: {modTranslation.GetDefault()}, Origin in package: {entry.Origin}");
                }

                if (modTranslation.GetDefault() != null && entry.Translation != null &&
                    entry.Translation != modTranslation.Key)
                {
                    modTranslation.AddTranslation(Localizer.CultureInfoToGameCulture(culture), entry.Translation);
                }
            }
        }

        public static string GetTranslation(this ModTranslation modTranslation, CultureInfo culture)
        {
            var translation = modTranslation.GetTranslation(Localizer.CultureInfoToGameCulture(culture));
            return string.IsNullOrWhiteSpace(translation) ? "" : translation;
        }

        public static string DefaultOrEmpty(this ModTranslation modTranslation)
        {
            var d = modTranslation.GetDefault();
            return string.IsNullOrWhiteSpace(d) || d == modTranslation.Key ? "" : d;
        }

        #endregion

        #region Reflection
        public static PropertyInfo[] ModTranslationOwnerField(this Type type)
        {
            return type.GetProperties().Where(p => p.GetCustomAttribute<ModTranslationOwnerFieldAttribute>() != null)
                       .ToArray();
        }

        public static string ModTranslationOwnerFieldName(this PropertyInfo prop)
        {
            return prop.GetCustomAttribute<ModTranslationOwnerFieldAttribute>()?.FieldName;
        }

        public static PropertyInfo[] ModTranslationProp(this Type type)
        {
            return type.GetProperties().Where(p => p.GetCustomAttribute<ModTranslationPropAttribute>() != null)
                       .ToArray();
        }

        #endregion
    }
}
