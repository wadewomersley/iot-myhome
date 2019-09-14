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
    using FreeImageAPI;

    internal class FacialRecognition
    {
        private IAmazonRekognition Rekognition;
        private ILogger Logger;
        private ICamera Camera;
        private Manager Manager;

        private float IgnoreUntil = 0;

        public byte[] LastImageCapturedJpg
        {
            get; private set;
        }

        public FacialRecognition(Manager manager, ICamera camera)
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
                    this.Logger.LogCritical(ex.Message + ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogCritical(ex.Message + ex.StackTrace);
            }

            try
            {
                this.Camera.ImageCaptured += Camera_ImageCaptured;
                this.Camera.Start();
            }
            catch (Exception ex)
            {
                this.Logger.LogCritical(ex.Message + ex.StackTrace);
            }
        }

        private void Camera_ImageCaptured(object sender, ImageCapturedEventArgs e)
        {
            lock (Manager)
            {
                if ((new DateTimeOffset(DateTime.UtcNow)).ToUnixTimeMilliseconds() < IgnoreUntil)
                {
                    this.Logger.LogDebug("Insufficient time since last match passed, ignoring");
                    return;
                }

                IgnoreUntil = (new DateTimeOffset(DateTime.UtcNow)).ToUnixTimeMilliseconds() + this.Manager.GetCaptureInterval();
            }

            try
            {
                this.LastImageCapturedJpg = ImageHandling.ResizeImage(e.ImageJpg, 640, 480, FreeImageAPI.FREE_IMAGE_FORMAT.FIF_JPEG);

                if (!e.HasFaces)
                {
                    return;
                }

                using (var ms = new MemoryStream(e.ImageJpg))
                {
                    this.Logger.LogDebug("Sending image to Rekognition");

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

                                var bb = match.Face.BoundingBox;
                                this.Logger.LogDebug("Cropping image using {0} {1} {2} {3}", bb.Left, bb.Top, bb.Width, bb.Height);

                                this.Manager.AddPerson(new Person()
                                {
                                    Name = "(unknown)",
                                    SpokenName = "someone",
                                    RemoteIDs = new List<string>() { match.Face.FaceId },
                                    Image = ImageHandling.CropImage(e.ImageJpg, bb.Left, bb.Top, bb.Width, bb.Height)
                                });

                                return;
                            }

                            this.Logger.LogDebug("Seen {0}", matchingPerson.Name);

                            //Investigate the mood
                            var r2 = new DetectFacesRequest();
                            r2.Attributes = new List<string>() { "ALL" };
                            r2.Image = new Image() { Bytes = ms };
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

                            var bb = faceRecord.Face.BoundingBox;
                            this.Manager.AddPerson(new Person()
                            {
                                Name = "(unknown)",
                                SpokenName = "someone",
                                RemoteIDs = new List<string>() { faceRecord.Face.FaceId },
                                Image = ImageHandling.CropImage(e.ImageJpg, bb.Left, bb.Top, bb.Width, bb.Height)
                            });
                        });
                    }
                }
            }
            catch (Exception)
            {
                return;
            }

            IgnoreUntil = (new DateTimeOffset(DateTime.UtcNow)).ToUnixTimeMilliseconds() + this.Manager.GetSleepAfterMatchInterval();
        }
    }
}
