using System.Drawing;

namespace PhotoFox.Core.Imaging
{
    public interface IThumbnailProvider
    {
        Image GenerateThumbnail(Image input, int width);
    }
}
