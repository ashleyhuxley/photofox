using System;

namespace PhotoFox.Core.Extensions
{
    public static class IntegerExtensions
    {
        public static string ToBatchId(this int value)
        {
            return value.ToString("D12");
        }
    }
}
