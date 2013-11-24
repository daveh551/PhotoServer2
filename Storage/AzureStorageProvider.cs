using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace PhotoServer.Storage
{
    public class AzureStorageProvider : IStorageProvider
    {
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _rootContainer;
        public AzureStorageProvider(string azureConnectionString, string container)
        {
            _storageAccount = CloudStorageAccount.Parse(azureConnectionString);
            _blobClient = _storageAccount.CreateCloudBlobClient();
            _rootContainer = _blobClient.GetContainerReference(container);
            try
            {

                _rootContainer.CreateIfNotExists();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw;
            }

        }
        bool IStorageProvider.FileExists(string path)
        {

            return _rootContainer.GetBlockBlobReference(path).Exists();
        }

        System.IO.Stream IStorageProvider.GetStream(string path)
        {
            var memoryStream = new MemoryStream();
            var blobRef = _rootContainer.GetBlockBlobReference(path);
            if (blobRef == null) return null;
            blobRef.DownloadToStream(memoryStream);
            return memoryStream;
        }


        public void WriteFile(string path, byte[] imageArray)
        {
            var blobRef = _rootContainer.GetBlockBlobReference(path);

            if (blobRef.Exists()) throw new IOException(string.Format("Blob {0} already exists in Containter {1}", path, _rootContainer.Name));
            using (var memory = new MemoryStream(imageArray))
                blobRef.UploadFromStream(memory);
        }


        public void DeleteFile(string path)
        {
            var blob = _rootContainer.GetBlockBlobReference(path);
            blob.DeleteIfExists();
        }

        public IEnumerable<string> GetFiles(string directoryPath)
        {
            var returnList = new List<string>();
            var directoryBlob = _rootContainer.GetDirectoryReference(directoryPath);
            directoryBlob.ListBlobs(true).ToList().ForEach(b => returnList.Add(directoryBlob.Uri.MakeRelativeUri(b.Uri).ToString()));
            return returnList;

        }
    }
}