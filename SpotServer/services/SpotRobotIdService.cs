using System;
using System.Threading.Tasks;
using Bosdyn.Api;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SpotServer.infrastructure;

namespace SpotServer.services
{
    class SpotRobotIdService : RobotIdService.RobotIdServiceBase
    {
        public override Task<RobotIdResponse> GetRobotId(RobotIdRequest request, ServerCallContext context)
        {
            return Task.FromResult(
                new RobotIdResponse{
                    Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
                    RobotId = new RobotId
                    {
                        Nickname = "monstrey-technologies", 
                        Species = "spot", 
                        Version = "2.0.2", 
                        SerialNumber = "virtual-spot-001",  
                        ComputerSerialNumber = "virtual-001",
                        SoftwareRelease = new RobotSoftwareRelease
                        {
                            Changeset = "initial", 
                            Name = "name",
                            ApiVersion = "2.0.2",
                            Type = "Some Type",
                            BuildInformation = "Build information",
                            ChangesetDate = Timestamp.FromDateTime(DateTime.Parse("2020-10-05").ToUniversalTime()),
                            InstallDate = Timestamp.FromDateTime(DateTime.Parse("2020-10-05").ToUniversalTime()),
                            Version = new SoftwareVersion
                            {
                                MajorVersion = 0,
                                MinorVersion = 0,
                                PatchLevel = 1
                            }
                        }
                    }
                });
        }
    }
}