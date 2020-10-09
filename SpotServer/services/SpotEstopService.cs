using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bosdyn.Api;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SpotServer.infrastructure;
using SpotServer.robot;

namespace SpotServer.services
{
    class SpotEstopService : EstopService.EstopServiceBase
    {

        public override Task<GetEstopSystemStatusResponse> GetEstopSystemStatus(GetEstopSystemStatusRequest request,
            ServerCallContext context)
        {
            return Task.FromResult(new GetEstopSystemStatusResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError {Code = CommonError.Types.Code.Ok}),
                Status = new EstopSystemStatus
                {
                    StopLevel = SpotRobot.SpotInstance.GetStopLevels().Min(),
                    Endpoints = {SpotRobot.SpotInstance.RegisteredEndpointsByConfig}
                }
            });
        }

        public override Task<SetEstopConfigResponse> SetEstopConfig(SetEstopConfigRequest request,
            ServerCallContext context)
        {
            SpotRobot.SpotInstance.EstopConfig = request.Config;
            return Task.FromResult(new SetEstopConfigResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError {Code = CommonError.Types.Code.Ok}),
                Status = SetEstopConfigResponse.Types.Status.Success,
                ActiveConfig = SpotRobot.SpotInstance.EstopConfig,
                Request = request
            });
        }

        public override Task<EstopCheckInResponse> EstopCheckIn(EstopCheckInRequest request, ServerCallContext context)
        {
            var endpointIndex =
                SpotRobot.SpotInstance.RegisteredEndpointsByConfig.FindIndex(status =>
                    status.Endpoint.Role == request.Endpoint.Role);
            EstopCheckInResponse.Types.Status status = EstopCheckInResponse.Types.Status.EndpointUnknown;
            if (endpointIndex >= 0)
            {
                SpotRobot.SpotInstance.RegisteredEndpointsByConfig[endpointIndex].StopLevel = request.StopLevel;
                SpotRobot.SpotInstance.HandleEstopCheckin();

                status = EstopCheckInResponse.Types.Status.Ok;
                Console.WriteLine(
                    $"EstopCheckIn - checkin endpoint: {request.StopLevel} | {request.Endpoint.Role} - {request.Endpoint.Name} | {SpotRobot.SpotInstance.GetStopLevels().Min()}");
            }

            if (request.Challenge != ~request.Response)
            {
                status = EstopCheckInResponse.Types.Status.IncorrectChallengeResponse;
            }

            return Task.FromResult(new EstopCheckInResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError {Code = CommonError.Types.Code.Ok}),
                Status = status,
                Request = request,
                Challenge = request.Challenge
            });
        }

        public override Task<DeregisterEstopEndpointResponse> DeregisterEstopEndpoint(
            DeregisterEstopEndpointRequest request, ServerCallContext context)
        {
            EstopEndpointWithStatus foundEndpoint = null;
            foreach (var registeredEndpoint in SpotRobot.SpotInstance.RegisteredEndpointsByConfig)
            {
                if (registeredEndpoint.Endpoint.UniqueId == request.TargetEndpoint.UniqueId)
                {
                    foundEndpoint = registeredEndpoint;
                }
            }

            if (foundEndpoint != null)
            {
                Console.WriteLine(
                    $"DeregisterEstopEndpointRequest - removing endpoint: {foundEndpoint.Endpoint.UniqueId}");
                SpotRobot.SpotInstance.RegisteredEndpointsByConfig.Remove(foundEndpoint);
            }

            return Task.FromResult(new DeregisterEstopEndpointResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError {Code = CommonError.Types.Code.Ok}),
                Request = request,
                Status = foundEndpoint != null
                    ? DeregisterEstopEndpointResponse.Types.Status.Success
                    : DeregisterEstopEndpointResponse.Types.Status.EndpointMismatch
            });
        }

        public override Task<RegisterEstopEndpointResponse> RegisterEstopEndpoint(RegisterEstopEndpointRequest request,
            ServerCallContext context)
        {
            EstopEndpointWithStatus foundEndpoint = null;
            EstopEndpointWithStatus newEndpoint = null;

            foreach (var registeredEndpoint in SpotRobot.SpotInstance.RegisteredEndpointsByConfig)
            {
                if (registeredEndpoint.Endpoint.UniqueId == request.TargetEndpoint.UniqueId)
                {
                    foundEndpoint = registeredEndpoint;
                    Console.WriteLine(
                        $"RegisterEstopEndpoint - endpoint allready exists: {registeredEndpoint.Endpoint.UniqueId}");
                }
            }

            if (foundEndpoint == null)
            {
                newEndpoint = new EstopEndpointWithStatus
                {
                    Endpoint = new EstopEndpoint(request.NewEndpoint),
                    StopLevel = EstopStopLevel.EstopLevelSettleThenCut
                };
                Console.WriteLine($"RegisterEstopEndpoint - adding new endpoint: {newEndpoint.Endpoint.UniqueId}");
                SpotRobot.SpotInstance.RegisteredEndpointsByConfig.Add(newEndpoint);
            }

            return Task.FromResult(new RegisterEstopEndpointResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError {Code = CommonError.Types.Code.Ok}),
                Status = RegisterEstopEndpointResponse.Types.Status.Success,
                Request = request,
                NewEndpoint = (foundEndpoint ?? newEndpoint).Endpoint
            });
        }

        public override Task<GetEstopConfigResponse> GetEstopConfig(GetEstopConfigRequest request,
            ServerCallContext context)
        {
            Console.WriteLine($"GetEstopConfig - getting config with id: {request.TargetConfigId}");
            return Task.FromResult(new GetEstopConfigResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError {Code = CommonError.Types.Code.Ok}),
                Request = request,
                ActiveConfig = SpotRobot.SpotInstance.EstopConfig
            });
        }
    }
}