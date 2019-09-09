using System;
using System.IO;
using System.Text;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace LargeCsvToExcelDotNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            string connStr = "DefaultEndpointsProtocol=https;AccountName=shelltrailstorage;AccountKey=pVNrlVP9mDf8Hu9+ZnzVMcfucqEIkUycYl7U1KyTJY/4ctbQLMOWs6OrFwdMaaWyq3z1AsdU3IwKf5T7t0SP0w==;EndpointSuffix=core.windows.net";

            CloudStorageAccount account = CloudStorageAccount.Parse(connStr);

            CloudBlobClient client = account.CreateCloudBlobClient();

            CloudBlobContainer container = client.GetContainerReference("source");

            MemoryStream ms = new MemoryStream();

            MemoryStream mswrt = new MemoryStream();

            CloudBlockBlob file = container.GetBlockBlobReference("test.csv");

            file.DownloadToStreamAsync(ms);

            CloudBlobContainer containerwrt = client.GetContainerReference("destination");

            containerwrt.CreateIfNotExistsAsync();

            CloudBlockBlob filewrt = containerwrt.GetBlockBlobReference("test2.xls");

            filewrt.DownloadToStreamAsync(mswrt);

            Stream blobStream = file.OpenReadAsync().Result;

            string text;

            using (StreamReader reader = new StreamReader(blobStream))
            {

                text = reader.ReadToEnd();

                Console.WriteLine(text);

                var options = new BlobRequestOptions()
                {
                    ServerTimeout = TimeSpan.FromMinutes(10)
                };

                using (var stream = new MemoryStream(Encoding.Default.GetBytes(text), false))
                {
                    filewrt.UploadFromStream(stream, null, options);
                }

            }
        }
    }
}
