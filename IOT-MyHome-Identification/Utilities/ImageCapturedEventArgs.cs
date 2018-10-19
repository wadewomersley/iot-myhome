namespace IOT_MyHome.Identification.Utilities
{
    using System;

    class ImageCapturedEventArgs : EventArgs
    {
        public byte[] ImageJpg { get; private set; }

        public ImageCapturedEventArgs(byte[] imageJpg)
        {
            this.ImageJpg = imageJpg;
        }
    }
}
