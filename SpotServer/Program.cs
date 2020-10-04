using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bosdyn.Api;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Logging;

namespace SpotServer
{

    internal class SpotTimeSync : TimeSyncService.TimeSyncServiceBase
    {
        public override Task<TimeSyncUpdateResponse> TimeSyncUpdate(TimeSyncUpdateRequest request, ServerCallContext context)
        {
            return Task.FromResult(new TimeSyncUpdateResponse
            {
                Header = new ResponseHeader
                {
                    RequestHeader = request.Header,
                    ResponseTimestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                    RequestReceivedTimestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                    Error = new CommonError{Code = CommonError.Types.Code.Ok}
                },
                State = new TimeSyncState
                {
                    Status = TimeSyncState.Types.Status.Ok
                },
                ClockIdentifier = "123",
                PreviousEstimate = new TimeSyncEstimate
                {
                    ClockSkew = new Duration
                    {
                        Nanos = 10000
                    },
                    RoundTripTime = new Duration
                    {
                        Nanos = 50000
                    }
                }
            });
        }
    }
    
    internal class SpotDirectoryService : DirectoryService.DirectoryServiceBase
    {
        public override Task<ListServiceEntriesResponse> ListServiceEntries(ListServiceEntriesRequest request, ServerCallContext context)
        {
            return Task.FromResult(
                new ListServiceEntriesResponse
                {
                    Header = new ResponseHeader
                    {
                        RequestHeader = request.Header,
                        ResponseTimestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                        RequestReceivedTimestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                        Error = new CommonError{Code = CommonError.Types.Code.Ok}
                    },
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

    internal class SpotAuthService : AuthService.AuthServiceBase
    {
        public override Task<GetAuthTokenResponse> GetAuthToken(GetAuthTokenRequest request, ServerCallContext context)
        {
            return Task.FromResult(
                new GetAuthTokenResponse
                {
                    Header = new ResponseHeader
                    {
                        RequestHeader = request.Header,
                        ResponseTimestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                        RequestReceivedTimestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                        Error = new CommonError{Code = CommonError.Types.Code.Ok}
                    }, 
                    Status = GetAuthTokenResponse.Types.Status.Ok, 
                    Token = "test token"
                });
        }
    }

    internal class SpotRobotIdService : RobotIdService.RobotIdServiceBase
    {
        public override Task<RobotIdResponse> GetRobotId(RobotIdRequest request, ServerCallContext context)
        {
            return Task.FromResult(
                new RobotIdResponse{
                    Header = new ResponseHeader
                    {
                        RequestHeader = request.Header,
                        ResponseTimestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                        RequestReceivedTimestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                        Error = new CommonError{Code = CommonError.Types.Code.Ok}
                        
                    }, 
                    RobotId = new RobotId
                    {
                        Nickname = "monstrey-technologies", 
                        Species = "spot", 
                        Version = "2.0.0", 
                        SerialNumber = "B12313",  
                        ComputerSerialNumber = "fdafds",
                        SoftwareRelease = new RobotSoftwareRelease
                        {
                            Changeset = "Changeset", 
                            Name = "name",
                            ApiVersion = "2.0.0",
                            Type = "someting",
                            BuildInformation = "Buildinformation",
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
    
    internal class Program
    {
        private const int Port = 443;

        public static void Main(string[] args)
        { 
            GrpcEnvironment.SetLogger(new ConsoleLogger());
            Server server = new Server
            {
                Services =
                {
                    AuthService.BindService(new SpotAuthService()), 
                    RobotIdService.BindService(new SpotRobotIdService()),
                    DirectoryService.BindService(new SpotDirectoryService()),
                    TimeSyncService.BindService(new SpotTimeSync())
                },
                Ports = { new ServerPort(
                    "0.0.0.0", 
                    Port, 
                    new SslServerCredentials(
                        new List<KeyCertificatePair>() {
                            new KeyCertificatePair(
                                File.ReadAllText(@"server.crt"), 
                                File.ReadAllText(@"server.key")
                            )}, 
                        File.ReadAllText(@"ca.crt"),
                        false
                    )
                )},
            };
            server.Start();

            Console.WriteLine("Spot mock-server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}