namespace PhotoFox.Storage.Models
{
    public class HashSearchResult
    {
        private HashSearchResult()
        {
            this.HashExists = false;
        }

        public HashSearchResult(string partitionKey, string rowKey)
        {
            this.HashExists = true;
            this.PhotoPartitionKey= partitionKey;
            this.PhotoRowKey= rowKey;
        }

        public static HashSearchResult NotFoundResult => new HashSearchResult();

        public bool HashExists { get; }

        public string? PhotoPartitionKey { get; }

        public string? PhotoRowKey { get; }
    }
}
