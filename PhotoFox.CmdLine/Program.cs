using Azure.Data.Tables;
using Azure.Storage.Blobs;
using PhotoFox.Storage.Models;
using System;
using System.Reflection.Metadata;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var cs = "DefaultEndpointsProtocol=https;AccountName=photofox;AccountKey=38hd9gnf9MLavn6EilD8dv0k7rwrFW1dD7TBHMgQdcU/9GkWEvI4piW0EqmCNuOmNsrohPK+kPCI+ASt/tuFPw==;EndpointSuffix=core.windows.net";

            var client = new BlobServiceClient(cs);
            var container = client.GetBlobContainerClient("images");
            var blobs = container.GetBlobs();

            var tclient = new TableServiceClient(cs);
            var tableClient = tclient.GetTableClient("PhotoMetadata");


            foreach (var blob in blobs )
            {
                var res = tableClient.Query<PhotoMetadata>(p => p.RowKey == blob.Name);
                if (!res.Any() )
                {
                    Console.WriteLine(blob.Name);
                }
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}