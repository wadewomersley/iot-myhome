namespace IOT_MyHome.Identification.Utilities
{
    using System;

    class ImageCapturedEventArgs : EventArgs
    {
        public byte[] ImageBmp { get; private set; }
        public byte[] ImagePng { get; private set; }

        public ImageCapturedEventArgs(byte[] imageBmp, byte[] imagePng)
        {
            this.ImageBmp = imageBmp;
            this.ImagePng = imagePng;
        }
    }
}
