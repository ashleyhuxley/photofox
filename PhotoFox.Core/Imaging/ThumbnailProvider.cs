﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Versioning;

namespace PhotoFox.Core.Imaging
{
    [SupportedOSPlatform("windows")]
    public class ThumbnailProvider : IThumbnailProvider
    {
        public Image GenerateThumbnail(Image original, int width)
        {
            return GenerateThumbnail(original, width, 0);
        }

        public Image GenerateThumbnail(Image input, int width, int rotationDegrees)
        {
            var factor = (double)input.Width / width;
            var newHeight = input.Height / factor;

            return ResizeImage(input, width, Convert.ToInt32(newHeight), rotationDegrees);
        }

        private static void Swap(ref int a, ref int b)
        {
            a = a + b;     
            b = a - b;     
            a = a - b;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        [SupportedOSPlatform("windows")]
        private static Bitmap ResizeImage(Image image, int width, int height, int rotateDegrees)
        {
            var srcRect = new Rectangle(0, 0, image.Width, image.Height);
            var destRect = new Rectangle(0, 0, width, height);

            Bitmap destImage;
            if (rotateDegrees == 90 || rotateDegrees == 270)
            {
                Swap(ref width, ref height);
            }

            destImage = new Bitmap(width, height);

            destImage.SetResolution(image.VerticalResolution, image.HorizontalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);

                    switch (rotateDegrees)
                    {
                        case 90:
                            graphics.DrawImage(image, new[] { new Point(destImage.Width, 0), new Point(destImage.Width, destImage.Height), new Point(0, 0) }, srcRect, GraphicsUnit.Pixel, wrapMode);
                            break;
                        case 180:
                            graphics.DrawImage(image, new[] { new Point(destImage.Width, destImage.Height), new Point(0, destImage.Height), new Point(destImage.Width, 0) }, srcRect, GraphicsUnit.Pixel, wrapMode);
                            break;
                        case 270:
                            graphics.DrawImage(image, new[] { new Point(0, destImage.Height), new Point(0, 0), new Point(destImage.Width, destImage.Height) }, srcRect, GraphicsUnit.Pixel, wrapMode);
                            break;
                        default:
                            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                            break;
                    }
                }
            }

            return destImage;
        }
    }
}
