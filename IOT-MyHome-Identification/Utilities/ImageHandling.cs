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
            var logger = Logging.Logger.GetLogger<ImageHandling>();

            try
            {
                using (var memStream = new MemoryStream(image))
                {
                    using (var img = new FreeImageBitmap(memStream))
                    {
                        int realLeft = Math.Max(0, (int)(img.Width * left));
                        int realTop = Math.Max(0, (int)(img.Height * top));
                        int realWidth = Math.Max(0, (int)(img.Width * width));
                        int realHeight = Math.Max(0, (int)(img.Height * height));

                        int right = Math.Max(0, img.Width - realLeft - realWidth);
                        int bottom = Math.Max(0, img.Height - realTop - realHeight);
                        var time = DateTime.UtcNow.ToString("HHmmssffffff");

                        using (var cropped = img.Copy(new System.Drawing.Rectangle(realLeft, realTop, realWidth, realHeight)))
                        {
                            using (var outStream = new MemoryStream())
                            {
                                cropped.Save(outStream, FREE_IMAGE_FORMAT.FIF_PNG);
                                return outStream.ToArray();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Unexpected error cropping image. {0}: {1}", ex.GetType().ToString(), ex.Message);
                return null;
            }
        }
    }
}
