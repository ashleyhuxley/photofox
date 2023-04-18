namespace PhotoFox.Model
{
    public class CameraSettings
    {
        public CameraSettings(
            string iso, 
            string aperture, 
            string focalLength, 
            string device, 
            string manufacturer,
            string exposure)
        {
            this.ISO= iso;
            this.Aperture= aperture;
            this.FocalLength = focalLength;
            this.Device = device;
            this.Manufacturer = manufacturer;
            this.Exposure = exposure;
        }

        public string ISO { get; }
        public string Aperture { get; }
        public string FocalLength { get; }
        public string Device { get; }
        public string Manufacturer { get; }
        public string Exposure { get; }
    }
}
