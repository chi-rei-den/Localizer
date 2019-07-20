using MonoMod.Utils;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace Localizer
{
    public static class Utils
    {
        public static void SerializeJsonAndCreateFile(object obj, string path, bool indent = true)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(JsonConvert.SerializeObject(obj, indent ? Formatting.Indented : Formatting.None));
                }
            }
        }

        public static T ReadFileAndDeserializeJson<T>(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (var sr = new StreamReader(fs))
                {
                    return JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
                }
            }
        }

        public static object ReadFileAndDeserializeJson(Type t, string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (var sr = new StreamReader(fs))
                {
                    return JsonConvert.DeserializeObject(sr.ReadToEnd(), t);
                }
            }
        }

        public static MethodBase GetMethodBase<T>(string findableName)
        {
            var method = typeof(T).FindMethod(findableName);
            return method != null ? MethodBase.GetMethodFromHandle(method.MethodHandle) : null;
        }

        public static string DateTimeEscapeForPath(DateTime dt)
        {
            return dt.ToString().Replace(' ', '-').Replace('/', '-').Replace(':', '-');
        }
    }
}