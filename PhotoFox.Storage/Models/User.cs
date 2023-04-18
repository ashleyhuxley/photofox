using Azure;
using Azure.Data.Tables;
using System;

namespace PhotoFox.Storage.Models
{
    public class User : ITableEntity
    {
        public User() 
        {
            this.PartitionKey = string.Empty;
            this.RowKey = string.Empty;
            this.UserName= string.Empty;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string UserName { get; set; }
    }
}
