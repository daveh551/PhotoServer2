using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace PhotoServer.Storage
{
    public class FileStorageProvider : IStorageProvider
    {
        private string _root;
        public FileStorageProvider(string root)
        {
            _root = root;
        }
        public bool FileExists(string path)
        {
            return File.Exists(Path.Combine(_root, path));
        }

        public System.IO.Stream GetStream(string path)
        {
            try
            {
                var returnStream = new FileStream(Path.Combine(_root, path), FileMode.Open);
                return returnStream;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }


        public void WriteFile(string path, byte[] imageArray)
        {
            string physicalPath = GetPhysicalPath(path);
            if (File.Exists(physicalPath)) throw new IOException(string.Format("File {0} already exists in tree {1}", path, _root));
            string dir = Path.GetDirectoryName(physicalPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using (var fileStream = new FileStream(physicalPath, FileMode.CreateNew, FileAccess.Write))
                fileStream.Write(imageArray, 0, imageArray.Length);
        }

        private string GetPhysicalPath(string path)
        {
            return Path.Combine(_root, path);
        }


        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public IEnumerable<string> GetFiles(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
                return Directory.GetFiles(directoryPath);
            return new List<string>();
        }
    }
}