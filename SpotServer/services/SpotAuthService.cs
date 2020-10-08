using System.Security.Cryptography;
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
                    Token = new HMACMD5().ComputeHash(System.Text.Encoding.UTF8.GetBytes(request.Username + request.Password)).ToString()
                });
        }
    }
}