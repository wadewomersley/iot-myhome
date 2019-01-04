namespace IOT_MyHome.Identification.Utilities
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using IOT_MyHome.Logging;
    using MMALSharp;
    using MMALSharp.Native;
    using MMALSharp.Handlers;
    using System.IO;
    using System.Collections.Generic;
    using MMALSharp.Components;

    internal class PiCamera : ICamera, IDisposable
    {
        public event EventHandler<ImageCapturedEventArgs> ImageCaptured;

        public int CaptureInterval { get; set; } = 100;
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
            Source = new CancellationTokenSource();
            this.Capture.Start();
        }

        public void Stop()
        {
            Source.Cancel();
            this.Running = false;
        }

        private async void TakeShot()
        {
            Logger.GetLogger<PiCamera>().LogDebug("Starting camera");

            if(File.Exists("/dev/null"))
            {
                MMALLog.LogLocation = "/dev/null";
            }

            var camera = MMALCamera.Instance;
            
            Logger.GetLogger<PiCamera>().LogDebug("Setting up CaptureHandler");

            try
            {
                using (var imgCaptureHandler = new CaptureHandler(ImageCaptured))
                {
                    var tl = new Timelapse { Mode = TimelapseMode.Millisecond, CancellationToken = Source.Token, Value = this.CaptureInterval };
                    Logger.GetLogger<PiCamera>().LogDebug("Starting TakePictureTimelapse with an interval of {0}ms", this.CaptureInterval);
                    await camera.TakePictureTimelapse(imgCaptureHandler, MMALEncoding.JPEG, MMALEncoding.I420, tl);
                }
            }
            catch (Exception ex)
            {
                Logger.GetLogger<PiCamera>().LogCritical("Critical failure. {0}: {1}", ex.GetType(), ex.Message);
            }

            if (this.Running)
            {
                Logger.GetLogger<PiCamera>().LogDebug("Restarting camera");
                this.Capture.Start();
            }
            else
            {
                Logger.GetLogger<PiCamera>().LogDebug("Stopping camera");

            }
        }

        public void Dispose()
        {
            MMALCamera.Instance.Cleanup();
        }
    }

    class CaptureHandler : ImageStreamCaptureHandler
    {
        private EventHandler<ImageCapturedEventArgs> ImageCaptured;

        private MemoryStream Stream;

        public CaptureHandler(EventHandler<ImageCapturedEventArgs> imageCaptured)
            : base("/tmp", "jpg")
        {
            ImageCaptured = imageCaptured;
            this.Stream = new MemoryStream();
        }

        public override void Process(byte[] data)
        {
            this.Processed += data.Length;

            if (this.Stream.CanWrite)
            {
                this.Stream.Write(data, 0, data.Length);
            }
            else
            {
                throw new IOException("Stream not writable.");
            }
        }

        public override void NewFile()
        {
            var buffer = new byte[this.Stream.Length];
            this.Stream.Seek(0, SeekOrigin.Begin);
            this.Stream.Read(buffer, 0, (int)this.Stream.Length);

            this.Stream?.Dispose();
            this.Stream = new MemoryStream(1048576);

            if (buffer.Length > 0)
            {
                Logger.GetLogger<PiCamera>().LogDebug("Triggering ImageCapturedEventArgs");
                
                this.ImageCaptured?.Invoke(this, new ImageCapturedEventArgs(buffer));
            }
        }

        public new void Dispose()
        {
            base.Dispose();
            Stream?.Dispose();
        }

        public override void PostProcess()
        {
        }
    }
}
