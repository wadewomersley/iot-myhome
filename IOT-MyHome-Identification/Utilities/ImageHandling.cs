namespace IOT_MyHome.Identification.Utilities
{
    using FreeImageAPI;
    using IOT_MyHome.Logging;
    using Microsoft.Extensions.Logging;
    using System;
    using System.IO;

    internal class ImageHandling
    {
        /// <summary>
        /// Calculate % difference between two images.
        /// </summary>
        /// <param name="imageA"></param>
        /// <param name="imageB"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown if images are not the same length</exception>
        public static double ImageDifference(byte[] imageA, byte[] imageB)
        {
            if (imageA.Length != imageB.Length)
            {
                throw new ArgumentException("Images are not the same length");
            }

            double different = 0;

            for (var i = 0; i < imageA.Length; i = i + 3)
            {
                if (imageA[i] != imageB[i] || imageA[i + 1] != imageB[i + 1] || imageA[i + 2] != imageB[i + 2])
                {
                    different += 3;
                }
            }

            return different / (double)imageA.Length * 100;
        }

        /// <summary>
        /// Modify pixels to be 100% or 0% of each color based on the average color of the image.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static byte[] AverageBitmapColors(byte[] image)
        {
            var pixels = image.Length / 3;

            var redSum = 0;
            var greenSum = 0;
            var blueSum = 0;

            for (var i = 0; i < pixels; i++)
            {
                redSum += image[i * 3];
                greenSum += image[i * 3 + 1];
                blueSum += image[i * 3 + 2];
            }

            var redAverage = redSum / pixels;
            var greenAverage = greenSum / pixels;
            var blueAverage = blueSum / pixels;

            for (var i = 0; i < pixels; i++)
            {
                image[i * 3] = image[i * 3] > (byte)redAverage ? (byte)255 : (byte)0;
                image[i * 3 + 1] = image[i * 3 + 1] > (byte)greenAverage ? (byte)255 : (byte)0;
                image[i * 3 + 2] = image[i * 3 + 2] > (byte)blueAverage ? (byte)255 : (byte)0;
            }

            return image;
        }

        /// <summary>
        /// Save a bitmap to a file as a raw bitmap.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="data"></param>
        public static void SaveBitmap(string filename, byte[] data)
        {
            using (var memStream = new MemoryStream(data))
            {
                using (var img = new FreeImageBitmap(memStream))
                {
                    img.Save(filename);
                }
            }
        }

        /// <summary>
        /// Resizes an image to the specified w/h
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static byte[] ResizeImage(byte[] image, uint width, uint height, FREE_IMAGE_FORMAT format = FREE_IMAGE_FORMAT.FIF_BMP)
        {
            try
            {
                using (var memStream = new MemoryStream(image))
                {
                    using (var img = new FreeImageBitmap(memStream))
                    {
                        img.Rescale((int)width, (int)height, FreeImageAPI.FREE_IMAGE_FILTER.FILTER_BILINEAR);
                        using (var outStream = new MemoryStream())
                        {
                            img.Save(outStream, format);
                            return outStream.ToArray();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Crop an image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="top"></param>
        /// <param name="left"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static byte[] CropImage(byte[] image, uint top, uint left, uint width, uint height)
        {
            try
            {
                using (var memStream = new MemoryStream(image))
                {
                    using (var img = new FreeImageBitmap(memStream))
                    {
                        using (var cropped = img.Copy((int)left, (int)top, img.Width - (int)left - (int)width, img.Height - (int)top - (int)height))
                        {
                            using (var outStream = new MemoryStream())
                            {
                                cropped.Save(outStream, FreeImageAPI.FREE_IMAGE_FORMAT.FIF_JPEG);
                                return outStream.ToArray();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
