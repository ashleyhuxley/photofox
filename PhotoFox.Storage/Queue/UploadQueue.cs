using Azure.Storage.Queues;
using System;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Queue
{
    public class UploadQueue
    {
        private const string QueueName = "uploads";

        private readonly IStorageConfig config;

        public UploadQueue(IStorageConfig config)
        {
            this.config = config;
        }

        public async Task QueueUploadMessage(string photoId, string albumId, string title)
        {
            if (string.IsNullOrEmpty(photoId))
            {
                throw new ArgumentNullException(nameof(photoId));
            }

            if (string.IsNullOrEmpty(albumId))
            {
                throw new ArgumentNullException(nameof(albumId));
            }

            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            string message = $"{photoId},{albumId},{title}";

            var client = new QueueClient(this.config.StorageConnectionString, QueueName);

            await client.SendMessageAsync(message);
        }
    }
}
