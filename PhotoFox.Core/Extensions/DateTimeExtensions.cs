using System;

namespace PhotoFox.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToPartitionKey(this DateTime value)
        {
            if (value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Must be a UTC date in order to be a partition key");
            }

            return value.ToString("yyyyMMdd");
        }

        public static string ToDupeKeyPartitionKey(this DateTime key)
        {
            return key.ToString("yyyyMMddHHmmssfff");
        }
    }
}
