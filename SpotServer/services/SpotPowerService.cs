using System;
using System.Threading;
using System.Threading.Tasks;
using Bosdyn.Api;
using Grpc.Core;
using SpotServer.infrastructure;

namespace SpotServer.services
{
    public class SpotPowerService : PowerService.PowerServiceBase
    {
        private PowerCommandStatus _status = PowerCommandStatus.StatusFaulted;

        public void StartMotors()
        {
            _status = PowerCommandStatus.StatusInProgress;
            new Thread(() =>
            {
                Thread.Sleep(2000);
                _status = PowerCommandStatus.StatusSuccess;
            }).Start();
        }

        public void StopMotors()
        {
            _status = PowerCommandStatus.StatusSuccess;
        }
        
        public override Task<PowerCommandResponse> PowerCommand(PowerCommandRequest request, ServerCallContext context)
        {
            if(request.Request == PowerCommandRequest.Types.Request.On)
            {
                StartMotors();
            }else if (request.Request == PowerCommandRequest.Types.Request.Off)
            {
                StopMotors();
            }
            
            return Task.FromResult(new PowerCommandResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
                LicenseStatus = LicenseInfo.Types.Status.Valid,
                Status = _status,
                PowerCommandId = 1,
            });
        }

        public override Task<PowerCommandFeedbackResponse> PowerCommandFeedback(PowerCommandFeedbackRequest request, ServerCallContext context)
        {
            Console.WriteLine($"PowerCommandFeedback - Motor status: {_status}");
            return Task.FromResult(new PowerCommandFeedbackResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError {Code = CommonError.Types.Code.Ok}),
                Status = _status
            });
        }
    }
}