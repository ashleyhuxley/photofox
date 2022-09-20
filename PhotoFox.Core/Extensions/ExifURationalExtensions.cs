using ExifLibrary;

namespace PhotoFox.Core.Extensions
{
    public static class ExifURationalExtensions
    {
        public static double GetValue(this ExifURational input)
        {
            return (double)input.Value.Numerator / input.Value.Denominator;
        }
    }
}
