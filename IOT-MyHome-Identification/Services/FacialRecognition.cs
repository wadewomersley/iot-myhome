namespace IOT_MyHome.Identification.Services
{
    using Amazon.Rekognition;
    using IOT_MyHome.Identification.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using IOT_MyHome.Identification.Model.JsonObjects;
    using Amazon.Rekognition.Model;
    using System.Threading.Tasks;
    using System.Linq;
    using System.IO;
    using Microsoft.Extensions.Logging;

    internal class FacialRecognition
    {
        private IAmazonRekognition Rekognition;
        private ILogger Logger;
        private Camera Camera;
        private Manager Manager;

        private byte[] LastImage = null;
        private long IgnoreUntil = 0;

        public byte[] LastImageCapturedPng
        {
            get; private set;
        }

        public FacialRecognition(Manager manager, Camera camera)
        {
            this.Logger = Logging.Logger.GetLogger<FacialRecognition>();
            this.Camera = camera;
            this.Manager = manager;

            var region = Amazon.RegionEndpoint.GetBySystemName(this.Manager.GetAmazonRegion());
            this.Rekognition = new AmazonRekognitionClient(this.Manager.GetAmazonAccessKey(), Manager.GetAmazonSecretKey(), region);
        }

        public async Task Start()
        {
            try
            {
                var collections = this.Rekognition.ListCollectionsAsync(new ListCollectionsRequest()).Result;
                if (!collections.CollectionIds.Contains(this.Manager.GetAmazonRekognitionCollection()))
                {
                    await this.Rekognition.CreateCollectionAsync(
                        new CreateCollectionRequest()
                        {
                            CollectionId = this.Manager.GetAmazonRekognitionCollection()
                        }
                    );
                }
            }
            catch (AggregateException exs)
            {
                foreach (var ex in exs.InnerExceptions)
                {
                    this.Logger.LogCritical(ex.Message);
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogCritical(ex.Message);
            }

            try
            {
                this.Camera.ImageCaptured += Camera_ImageCaptured;
                this.Camera.Start();
            }
            catch (Exception ex)
            {
                this.Logger.LogCritical(ex.Message);
            }
        }

        private void Camera_ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            if ((new DateTimeOffset(DateTime.UtcNow)).ToUnixTimeSeconds() < IgnoreUntil)
            {
                this.Logger.LogDebug("Insufficient time since last match passed, ignoring");
                return;
            }

            var bitmap = e.ImageBmp;

            this.LastImageCapturedPng = e.ImagePng;

            var newImage = ImageHandling.ResizeImage(bitmap, 32, 32);
            newImage = ImageHandling.AverageBitmapColors(newImage);

            if (LastImage == null)
            {
                LastImage = newImage;
                return;
            }

            double diff;

            try
            {
                diff = ImageHandling.ImageDifference(LastImage, newImage);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return;
            }

            if (diff < 25)
            {
                this.Logger.LogDebug("No changes detected, ignoring. {0}", diff);
                LastImage = newImage;
                return;
            }

            using (var ms = new MemoryStream(e.ImagePng))
            {
                LastImage = newImage;

                var searchRequest = new SearchFacesByImageRequest();
                searchRequest.CollectionId = this.Manager.GetAmazonRekognitionCollection();
                searchRequest.Image = new Image() { Bytes = ms };
                searchRequest.FaceMatchThreshold = this.Manager.GetRequiredSimilary();
                SearchFacesByImageResponse result;

                try
                {
                    result = this.Rekognition.SearchFacesByImageAsync(searchRequest).Result;
                }
                catch (AggregateException ex)
                {
                    if (ex.InnerException is InvalidParameterException)
                    {
                        return;
                    }

                    throw;
                }

                if (result.FaceMatches.Count > 0)
                {
                    result.FaceMatches.ForEach((match) =>
                    {
                        var matchingPerson = this.Manager.GetPerson(match.Face.FaceId);

                        if (matchingPerson == null)
                        {
                            this.Logger.LogDebug("Seen person with no local information: {0}", match.Face.FaceId);

                            this.AddDefaultPerson(match.Face, e.ImagePng);

                            return;
                        }

                        this.Logger.LogDebug("Seen {0}", matchingPerson.Name);

                        //Investigate the mood
                        var r2 = new DetectFacesRequest();
                        r2.Attributes = new List<string>() { "ALL" };
                        r2.Image = new Image() { Bytes = ms };
                        var r2r = this.Rekognition.DetectFacesAsync(r2).Result;
                    });
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    var recognise = new IndexFacesRequest();
                    recognise.CollectionId = this.Manager.GetAmazonRekognitionCollection();
                    recognise.Image = new Image() { Bytes = ms };
                    var indexResult = this.Rekognition.IndexFacesAsync(recognise).Result;

                    indexResult.FaceRecords.ForEach((faceRecord) =>
                    {
                        this.Logger.LogDebug("Recognised new person with remote ID {0}", faceRecord.Face.FaceId);

                        this.AddDefaultPerson(faceRecord.Face, e.ImagePng);
                    });
                }

                IgnoreUntil = (new DateTimeOffset(DateTime.UtcNow)).ToUnixTimeSeconds() + (this.Manager.GetSleepAfterMatchInterval() / 1000);
            }
        }

        private void AddDefaultPerson(Face face, byte[] pngImage)
        {

            using (var outMs = new MemoryStream())
            {
                using (var ms = new MemoryStream(pngImage))
                {
                    var imageObject = FreeImageAPI.FreeImageBitmap.FromStream(ms);
                    var left = (int)(face.BoundingBox.Left * imageObject.Width);
                    var top = (int)(face.BoundingBox.Top * imageObject.Height);
                    var width = (int)(face.BoundingBox.Width * imageObject.Width);
                    var height = (int)(face.BoundingBox.Height * imageObject.Height);

                    var right = Math.Max(0, imageObject.Width - width - left);
                    var bottom = Math.Max(0, imageObject.Height - height - top);
                    left = Math.Max(0, left);
                    top = Math.Max(0, top);

                    var faceImage = imageObject.Copy(left, top, right, bottom);
                    faceImage.Save(outMs, FreeImageAPI.FREE_IMAGE_FORMAT.FIF_PNG);
                }
                outMs.Seek(0, SeekOrigin.Begin);

                var buffer = new byte[outMs.Length];
                outMs.Read(buffer, 0, (int)outMs.Length);

                this.Manager.AddPerson(new Person()
                {
                    Name = "(unknown)",
                    SpokenName = "someone",
                    RemoteIDs = new List<string>() { face.FaceId },
                    Image = buffer
                });
            }
        }
    }
}
