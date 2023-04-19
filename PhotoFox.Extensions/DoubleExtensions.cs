using System;

namespace PhotoFox.Extensions
{
    public static class DoubleExtensions
    {
        public static DateTime ToDateTime(this double value)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(value);
            return dateTime;
        }

        public static DateTime ToDateTime(this long value)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(value);
            return dateTime;
        }
    }
}
