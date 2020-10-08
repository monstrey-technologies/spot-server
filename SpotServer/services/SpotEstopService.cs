using System;
using System.Collections.Generic;
using System.Linq;
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
        List<EstopEndpointWithStatus> RegisteredEndpointsByConfig = new List<EstopEndpointWithStatus>();
        private EstopConfig activeConfig = null;

        private HashSet<EstopStopLevel> getStopLevels()
        {
            HashSet<EstopStopLevel> estopList = new HashSet<EstopStopLevel>();
            foreach (var estopEndpointWithStatuse in RegisteredEndpointsByConfig)
            {
                estopList.Add(estopEndpointWithStatuse.StopLevel);
            }
            return estopList;
        }
        
        public override Task<GetEstopSystemStatusResponse> GetEstopSystemStatus(GetEstopSystemStatusRequest request, ServerCallContext context)
        {
            return Task.FromResult(new GetEstopSystemStatusResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError {Code = CommonError.Types.Code.Ok}),
                Status = new EstopSystemStatus
                {
                    StopLevel = getStopLevels().Min(),
                    Endpoints = {RegisteredEndpointsByConfig}
                }
            });
        }

        public override Task<SetEstopConfigResponse> SetEstopConfig(SetEstopConfigRequest request, ServerCallContext context)
        {
            activeConfig = request.Config;
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
            var endpointIndex = RegisteredEndpointsByConfig.FindIndex(status => status.Endpoint.Role == request.Endpoint.Role);
            if (endpointIndex >= 0)
            {
                RegisteredEndpointsByConfig[endpointIndex].StopLevel = request.StopLevel;
            }
            
            Console.WriteLine($"EstopCheckIn - checkin endpoint: {request.StopLevel} | {request.Endpoint.Role} - {request.Endpoint.Name} | {getStopLevels().Min()}");
            
            return Task.FromResult(new EstopCheckInResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
                Status = (request.Challenge == ~request.Response)?EstopCheckInResponse.Types.Status.Ok:EstopCheckInResponse.Types.Status.IncorrectChallengeResponse,
                Request = request,
                Challenge = request.Challenge
            });
        }

        public override Task<DeregisterEstopEndpointResponse> DeregisterEstopEndpoint(DeregisterEstopEndpointRequest request, ServerCallContext context)
        {
            EstopEndpointWithStatus foundEndpoint = null;
            foreach (var registeredEndpoint in RegisteredEndpointsByConfig)
            {
                if (registeredEndpoint.Endpoint.UniqueId == request.TargetEndpoint.UniqueId)
                {
                    foundEndpoint = registeredEndpoint;
                }
            }

            if(foundEndpoint != null)
            {
                Console.WriteLine($"DeregisterEstopEndpointRequest - removing endpoint: {foundEndpoint.Endpoint.UniqueId}");
                RegisteredEndpointsByConfig.Remove(foundEndpoint);
            }
            
            return Task.FromResult(new DeregisterEstopEndpointResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
                Request = request,
                Status = foundEndpoint != null?DeregisterEstopEndpointResponse.Types.Status.Success:DeregisterEstopEndpointResponse.Types.Status.EndpointMismatch
            });
        }

        public override Task<RegisterEstopEndpointResponse> RegisterEstopEndpoint(RegisterEstopEndpointRequest request, ServerCallContext context)
        {
            EstopEndpointWithStatus foundEndpoint = null;
            EstopEndpointWithStatus newEndpoint = null;
            
            foreach (var registeredEndpoint in RegisteredEndpointsByConfig)
            {
                if (registeredEndpoint.Endpoint.UniqueId == request.TargetEndpoint.UniqueId)
                {
                    foundEndpoint = registeredEndpoint;
                    Console.WriteLine($"RegisterEstopEndpoint - endpoint allready exists: {registeredEndpoint.Endpoint.UniqueId}");
                }
            }

            if(foundEndpoint == null)
            {
                newEndpoint = new EstopEndpointWithStatus
                {
                    Endpoint = new EstopEndpoint(request.NewEndpoint),
                    StopLevel = EstopStopLevel.EstopLevelSettleThenCut
                };
                Console.WriteLine($"RegisterEstopEndpoint - adding new endpoint: {newEndpoint.Endpoint.UniqueId}");
                RegisteredEndpointsByConfig.Add(newEndpoint);
            }
            
            return Task.FromResult(new RegisterEstopEndpointResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
                Status = RegisterEstopEndpointResponse.Types.Status.Success,
                Request = request,
                NewEndpoint = (foundEndpoint ?? newEndpoint).Endpoint
            });
        }

        public override Task<GetEstopConfigResponse> GetEstopConfig(GetEstopConfigRequest request, ServerCallContext context)
        {
            Console.WriteLine($"GetEstopConfig - getting config with id: {request.TargetConfigId}");
            return Task.FromResult(new GetEstopConfigResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
                Request = request,
                ActiveConfig = activeConfig
            });
        }
    }
}