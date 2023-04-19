using ExifLibrary;

namespace PhotoFox.Extensions
{
    public static class ExifURationalExtensions
    {
        public static double GetValue(this ExifURational input)
        {
            return (double)input.Value.Numerator / input.Value.Denominator;
        }
    }
}
