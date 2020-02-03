using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ModPatch
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var FILE_PATH = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Documents\My Games\Terraria\ModLoader\Mods\Localizer.tmod");
            var tModLoaderVersion = "";
            var modName = "";
            var modVersion = "";
            var files = new List<(string fileName, int length, int compressedLength, byte[] content)>();

            using (var fileStream = File.OpenRead(FILE_PATH))
            {
                using (var br = new BinaryReader(fileStream))
                {
                    br.ReadBytes(4); // TMOD
                    tModLoaderVersion = br.ReadString(); // tModLoader version
                    br.ReadBytes(280); // Signature
                    modName = br.ReadString(); // Name
                    modVersion = br.ReadString(); // Version
                    var fileCount = br.ReadInt32();
                    for (var i = 0; i < fileCount; i++)
                    {
                        var entry = (filename: br.ReadString(), length: br.ReadInt32(),
                                     compressedLength: br.ReadInt32(),
                                     content: new byte[] { });
                        files.Add(entry);
                    }

                    for (var i = 0; i < fileCount; i++)
                    {
                        var file = files[i];
                        var content = br.ReadBytes(file.compressedLength);
                        files[i] = (file.fileName, file.length, file.compressedLength, content);
                    }
                }
            }

            if (!modName.StartsWith("!"))
            {
                modName = "!" + modName;
            }

            files = files.Select(f => (f.fileName.EndsWith("NA.dll") && !f.fileName.StartsWith("!"))
                                     ? ("!" + f.fileName, f.length, f.compressedLength, f.content)
                                     : f).ToList();

            using (var fileStream = new FileStream(FILE_PATH, FileMode.Create, FileAccess.ReadWrite))
            {
                using (var bw = new BinaryWriter(fileStream))
                {
                    bw.Write(Encoding.UTF8.GetBytes("TMOD"));
                    bw.Write(tModLoaderVersion);
                    var hashPos = bw.BaseStream.Position;
                    bw.Seek(280, SeekOrigin.Current);
                    var contentPos = bw.BaseStream.Position;
                    bw.Write(modName);
                    bw.Write(modVersion);
                    bw.Write(files.Count);
                    foreach (var file in files)
                    {
                        bw.Write(file.fileName);
                        bw.Write(file.length);
                        bw.Write(file.compressedLength);
                    }
                    foreach (var file in files)
                    {
                        bw.Write(file.content);
                    }
                    bw.Seek((int)contentPos, SeekOrigin.Begin);
                    var hash = SHA1.Create().ComputeHash(bw.BaseStream);
                    bw.Seek((int)hashPos, SeekOrigin.Begin);
                    bw.Write(hash);
                }
            }

            var enabledFilePath =
                Environment.ExpandEnvironmentVariables(
                    @"%USERPROFILE%\Documents\My Games\Terraria\ModLoader\Mods\enabled.json");

            if (!File.Exists(enabledFilePath))
            {
                return;
            }

            File.WriteAllText(enabledFilePath, File.ReadAllText(enabledFilePath).Replace("\"Localizer\"", "\"!Localizer\""));
        }
    }
}
