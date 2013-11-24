using System.Collections.Generic;
using System.IO;

namespace PhotoServer.Storage
{
    public interface IStorageProvider
    {
        bool FileExists(string path);
        Stream GetStream(string path);
        void WriteFile(string path, byte[] imageArray);
        void DeleteFile(string path);
        IEnumerable<string> GetFiles(string directoryPath);
    }
}
