using System;
using System.Reflection;
using Mono.Cecil;
using MonoMod.Utils;
using Terraria;

namespace Localizer.Helpers
{
    public static class ReflectionHelper
    {
        public const BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
                                        BindingFlags.Instance;
        
        public static object Field(this object o, string name, BindingFlags flags = All)
        {
            if(o is null)
                throw new ArgumentNullException(nameof(o));
            var field = o.GetType().GetField(name, flags) ??
                        throw new Exception($"Cannot find field: {o.GetType().FullName}.{name}");
            return field.GetValue(o);
        }
        
        public static object Field(this Type t, string name, BindingFlags flags = All)
        {
            if(t is null)
                throw new ArgumentNullException(nameof(t));
            var field = t.GetField(name, flags) ??
                        throw new Exception($"Cannot find field: {t.FullName}.{name}");
            return field.GetValue(null);
        }
        
        public static object Prop(this object o, string name, BindingFlags flags = All)
        {
            if(o is null)
                throw new ArgumentNullException(nameof(o));
            var prop = o.GetType().GetProperty(name, flags) ??
                        throw new Exception($"Cannot find property: {o.GetType().FullName}.{name}");
            return prop.GetValue(o);
        }
        
        public static object Prop(this Type t, string name, BindingFlags flags = All)
        {
            if(t is null)
                throw new ArgumentNullException(nameof(t));
            var prop = t.GetProperty(name, flags) ??
                       throw new Exception($"Cannot find property: {t.FullName}.{name}");
            return prop.GetValue(null);
        }

        public static object Method(this object o, string name, object arg, BindingFlags flags = All)
        {
            if(o is null)
                throw new ArgumentNullException(nameof(o));
            return o.Method(name, new[] {arg}, flags);
        }
        
        public static object Method(this object o, string name, object[] args = null, BindingFlags flags = All)
        {
            if(o is null)
                throw new ArgumentNullException(nameof(o));
            var method = o.GetType().GetMethod(name, flags) ??
                         throw new Exception($"Cannot find method: {o.GetType().FullName}.{name}");
            return method.Invoke(o, args);
        }

        public static object Method(this Type t, string name, object[] args = null, BindingFlags flags = All)
        {
            if(t is null)
                throw new ArgumentNullException(nameof(t));
            var method = t.GetMethod(name, flags) ??
                         throw new Exception($"Cannot find method: {t.FullName}.{name}");
            return method.Invoke(null, args);
        }
        
        public static object Method(this Type t, string name, object arg, BindingFlags flags = All)
        {
            if(t is null)
                throw new ArgumentNullException(nameof(t));
            return t.Method(name, new[] {arg}, flags);
        }

        public static Module Tr()
        {
            return Assembly.GetAssembly(typeof(Main)).ManifestModule;
        }

        public static MethodBase FindMethod(this Module module, string findableID)
        {
            try
            {
                var typeName = findableID.Split(' ')[1].Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0];
                return module.GetType(typeName)?.FindMethodAndConstructor(findableID);
            }
            catch (Exception e)
            {
                Localizer.Log.Debug(e);
                return null;
            }
        }
        
        public static MethodBase FindMethodAndConstructor(this Type type, string findableID)
        {
            try
            {
                var m = type.FindMethod(findableID);
                if (m is null)
                {
                    foreach (var c in type.GetConstructors(ReflectionHelper.All))
                    {
                        if (c.GetID() == findableID)
                            return c;
                    }
                }

                return m;
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
                var typeName = findableID.Split(' ')[1].Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[0];
                return module.GetType(typeName)?.FindMethod(findableID);
            }
            catch (Exception e)
            {
                Localizer.Log.Debug(e);
                return null;
            }
        }

        public static T GetAttribute<T>(Type type) where T : Attribute
        {
            return type.GetCustomAttribute<T>();
        }
    }
}
