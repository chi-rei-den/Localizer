using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ModPatch
{
    class Program
    {
        static void Main(string[] args)
        {
            var FILE_PATH = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\Documents\My Games\Terraria\ModLoader\Mods\Localizer.tmod");
            var tModLoaderVersion = "";
            var modName = "";
            var modVersion = "";
            var files = new List<(string fileName, int length, int compressedLength)>();
            var content = new byte[0];
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
                        var entry = (filename: br.ReadString(), length: br.ReadInt32(), compressedLength: br.ReadInt32());
                        files.Add(entry);
                    }
                    content = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
                }
            }

            modName = "!" + modName;
            files = files.Select(f => (f.fileName.EndsWith(".XNA.dll") || f.fileName.EndsWith(".FNA.dll")) ? ("!" + f.fileName, f.length, f.compressedLength) : f).ToList();

            using (var fileStream = File.OpenWrite(FILE_PATH))
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
                    bw.Write(content);
                    bw.Seek((int)contentPos, SeekOrigin.Begin);
                    var hash = SHA1.Create().ComputeHash(bw.BaseStream);
                    bw.Seek((int)hashPos, SeekOrigin.Begin);
                    bw.Write(hash);
                }
            }
        }
    }
}
