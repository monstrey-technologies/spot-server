using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bosdyn.Api;
using Grpc.Core;

namespace SpotServer
{
    class SpotAuthService : Bosdyn.Api.AuthService.AuthServiceBase
    {
        public override Task<GetAuthTokenResponse> GetAuthToken(GetAuthTokenRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GetAuthTokenResponse {Header = new ResponseHeader {}, Status = GetAuthTokenResponse.Types.Status.Ok, Token = "test token"});
            // return base.GetAuthToken(request, context);
        }
    }
    
    internal class Program
    {
        private const int Port = 443;

        public static void Main(string[] args)
        {
            Server server = new Server
            {
                Services = { Bosdyn.Api.AuthService.BindService((new SpotAuthService())) },
                Ports = { new ServerPort("localhost", Port, new SslServerCredentials(new List<KeyCertificatePair>())) }
            };
            server.Start();

            Console.WriteLine("Spot mock-server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}