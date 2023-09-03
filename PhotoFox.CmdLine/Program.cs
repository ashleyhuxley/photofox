using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=photofox;AccountKey=38hd9gnf9MLavn6EilD8dv0k7rwrFW1dD7TBHMgQdcU/9GkWEvI4piW0EqmCNuOmNsrohPK+kPCI+ASt/tuFPw==;EndpointSuffix=core.windows.net";

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("images");

            foreach (BlobItem blobItem in containerClient.GetBlobs())
            {
                BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);

                BlobProperties blobProperties = blobClient.GetProperties();
                if (blobProperties.ContentType != "image/jpeg" &&  blobProperties.ContentType != "image/png")
                {
                    Console.WriteLine(blobItem.Name);
                }
            }
        }
    }

    public enum ImageFormat
    {
        Unknown,
        JPG,
        PNG
    }

    public class ImageFormatDetector
    {
        public static ImageFormat Detect(byte[] bytes)
        {
            // Check for PNG signature
            if (bytes.Length >= 8 &&
                bytes[0] == 137 && bytes[1] == 80 && bytes[2] == 78 && bytes[3] == 71 &&
                bytes[4] == 13 && bytes[5] == 10 && bytes[6] == 26 && bytes[7] == 10)
            {
                return ImageFormat.PNG;
            }

            // Check for JPG signature
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xD8)
            {
                return ImageFormat.JPG;
            }

            return ImageFormat.Unknown;
        }
    }
}