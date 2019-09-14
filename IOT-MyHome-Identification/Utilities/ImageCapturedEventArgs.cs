namespace IOT_MyHome.Identification.Utilities
{
    using System;

    class ImageCapturedEventArgs : EventArgs
    {
        public byte[] ImageJpg { get; private set; }

        public bool HasFaces { get; private set; }

        public ImageCapturedEventArgs(byte[] imageJpg, bool hasFaces = false)
        {
            this.ImageJpg = imageJpg;
            this.HasFaces = hasFaces;
        }
    }
}
