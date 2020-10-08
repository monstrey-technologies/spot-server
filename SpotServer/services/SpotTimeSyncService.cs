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
        private const string ClockIdentifier = "clock1234";
        public override Task<TimeSyncUpdateResponse> TimeSyncUpdate(TimeSyncUpdateRequest request, ServerCallContext context)
        {
            return Task.FromResult(new TimeSyncUpdateResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
                State = new TimeSyncState
                {
                    Status = TimeSyncState.Types.Status.Ok,
                    BestEstimate = new TimeSyncEstimate
                    {
                        ClockSkew = new Duration
                        {
                            Seconds = 0,
                        },
                        RoundTripTime = new Duration
                        {
                            Seconds = 0
                        },
                    },
                    MeasurementTime = Timestamp.FromDateTime(DateTime.UtcNow)
                },
                ClockIdentifier = request.ClockIdentifier ?? ClockIdentifier,
            });
        }
    }
}