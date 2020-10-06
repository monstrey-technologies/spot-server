using System;
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
                                Authority = "auth.spot.robot",
                                Name = "auth",
                                UserTokenRequired = false
                            },
                            new ServiceEntry
                            {
                                Authority = "id.spot.robot",
                                Name = "robot-id",
                                UserTokenRequired = false
                            },
                            new ServiceEntry
                            {
                                Authority = "api.spot.robot",
                                Name = "directory",
                                UserTokenRequired = true
                            },
                            new ServiceEntry
                            {
                                Authority = "api.spot.robot",
                                Name = "directory-registration",
                                UserTokenRequired = true
                            },
                            new ServiceEntry
                            {
                                Authority = "graph-nav.spot.robot",
                                Name = "graph-nav-service",
                                UserTokenRequired = true
                            },
                            new ServiceEntry
                            {
                                Authority = "estop.spot.robot",
                                Name = "estop",
                                UserTokenRequired = true
                            },
                            new ServiceEntry
                            {
                                Authority = "timesync.spot.robot",
                                Name = "time-sync",
                                UserTokenRequired = true
                            }
                        }
                    }
                }
            );
        }
    }
}