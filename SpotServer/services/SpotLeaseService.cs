using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bosdyn.Api;
using Grpc.Core;
using SpotServer.infrastructure;
using SpotServer.robot;

namespace SpotServer.services
{
    public class SpotLeaseService : LeaseService.LeaseServiceBase
    {
        

        public override Task<AcquireLeaseResponse> AcquireLease(AcquireLeaseRequest request, ServerCallContext context)
        {
            AcquireLeaseResponse.Types.Status status;
            Lease lease = null;
            SpotRobot.SpotInstance.Leases.TryGetValue(request.Resource, out var leaseTuple);
            if (leaseTuple == null)
            {
                lease = new Lease
                {
                    Resource = request.Resource,
                    Sequence = { 1 }
                };
                SpotRobot.SpotInstance.Leases.Add(request.Resource, new Tuple<string, Lease>(request.Header.ClientName, lease));
                Console.WriteLine($"AcquireLease - registering new lease for resource \"{request.Resource}\" and client \"{request.Header.ClientName}\"");
                status = AcquireLeaseResponse.Types.Status.Ok;
            }
            else if (leaseTuple.Item1 == request.Header.ClientName)
            {
                Console.WriteLine($"AcquireLease - lease already exists for resource \"{request.Resource}\" and client \"{request.Header.ClientName}\"");
                status = AcquireLeaseResponse.Types.Status.Ok;
            }
            else
            {
                Console.WriteLine($"AcquireLease - lease already exists for resource \"{request.Resource}\" and but no for client \"{request.Header.ClientName}\"");
                status = AcquireLeaseResponse.Types.Status.ResourceAlreadyClaimed;
            }
            
            return Task.FromResult(new AcquireLeaseResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
                Status = status,
                Lease = lease,
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
            return Task.FromResult(new RetainLeaseResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
                LeaseUseResult = new LeaseUseResult
                {
                    Status = LeaseUseResult.Types.Status.Ok,
                }
            });
        }

        public override Task<ReturnLeaseResponse> ReturnLease(ReturnLeaseRequest request, ServerCallContext context)
        {
            ReturnLeaseResponse.Types.Status status = ReturnLeaseResponse.Types.Status.NotActiveLease;
            
            var key = SpotRobot.SpotInstance.Leases.First(pair =>  pair.Value.Item1 == request.Header.ClientName ).Key;
            if (key != null)
            {
                SpotRobot.SpotInstance.Leases.Remove(key);
                status = ReturnLeaseResponse.Types.Status.Ok;
                Console.WriteLine($"ReturnLease - lease returned for resource \"{ key }\" by client \"{request.Header.ClientName}\"");
            }
            return Task.FromResult(new ReturnLeaseResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
                Status = status
            });
        }
    }
}