using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Localizer.Attributes;
using Localizer.DataModel;
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
                    Localizer.Log.Warn(
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

        public static object GetFieldDirectly(this Type type, object obj, string fieldName)
        {
            return type.GetField(fieldName,
                                 BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                 BindingFlags.Static).GetValue(obj);
        }


        public static T GetFieldDirectly<T>(this Type type, object obj, string fieldName) where T : class
        {
            return GetFieldDirectly(type, obj, fieldName) as T;
        }

        public static object GetPropDirectly(this Type type, object obj, string propName)
        {
            return type.GetProperty(
                           propName,
                           BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                       .GetValue(obj);
        }


        public static T GetPropDirectly<T>(this Type type, object obj, string propName) where T : class
        {
            return GetPropDirectly(type, obj, propName) as T;
        }

        public static MethodInfo FindMethod(this Module module, string findableID)
        {
            try
            {
                var typeName = findableID.Split(' ')[1].Split(new[] {"::"}, StringSplitOptions.RemoveEmptyEntries)[0];
                return module.GetType(typeName)?.FindMethod(findableID);
            }
            catch (Exception e)
            {
                Localizer.Log.Debug(e);
                return null;
            }
        }

        public static MethodDefinition FindMethod(this ModuleDefinition module, string findableID)
        {
            try
            {
                var typeName = findableID.Split(' ')[1].Split(new[] {"::"}, StringSplitOptions.RemoveEmptyEntries)[0];
                return module.GetType(typeName)?.FindMethod(findableID);
            }
            catch (Exception e)
            {
                Localizer.Log.Debug(e);
                return null;
            }
        }

        public static PropertyInfo[] GetTModLocalizeFieldPropInfos(this Type type)
        {
            return type.GetProperties().Where(p => p.GetCustomAttribute<TModLocalizeFieldAttribute>() != null)
                       .ToArray();
        }

        public static string GetTModLocalizeFieldName(this PropertyInfo prop)
        {
            return (prop.GetCustomAttribute(typeof(TModLocalizeFieldAttribute)) as TModLocalizeFieldAttribute)
                ?.FieldName;
        }

        public static PropertyInfo[] GetTModLocalizePropPropInfos(this Type type)
        {
            return type.GetProperties().Where(p => p.GetCustomAttribute<TModLocalizeTextPropAttribute>() != null)
                       .ToArray();
        }

        #endregion
    }
}
