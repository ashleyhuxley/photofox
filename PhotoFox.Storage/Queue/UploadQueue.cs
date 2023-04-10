using Azure.Storage.Queues;
using PhotoFox.Storage.Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace PhotoFox.Storage.Queue
{
    public class UploadQueue : IUploadQueue
    {
        private const string QueueName = "uploads";

        private readonly IStorageConfig config;

        public UploadQueue(IStorageConfig config)
        {
            this.config = config;
        }

        public async Task QueueUploadMessage(UploadMessage message)
        {
            var client = new QueueClient(this.config.StorageConnectionString, QueueName, new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            });

            await client.SendMessageAsync(JsonSerializer.Serialize(message));
        }
    }
}
