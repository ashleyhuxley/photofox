namespace PhotoFox.Core.Extensions
{
    public static class LongExtensions
    {
        public static string ToFileSize(this long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };

            int order = 0;
            while (bytes >= 1024 && order < sizes.Length - 1)
            {
                order++;
                bytes = bytes / 1024;
            }

            return string.Format("{0:0.##} {1}", bytes, sizes[order]);
        }
    }
}
