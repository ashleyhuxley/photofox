using System;

namespace PhotoFox.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToPartitionKey(this DateTime value)
        {
            return value.ToString("yyyyMMdd");
        }
    }
}
