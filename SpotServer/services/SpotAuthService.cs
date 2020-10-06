using System.Threading.Tasks;
using Bosdyn.Api;
using Grpc.Core;
using SpotServer.infrastructure;

namespace SpotServer.services
{
    class SpotAuthService : AuthService.AuthServiceBase
    {
        public override Task<GetAuthTokenResponse> GetAuthToken(GetAuthTokenRequest request, ServerCallContext context)
        {
            return Task.FromResult(
                new GetAuthTokenResponse
                {
                    Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
                    Status = GetAuthTokenResponse.Types.Status.Ok, 
                    Token = "test token"
                });
        }
    }
}