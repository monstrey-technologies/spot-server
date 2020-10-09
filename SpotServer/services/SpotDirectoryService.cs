using System;
using System.Linq;
using System.Threading.Tasks;
using Bosdyn.Api;
using Google.Protobuf.Collections;
using Grpc.Core;
using SpotServer.infrastructure;

namespace SpotServer.services
{
    class SpotDirectoryService : DirectoryService.DirectoryServiceBase
    {
        public override Task<ListServiceEntriesResponse> ListServiceEntries(ListServiceEntriesRequest request, ServerCallContext context)
        {
            return Task.FromResult(
                new ListServiceEntriesResponse
                {
                    Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
                    ServiceEntries =
                    {
                        new RepeatedField<ServiceEntry>
                        {
                            new ServiceEntry
                            {
                                Authority = "api.spot.robot",
                                Name = "lease",
                                UserTokenRequired = true,
                                Type = "bosdyn.api.LeaseService"
                            },
                            new ServiceEntry
                            {
                                Authority = "api.spot.robot",
                                Name = "power",
                                UserTokenRequired = true,
                                Type = "bosdyn.api.PowerService"
                            },
                            new ServiceEntry
                            {
                                Authority = "api.spot.robot",
                                Name = "robot-command",
                                UserTokenRequired = true,
                                Type = "bosdyn.api.RobotCommandService"
                            },
                            new ServiceEntry
                            {
                                Authority = "api.spot.robot",
                                Name = "robot-state",
                                UserTokenRequired = true,
                                Type = "bosdyn.api.RobotStateService"
                            },
                            new ServiceEntry
                            {
                                Authority = "api.spot.robot",
                                Name = "image",
                                UserTokenRequired = true,
                                Type = "bosdyn.api.ImageService"
                            },
                            new ServiceEntry
                            {
                                Authority = "auth.spot.robot",
                                Name = "auth",
                                UserTokenRequired = false,
                                Type = "bosdyn.api.AuthService"
                            },
                            new ServiceEntry
                            {
                                Authority = "id.spot.robot",
                                Name = "robot-id",
                                UserTokenRequired = true,
                                Type = "bosdyn.api.RobotIdService"
                            },
                            new ServiceEntry
                            {
                                Authority = "api.spot.robot",
                                Name = "directory-registration",
                                UserTokenRequired = true,
                                Type = "bosdyn.api.DirectoryService"
                            },
                            new ServiceEntry
                            {
                                Authority = "estop.spot.robot",
                                Name = "estop",
                                UserTokenRequired = true,
                                Type = "bosdyn.api.EstopService"
                            },
                            new ServiceEntry
                            {
                                Authority = "timesync.spot.robot",
                                Name = "time-sync",
                                UserTokenRequired = true,
                                Type = "bosdyn.api.TimeSyncService"
                            }
                        }
                    }
                }
            );
        }
    }
}