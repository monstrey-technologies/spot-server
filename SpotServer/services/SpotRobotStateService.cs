using System.Threading.Tasks;
using Bosdyn.Api;
using Grpc.Core;
using SpotServer.infrastructure;
using SpotServer.robot;

namespace SpotServer.services
{
    public class SpotRobotStateService : RobotStateService.RobotStateServiceBase
    {
        public override Task<RobotMetricsResponse> GetRobotMetrics(RobotMetricsRequest request, ServerCallContext context)
        {
            return base.GetRobotMetrics(request, context);
        }

        public override Task<RobotStateResponse> GetRobotState(RobotStateRequest request, ServerCallContext context)
        {
            return Task.FromResult(new RobotStateResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError {Code = CommonError.Types.Code.Ok}),
                RobotState = SpotRobot.SpotInstance.SpotRobotStatus.RobotState
            });
        }

        public override Task<RobotHardwareConfigurationResponse> GetRobotHardwareConfiguration(RobotHardwareConfigurationRequest request, ServerCallContext context)
        {
            return base.GetRobotHardwareConfiguration(request, context);
        }

        public override Task<RobotLinkModelResponse> GetRobotLinkModel(RobotLinkModelRequest request, ServerCallContext context)
        {
            return base.GetRobotLinkModel(request, context);
        }
    }
}