namespace IOT_MyHome.Identification.Utilities
{
    using System;

    class ImageCapturedEventArgs : EventArgs
    {
        public byte[] ImagePng { get; private set; }

        public bool HasFaces { get; private set; }

        public ImageCapturedEventArgs(byte[] imagePng, bool hasFaces = false)
        {
            this.ImagePng = imagePng;
            this.HasFaces = hasFaces;
        }
    }
}
