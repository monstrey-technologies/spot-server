using System;
using System.Threading.Tasks;
using Bosdyn.Api;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using SpotServer.infrastructure;

namespace SpotServer.services
{
    class SpotTimeSyncService : TimeSyncService.TimeSyncServiceBase
    {
        public override Task<TimeSyncUpdateResponse> TimeSyncUpdate(TimeSyncUpdateRequest request, ServerCallContext context)
        {
            return Task.FromResult(new TimeSyncUpdateResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
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
}