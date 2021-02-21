namespace IOT_MyHome.Identification.Utilities
{
    using OpenCvSharp;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class USBCamera : ICamera
    {
        public event EventHandler<ImageCapturedEventArgs> ImageCaptured;

        public int CaptureInterval { get; set; } = 300;
        private Task Capture;
        private CascadeClassifier Classifier;

        public bool Running { get; private set; } = false;

        public USBCamera(int captureInterval)
        {
            this.CaptureInterval = captureInterval;

            this.Capture = new Task(TakeShot, TaskCreationOptions.LongRunning);
            this.Capture.Start();


            this.Classifier = new CascadeClassifier("haarcascade_frontalface_alt2.xml");
        }

        public void Start()
        {
            this.Running = true;
        }

        public void Stop()
        {
            this.Running = false;
        }

        private void TakeShot()
        {
            var capture = VideoCapture.FromCamera(0);
            capture.Set(VideoCaptureProperties.FrameWidth, 1280);
            capture.Set(VideoCaptureProperties.FrameHeight, 720);
            capture.Set(VideoCaptureProperties.Fps, 5);
            var image = new Mat();

            var nextAllowed = DateTime.UtcNow;
            nextAllowed.Subtract(TimeSpan.FromSeconds(500));
            var captureInterval = TimeSpan.FromSeconds(this.CaptureInterval);


            while (true)
            {
                if (!this.Running)
                {
                    Thread.Sleep(100);
                    continue;
                }

                capture.Read(image);

                //@TODO: Sleep needs to capture.Read but then ignore until CaptureInterval reached due to buffer

                if (image.Empty())
                {
                    Console.WriteLine("Empty image");
                    break;
                }

                if (DateTime.UtcNow < nextAllowed)
                {
                    Thread.Sleep(100);
                    continue;
                }

                
                var faces = this.Classifier.DetectMultiScale(image, 1.1, 5, HaarDetectionTypes.ScaleImage, new Size(30, 30));

                var png = image.ToBytes(".png");        
                this.ImageCaptured?.Invoke(this, new ImageCapturedEventArgs(png, faces.Length > 0));

                if (faces.Length > 0)
                {
                    nextAllowed = DateTime.UtcNow;
                    nextAllowed.Add(captureInterval);
                }
            }
        }
    }
}
