using Azure;
using Azure.Data.Tables;
using System;
using System.ComponentModel;

namespace PhotoFox.Storage.Models
{
    public class LogEntry : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string LogLevel { get; set; }
        public string Source { get; set; }
        public string PhotoId { get; set; }
        public string AlbumId { get; set; }
        public string PhotoHash { get; set; }
        public string Message { get; set; }
    }

    public static class LogLevel
    {
        public static string Warn = "WARN";
        public static string Error = "ERROR";
        public static string Info = "INFO";
    }
}
