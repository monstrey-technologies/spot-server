using System;
using System.Collections.Generic;
using System.IO;
using Bosdyn.Api;
using Grpc.Core;
using Grpc.Core.Logging;
using SpotServer.services;

namespace SpotServer
{
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
                    TimeSyncService.BindService(new SpotTimeSyncService()),
                    EstopService.BindService(new SpotEstopService()),
                    LeaseService.BindService(new SpotLeaseService()),
                    PowerService.BindService(new SpotPowerService()),
                    RobotStateService.BindService(new SpotRobotStateService()),
                    RobotCommandService.BindService(new SpotRobotCommandService()),
                    ImageService.BindService(new SpotImageService())
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

            Console.WriteLine("Virtual spot 001 is active on port " + Port);
            Console.WriteLine("Press any key to shutdown");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}