using System;
using System.Threading.Tasks;
using Bosdyn.Api;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SpotServer.infrastructure;

namespace SpotServer.services
{
    class SpotEstopService : EstopService.EstopServiceBase
    {
        public override Task<SetEstopConfigResponse> SetEstopConfig(SetEstopConfigRequest request, ServerCallContext context)
        {
            EstopConfig activeConfig = request.Config;
            activeConfig.UniqueId = request.TargetConfigId;
            return Task.FromResult(new SetEstopConfigResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
                Status = SetEstopConfigResponse.Types.Status.Success,
                ActiveConfig = activeConfig,
                Request = request
            });
        }

        public override Task<EstopCheckInResponse> EstopCheckIn(EstopCheckInRequest request, ServerCallContext context)
        {
            if (request.StopLevel == EstopStopLevel.EstopLevelCut)
            {
                Console.WriteLine("EMERGENCY STOP");
            }
            return Task.FromResult(new EstopCheckInResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
                Status = (request.Challenge == ~request.Response)?EstopCheckInResponse.Types.Status.Ok:EstopCheckInResponse.Types.Status.IncorrectChallengeResponse,
                Request = request,
                Challenge = request.Challenge
            });
        }

        public override Task<RegisterEstopEndpointResponse> RegisterEstopEndpoint(RegisterEstopEndpointRequest request, ServerCallContext context)
        {
            return Task.FromResult(new RegisterEstopEndpointResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
                Status = RegisterEstopEndpointResponse.Types.Status.Success,
                Request = request,
                NewEndpoint = new EstopEndpoint(request.NewEndpoint)
            });
        }

        public override Task<GetEstopConfigResponse> GetEstopConfig(GetEstopConfigRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GetEstopConfigResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
                Request = request,
                ActiveConfig = new EstopConfig()
                {
                    UniqueId = Guid.NewGuid().ToString("N"),
                    Endpoints = { 
                        new EstopEndpoint
                        {
                            Name = "Estop",
                            Role = "PDB_rooted",
                            UniqueId = Guid.NewGuid().ToString("N"),
                            Timeout = new Duration{Seconds = 5}
                        }}
                }
            });
        }
    }
}