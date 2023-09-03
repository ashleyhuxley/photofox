﻿using Azure;
using Azure.Data.Tables;
using System;

namespace PhotoFox.Storage.Models
{
    public class Folder : ITableEntity
    {
        public Folder()
        {
            this.PartitionKey = string.Empty;
            this.RowKey = string.Empty;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
