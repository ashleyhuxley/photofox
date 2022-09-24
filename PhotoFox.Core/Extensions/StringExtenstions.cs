namespace PhotoFox.Core.Extensions
{
    public static class StringExtenstions
    {
        public static string ToHashPartitionKey(this string hash)
        {
            return hash.Substring(0, 2);
        }
    }
}
