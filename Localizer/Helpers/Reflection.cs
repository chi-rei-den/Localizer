using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MonoMod.Utils;

namespace System.Reflection
{
    public static class NoroHelper
    {
        public const BindingFlags AnyVisibility = BindingFlags.Public | BindingFlags.NonPublic;
        public const BindingFlags Any = AnyVisibility | BindingFlags.Static | BindingFlags.Instance;

        public static MethodInfo MethodInfo(Expression<Action> expression) => (expression.Body as MethodCallExpression)?.Method;

        public static Type Type(this string name)
        {
            //return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.DefinedTypes).FirstOrDefault(t => t.FullName == name);
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.DefinedTypes.Any(t => t.FullName == name))?.ManifestModule.GetType(name);
        }

        public static FieldInfo Field(this Type type, string name, BindingFlags flags = Any) => type?.GetField(name, flags) ?? type?.BaseType?.Field(name, flags);

        public static PropertyInfo Property(this Type type, string name, BindingFlags flags = Any) => type?.GetProperty(name, flags) ?? type?.BaseType?.Property(name, flags);

        public static MethodInfo Method(this Type t, string name, IEnumerable<Type> args) => t.Method(name, args.ToArray());

        public static MethodInfo Method(this Type t, string name, params Type[] args)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (args == null || args.Length == 0)
            {
                return t.GetMethod(name, Any);
            }

            return t.GetMethod(name, Any, null, args, new ParameterModifier[] { });
        }

        public static object ValueOf(this Type type, string name, object value = null)
        {
            var field = type?.Field(name);
            if (field != null)
            {
                return field.GetValue(value);
            }

            var prop = type?.Property(name);
            if (prop != null)
            {
                return prop.GetValue(value);
            }

            throw new MemberAccessException($"{name} not found ({type})");
        }

        public static object ValueOf(this object obj, string name) => obj?.GetType().ValueOf(name, obj);

        public static T ValueOf<T>(this Type type, string name) => (T)type?.ValueOf(name);

        public static T ValueOf<T>(this object obj, string name) => (T)obj?.ValueOf(name);

        public static void SetField(this Type type, string name, object value) => type?.Field(name).SetValue(null, value);

        public static void SetField(this object obj, string name, object value) => obj?.GetType().Field(name).SetValue(obj, value);

        public static object Invoke(this object o, string name, params object[] args)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var method = o.GetType().Method(name, args.Select(a => a.GetType()))
                         ?? throw new MissingMethodException($"{o.GetType().FullName}.{name}");
            return method.Invoke(o, args);
        }
    }
}
