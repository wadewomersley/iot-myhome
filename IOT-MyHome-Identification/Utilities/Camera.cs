namespace IOT_MyHome.Identification.Utilities
{
    using OpenCvSharp;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    class Camera
    {
        public event EventHandler<ImageCapturedEventArgs> ImageCaptured;

        public int CaptureInterval { get; set; } = 1000;
        private Task Capture;
        private ManualResetEvent Trigger = new ManualResetEvent(false);
        public bool Running { get; private set; } = false;

        public Camera(int captureInterval)
        {
            this.CaptureInterval = captureInterval;

            this.Capture = new Task(TakeShot, TaskCreationOptions.LongRunning);
            this.Capture.Start();
        }

        public void Start()
        {
            this.Running = true;
            this.Trigger.Set();
        }

        public void Stop()
        {
            this.Running = false;
            this.Trigger.Reset();
        }

        private void TakeShot()
        {
            var capture = new VideoCapture(0);
            capture.Set(CaptureProperty.FrameWidth, 1280);
            capture.Set(CaptureProperty.FrameHeight, 720);
            var image = new Mat();

            while (true)
            {
                this.Trigger.WaitOne();

                capture.Read(image);

                this.ImageCaptured?.Invoke(this, new ImageCapturedEventArgs(image.ToBytes(".bmp"), image.ToBytes(".png")));

                Thread.Sleep(this.CaptureInterval);
            }
        }
    }
}
