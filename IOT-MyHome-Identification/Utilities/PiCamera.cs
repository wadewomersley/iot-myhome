namespace IOT_MyHome.Identification.Utilities
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using IOT_MyHome.Logging;
    using MMALSharp;
    using Nito.AsyncEx;
    using MMALSharp.Native;
    using MMALSharp.Handlers;

    internal class PiCamera : ICamera, IDisposable
    {
        public event EventHandler<ImageCapturedEventArgs> ImageCaptured;

        public int CaptureInterval { get; set; } = 10;
        private Task Capture;
        private ManualResetEvent Trigger = new ManualResetEvent(false);
        private CancellationTokenSource Source;
        public bool Running { get; private set; } = false;

        public PiCamera(int captureInterval)
        {
            this.CaptureInterval = captureInterval;

            this.Capture = new Task(TakeShot, TaskCreationOptions.LongRunning);
        }

        public void Start()
        {
            this.Running = true;
            Source = new CancellationTokenSource(TimeSpan.FromDays(3650));
            this.Capture.Start();
        }

        public void Stop()
        {
            Source.Cancel();
            this.Running = false;
        }

        private void TakeShot()
        {
            Logger.GetLogger<PiCamera>().LogDebug("Starting camera");

            AsyncContext.Run(async () =>
            {
                using (var imgCaptureHandler = new CaptureHandler(ImageCaptured))
                {
                    var tl = new Timelapse { Mode = TimelapseMode.Millisecond, CancellationToken = Source.Token, Value = this.CaptureInterval };
                    await MMALCamera.Instance.TakePictureTimelapse(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420, tl);
                }
            });

            Logger.GetLogger<PiCamera>().LogDebug("Stopping camera");
        }

        public void Dispose()
        { 
            MMALCamera.Instance.Cleanup();
        }
    }

    class CaptureHandler : ImageStreamCaptureHandler
    {
        private EventHandler<ImageCapturedEventArgs> ImageCaptured;

        public byte[] LastImage { get; private set; }

        public CaptureHandler(EventHandler<ImageCapturedEventArgs> imageCaptured)
            : base("", "jpg")
        {
            ImageCaptured = imageCaptured;
        }

        public override void Process(byte[] data)
        {
            LastImage = data;
            this.ImageCaptured?.Invoke(this, new ImageCapturedEventArgs(data));
        }
    }
}
