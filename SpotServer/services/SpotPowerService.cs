using System;
using System.Threading;
using System.Threading.Tasks;
using Bosdyn.Api;
using Grpc.Core;
using SpotServer.infrastructure;
using SpotServer.robot;

namespace SpotServer.services
{
    public class SpotPowerService : PowerService.PowerServiceBase
    {
        public override Task<PowerCommandResponse> PowerCommand(PowerCommandRequest request, ServerCallContext context)
        {
            if(request.Request == PowerCommandRequest.Types.Request.On)
            {
                SpotRobot.SpotInstance.StartMotors();
            }else if (request.Request == PowerCommandRequest.Types.Request.Off)
            {
                SpotRobot.SpotInstance.StopMotors();
            }
            
            return Task.FromResult(new PowerCommandResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
                LicenseStatus = LicenseInfo.Types.Status.Valid,
                Status = SpotRobot.SpotInstance.PowerCommandStatus,
                PowerCommandId = 1,
            });
        }

        public override Task<PowerCommandFeedbackResponse> PowerCommandFeedback(PowerCommandFeedbackRequest request, ServerCallContext context)
        {
            Console.WriteLine($"PowerCommandFeedback - Motor status: {SpotRobot.SpotInstance.PowerCommandStatus}");
            return Task.FromResult(new PowerCommandFeedbackResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError {Code = CommonError.Types.Code.Ok}),
                Status = SpotRobot.SpotInstance.PowerCommandStatus
            });
        }
    }
}