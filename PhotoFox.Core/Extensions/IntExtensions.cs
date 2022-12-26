using System;

namespace PhotoFox.Core.Extensions
{
    public static class IntExtensions
    {
        public static int ToRotationDegrees(this int? exifOrientation)
        {
            if (!exifOrientation.HasValue)
            {
                return 0;
            }

            switch (exifOrientation)
            {
                case 1: return 0;
                case 2: throw new NotImplementedException();
                case 3: return 180;
                case 4: throw new NotImplementedException();
                case 5: throw new NotImplementedException();
                case 6: return 90;
                case 7: throw new NotImplementedException();
                case 8: return 270;
                default: throw new ArgumentOutOfRangeException(nameof(exifOrientation));
            }
        }
    }
}
