using System;

namespace PhotoFox.Model
{
    public interface IDisplayableItem
    {
        double? GeolocationLongitude { get; }

        double? GeolocationLatitude { get; }

        DateTime DateTaken { get; }

        long? FileSize { get; }
    }
}
