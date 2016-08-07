namespace MyHomework.WebApp.Basic
{
    using Configurations;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using LogLevel = Microsoft.Extensions.Logging.LogLevel;

    public class BlobStorageManager
    {
        private CloudStorageAccount storageAccount;

        public BlobStorageManager(IOptions<ConnectionStrings> options)
        {
            storageAccount = CloudStorageAccount.Parse(options.Value.AzureStorageConnectionString);
        }

        public async Task<Uri> Upload(IFormFile file, string fileName)
        {
            var container = await this.GetBlobContainer();

            string blobName = string.Format("{0}/{1}", Guid.NewGuid(), fileName);
            // Get a reference to a blob
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

            using (var fileStream = file.OpenReadStream())
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
            }

            return blockBlob.Uri;
        }

        private async Task<CloudBlobContainer> GetBlobContainer()
        {
            // Create a blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Get a reference to a container named “mycontainer.”
            CloudBlobContainer container = blobClient.GetContainerReference("myhomework");

            // If “mycontainer” doesn’t exist, create it.
            await container.CreateIfNotExistsAsync();

            await container.SetPermissionsAsync(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });

            return container;
        }
    }
}
