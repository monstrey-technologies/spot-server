using System;
using System.IO;
using System.Threading.Tasks;
using Bosdyn.Api;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SpotServer.infrastructure;

namespace SpotServer.services
{
    public class SpotImageService: ImageService.ImageServiceBase
    {
        public override Task<GetImageResponse> GetImage(GetImageRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GetImageResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError {Code = CommonError.Types.Code.Ok}),
                ImageResponses = { new ImageResponse
                    {
                        Status = ImageResponse.Types.Status.Ok,
                        Shot = new ImageCapture()
                        {
                            AcquisitionTime = Timestamp.FromDateTime(DateTime.UtcNow),
                            Image = new Image()
                            {
                                Format = Image.Types.Format.Jpeg,
                                Cols = 640,
                                Rows = 480,
                                Data = ByteString.CopyFrom(File.ReadAllBytes(@"assets/camera.jpg"))
                            }
                        },
                        Source = new ImageSource()
                        {
                            ImageType = ImageSource.Types.ImageType.Visual,
                            Cols = 640,
                            Rows = 480,
                            DepthScale = 1000
                        }
                    }
                }
            });
        }

        public override Task<ListImageSourcesResponse> ListImageSources(ListImageSourcesRequest request, ServerCallContext context)
        {
            return Task.FromResult(new ListImageSourcesResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError {Code = CommonError.Types.Code.Ok}),
                ImageSources = { new ImageSource
                    {
                        ImageType = ImageSource.Types.ImageType.Visual,
                        Cols = 640,
                        Rows = 480,
                        DepthScale = 1000
                    }
                }
            });
        }
    }
}