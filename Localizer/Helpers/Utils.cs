using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Harmony.ILCopying;
using Localizer.Attributes;
using Localizer.DataModel;
using Localizer.DataModel.Default;
using Localizer.Helpers;
using Mono.Cecil;
using MonoMod.Utils;
using Newtonsoft.Json;
using Terraria.ModLoader;
using File = System.IO.File;

// ReSharper disable once CheckNamespace
namespace Localizer
{
    public static partial class Utils
    {
        #region Zip

        /// <summary>
        ///     Add a file into a ZipArchive as an entry.
        /// </summary>
        /// <param name="archive">Target archive.</param>
        /// <param name="filePath">The path of the file.</param>
        /// <param name="entryName">Entry name in the archive.</param>
        public static void WriteZipArchiveEntry(ZipArchive archive, string filePath, string entryName)
        {
            var fileEntry = archive.CreateEntry(entryName);
            using (var stream = fileEntry.Open())
            {
                var fileBytes = File.ReadAllBytes(filePath);
                stream.Write(fileBytes, 0, fileBytes.Length);
            }
        }

        #endregion

        #region Reflection

        /// <summary>
        ///     Return the method matched with the findableName in the given type.
        ///     Return null if fail.
        /// </summary>
        /// <param name="findableName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static MethodBase GetMethodBase<T>(string findableName)
        {
            var method = typeof(T).FindMethod(findableName);
            return method != null ? MethodBase.GetMethodFromHandle(method.MethodHandle) : null;
        }

        #endregion

        #region Json

        /// <summary>
        ///     Serialize an object and write to disk.
        /// </summary>
        /// <param name="obj">Object want to serialize.</param>
        /// <param name="path">Store path of the serialized file.</param>
        /// <param name="indent"></param>
        public static void SerializeJsonAndCreateFile(object obj, string path, bool indent = true)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.Write(JsonConvert.SerializeObject(obj, indent ? Formatting.Indented : Formatting.None));
                }
            }
        }

        /// <summary>
        ///     Read a file return the deserialized object.
        /// </summary>
        /// <param name="path">Path of the file.</param>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <returns></returns>
        public static T ReadFileAndDeserializeJson<T>(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return ReadFileAndDeserializeJson<T>(fs);
            }
        }

        /// <summary>
        ///     Read a stream return the deserialized object.
        /// </summary>
        /// <param name="stream">Stream want to deserialize.</param>
        /// <typeparam name="T">The type of result.</typeparam>
        /// <returns></returns>
        public static T ReadFileAndDeserializeJson<T>(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                return JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
            }
        }

        /// <summary>
        ///     Read a file return the deserialized object.
        /// </summary>
        /// <param name="t">The type of result.</param>
        /// <param name="path">Path of the file.</param>
        /// <returns></returns>
        public static object ReadFileAndDeserializeJson(Type t, string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return ReadFileAndDeserializeJson(t, fs);
            }
        }

        /// <summary>
        ///     Read a file return the deserialized object.
        /// </summary>
        /// <param name="t">The type of result.</param>
        /// <param name="stream">Stream want to deserialize.</param>
        /// <returns></returns>
        public static object ReadFileAndDeserializeJson(Type t, Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                return JsonConvert.DeserializeObject(sr.ReadToEnd(), t);
            }
        }

        #endregion

        #region GameCulture

        #endregion

        #region Log

        public static void LogFatal(object o)
        {
            Localizer.Log.Fatal(o);
        }
        
        public static void LogError(object o)
        {
            if(Localizer.Config.LogLevel >= LogLevel.Error)
                Localizer.Log.Error(o);
        }

        public static void LogWarn(object o)
        {
            if(Localizer.Config.LogLevel >= LogLevel.Warn)
                Localizer.Log.Warn(o);
        }

        public static void LogInfo(object o)
        {
            if(Localizer.Config.LogLevel >= LogLevel.Info)
                Localizer.Log.Info(o);
        }

        public static void LogDebug(object o)
        {
            if(Localizer.Config.LogLevel >= LogLevel.Debug)
                Localizer.Log.Debug(o);
        }
        #endregion

        #region Network

        public static HttpWebResponse GET(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json;charset=UTF-8";
            request.Accept = "application/vnd.github.v3+json";
            request.UserAgent = "Localizer";
            request.Timeout = 9000;

            return (HttpWebResponse)request.GetResponse();
        }

        public static string GetResponseBody(HttpWebResponse response)
        {
            if (response == null)
                return null;
            
            Encoding encoding = Encoding.UTF8;
            
            using (var myResponseStream = response.GetResponseStream())
            {
                using (var myStreamReader = new StreamReader(myResponseStream, encoding))
                {
                    return myStreamReader.ReadToEnd();
                }
            }
        }

        #endregion

        #region Others

        /// <summary>
        ///     Create mappings from the name of actual ModTranslation container in the mod to the property
        ///     in the translation file.
        /// </summary>
        /// <param name="entryType"></param>
        /// <returns></returns>
        public static Dictionary<string, PropertyInfo> CreateEntryMappings(Type entryType)
        {
            if (entryType == null)
            {
                throw new ArgumentNullException();
            }

            var mappings = new Dictionary<string, PropertyInfo>();

            foreach (var prop in entryType.ModTranslationProp())
            {
                var attr =
                    prop.GetCustomAttribute(typeof(ModTranslationPropAttribute)) as ModTranslationPropAttribute;

                mappings.Add(attr.PropName, prop);
            }

            return mappings;
        }

        /// <summary>
        ///     Get the translation of the property of the entry.
        ///     eg: GetTranslation(*an ItemEntry*, *Name property*)
        ///     which returns ItemEntry.Name.Translation.
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static string GetTranslationOfEntry(IEntry entry, PropertyInfo prop)
        {
            return (prop.GetValue(entry) as BaseEntry)?.Translation;
        }

        /// <summary>
        ///     Create directory if doesn't exist.
        /// </summary>
        /// <param name="path"></param>
        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static ICollection<IMod> GetLoadedMods()
        {
            return ModLoader.Mods?.Select(m => Localizer.GetWrappedMod(m.Name)).ToArray();
        }

        public static Mod GetModByName(string name)
        {
            return ModLoader.Mods.FirstOrDefault(m => m.Name == name);
        }

        public static string DateTimeToFileName(DateTime dateTime)
        {
            return string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", dateTime);
        }

        public static string EscapePath(string path)
        {
            return path.Trim(Path.GetInvalidPathChars());
        }

        public static List<ILInstruction> GetInstructions(MethodBase method)
        {
            var dummy = new DynamicMethod("Dummy", typeof(void), new Type[] { });
            if (method.GetMethodBody() is null)
            {
                return null;
            }
            return MethodBodyReader.GetInstructions(dummy.GetILGenerator(), method);
        }

        public static void SafeWrap(Action action)
        {
            SafeWrap(action, out var ex);
        }

        public static void SafeWrap(Action action, out Exception ex)
        {
            try
            {
                ex = null;
                action();
            }
            catch (Exception e)
            {
                ex = e;
                LogError(e);
            }
        }
        
        public static T SafeWrap<T>(Func<T> func)
        {
            return SafeWrap(func, out var ex);
        }
        
        public static T SafeWrap<T>(Func<T> func, out Exception ex)
        {
            try
            {
                ex = null;
                return func();
            }
            catch (Exception e)
            {
                ex = e;
                LogError(e);
            }
            
            return default;
        }
        #endregion
    }
}
