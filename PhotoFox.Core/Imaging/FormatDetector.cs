namespace PhotoFox.Core.Imaging
{
    public static class FormatDetector
    {
        public static ImageType GetFormat(byte[] bytes)
        {
            // Check for PNG signature
            if (bytes.Length >= 8 &&
                bytes[0] == 137 && bytes[1] == 80 && bytes[2] == 78 && bytes[3] == 71 &&
                bytes[4] == 13 && bytes[5] == 10 && bytes[6] == 26 && bytes[7] == 10)
            {
                return ImageType.Png;
            }

            // Check for JPG signature
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xD8)
            {
                return ImageType.Jpeg;
            }

            return ImageType.Unknown;
        }
    }
}
