using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bosdyn.Api;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Core.Logging;

namespace SpotServer
{

    internal class SpotAuthService : AuthService.AuthServiceBase
    {
        public override Task<GetAuthTokenResponse> GetAuthToken(GetAuthTokenRequest request, ServerCallContext context)
        {
            Console.WriteLine("SpotAuthService");
            return Task.FromResult(
                new GetAuthTokenResponse
                {
                    Header = new ResponseHeader
                    {
                        RequestHeader = request.Header,
                        RequestReceivedTimestamp = new Timestamp()
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
            Console.WriteLine(request.Header.ClientName);
            return Task.FromResult(
                new RobotIdResponse{
                    Header = new ResponseHeader
                    {
                        RequestHeader = request.Header,
                        RequestReceivedTimestamp = new Timestamp()
                    }, 
                    RobotId = new RobotId
                    {
                        Nickname = "mock-robot", 
                        Species = "spot", 
                        Version = "2.0", 
                        SerialNumber = "mock-serial", 
                        SoftwareRelease = new RobotSoftwareRelease{}, 
                        ComputerSerialNumber = "1234"
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
                Services = { AuthService.BindService(new SpotAuthService()), RobotIdService.BindService(new SpotRobotIdService())},
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