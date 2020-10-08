using System.Collections.Generic;
using System.Threading.Tasks;
using Bosdyn.Api;
using Grpc.Core;
using SpotServer.infrastructure;

namespace SpotServer.services
{
    public class SpotLeaseService : LeaseService.LeaseServiceBase
    {
        private static readonly Dictionary<string, Lease> Leases = new Dictionary<string, Lease>();

        public override Task<AcquireLeaseResponse> AcquireLease(AcquireLeaseRequest request, ServerCallContext context)
        {
            if (!Leases.ContainsKey(request.Resource))
            {
                Leases.Add(request.Resource, new Lease
                {
                    Resource = request.Resource
                });
            }
            return Task.FromResult(new AcquireLeaseResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
                Status = AcquireLeaseResponse.Types.Status.Ok,
                Lease = new Lease
                {
                    
                },
                LeaseOwner = new LeaseOwner
                {
                    ClientName = request.Header.ClientName,
                }
            });
        }

        public override Task<ListLeasesResponse> ListLeases(ListLeasesRequest request, ServerCallContext context)
        {
            return base.ListLeases(request, context);
        }

        public override Task<TakeLeaseResponse> TakeLease(TakeLeaseRequest request, ServerCallContext context)
        {
            return base.TakeLease(request, context);
        }

        public override Task<RetainLeaseResponse> RetainLease(RetainLeaseRequest request, ServerCallContext context)
        {
            return base.RetainLease(request, context);
        }

        public override Task<ReturnLeaseResponse> ReturnLease(ReturnLeaseRequest request, ServerCallContext context)
        {
            return base.ReturnLease(request, context);
        }
    }
}