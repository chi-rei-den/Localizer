using System.IO;
using Localizer.DataModel;

namespace Localizer.Services.File
{
    public interface IFileLoadService : IService
    {
        /// <summary>
        ///     Load a file from stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        IFile Load(Stream stream, string typeName);
    }
}
