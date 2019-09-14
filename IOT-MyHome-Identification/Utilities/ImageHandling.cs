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
        public static byte[] CropImage(byte[] image, float top, float left, float width, float height)
        {
            try
            {
                using (var memStream = new MemoryStream(image))
                {
                    using (var img = new FreeImageBitmap(memStream))
                    {
                        int realLeft = (int)(img.Width * left);
                        int realTop = (int)(img.Height * top);
                        int realWidth = (int)(img.Width * width);
                        int realHeight = (int)(img.Height * height);

                        using (var cropped = img.Copy((int)realLeft, (int)realTop, img.Width - (int)realLeft - (int)realWidth, img.Height - (int)realTop - (int)realHeight))
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
