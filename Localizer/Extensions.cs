using Localizer.DataModel;
using System;
using System.Globalization;
using System.Reflection;
using Terraria.ModLoader;

namespace Localizer
{
    public static class Extensions
    {
        public static void Import(this ModTranslation modTranslation, BaseEntry entry, CultureInfo culture)
        {
            if (entry != null)
            {
                if (entry.Origin != modTranslation.GetDefault()
                    && modTranslation.GetDefault() != modTranslation.Key
                    && !string.IsNullOrWhiteSpace(modTranslation.GetDefault())
                    && !string.IsNullOrWhiteSpace(entry.Origin))
                {
                    Localizer.Log.Warn($"Mismatch origin text when importing \"{modTranslation.Key}\", Origin in mod: {modTranslation.GetDefault()}, Origin in package: {entry.Origin}");
                }

                if (modTranslation.GetDefault() != null && entry.Translation != null && entry.Translation != modTranslation.Key)
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

        public static object GetFieldDirectly(this Type type, object obj, string fieldName)
        {
            return type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).GetValue(obj);
        }


        public static T GetFieldDirectly<T>(this Type type, object obj, string fieldName) where T : class
        {
            return type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).GetValue(obj) as T;
        }

        public static MethodInfo FindMethod(this Module module, string findableID)
        {
            try
            {
                var typeName = findableID.Split(' ')[1].Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0];
                return module.GetType(typeName)?.FindMethod(findableID);
            }
            catch (Exception e)
            {
                Localizer.Log.Debug(e);
                return null;
            }
        }
    }
}
