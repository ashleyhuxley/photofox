namespace PhotoFox.Model
{
    public class Geolocation
    {
        public Geolocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; }
        public double Longitude { get; }
    }
}
