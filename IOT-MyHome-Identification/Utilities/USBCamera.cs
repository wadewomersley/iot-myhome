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
            var capture = VideoCapture.FromCamera(-1);
            capture.Set(CaptureProperty.Fps, 5);
            var image = new Mat();
            
            while (true)
            {
                if (!this.Running)
                {
                    Thread.Sleep(100);
                    continue;
                }

                capture.Read(image);

                if (image.Empty())
                {
                    Console.WriteLine("Empty image");
                    break;
                }

                var faces = this.Classifier.DetectMultiScale(image, 1.1, 5, HaarDetectionType.ScaleImage, new Size(30, 30));

                var jpg = image.ToBytes(".jpg");
                this.ImageCaptured?.Invoke(this, new ImageCapturedEventArgs(jpg, faces.Length > 0));

                Thread.Sleep(this.CaptureInterval);
            }
        }
    }
}
