using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using southosting.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using ImageMagick;

namespace southosting.Logic
{
    public class FileHelpers
    {
        public static bool IsImage(IFormFile file)
        {
            if (file.ContentType.Contains("image")) return true;

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };
            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        public static async Task<bool> UploadFileToStorage(IFormFile file, string fileName, BlobStorage _config)
        {
            StorageCredentials storageCredentials = new StorageCredentials(_config.AccountName, _config.AccountKey);
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_config.ImageContainer);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

            CloudBlobContainer thumbnailContainer = blobClient.GetContainerReference(_config.ThumbnailContainer);
            CloudBlockBlob thumbnailBlockBlob = container.GetBlockBlobReference(fileName);

            using (var fileStream = file.OpenReadStream())
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
            }

            return await Task.FromResult(true);
        }

        public static async Task<bool> DeleteFileAsync(string fileName, BlobStorage _config)
        {
            StorageCredentials storageCredentials = new StorageCredentials(_config.AccountName, _config.AccountKey);
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_config.ImageContainer);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
            await blockBlob.DeleteIfExistsAsync();

            CloudBlobContainer thumbnailContainer = blobClient.GetContainerReference(_config.ThumbnailContainer);
            CloudBlockBlob thumbnailBlockBlob = container.GetBlockBlobReference(fileName);
            await thumbnailBlockBlob.DeleteIfExistsAsync();

            return await Task.FromResult(true);
        }

        public static async Task<bool> UploadThumbnailToFileStorage(Stream imageStream, CloudBlockBlob blockBlob, int maxWidth = 0, int maxHeight = 0)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                using (MagickImage image = new MagickImage(imageStream))
                {
                    image.Resize(maxWidth, maxHeight);
                    image.Write(outStream);
                }

                await blockBlob.UploadFromStreamAsync(outStream);
            }
            return await Task.FromResult(true);
        }
    }
}