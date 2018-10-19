namespace IOT_MyHome.Identification.Utilities
{
    using System;

    internal interface ICamera
    {
        int CaptureInterval { get; set; }
        bool Running { get; }

        event EventHandler<ImageCapturedEventArgs> ImageCaptured;

        void Start();
        void Stop();
    }
}