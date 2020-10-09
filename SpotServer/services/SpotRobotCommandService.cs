using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Bosdyn.Api;
using Grpc.Core;
using SpotServer.infrastructure;
using SpotServer.robot;

namespace SpotServer.services
{
    public class SpotRobotCommandService : RobotCommandService.RobotCommandServiceBase
    {
        private Random generator = new Random();
        
        public override Task<RobotCommandResponse> RobotCommand(RobotCommandRequest request, ServerCallContext context)
        {
            SpotRobot.SpotInstance.Leases.TryGetValue(request.Lease.Resource, out var lease );
            LeaseUseResult.Types.Status leaseResultStatus = LeaseUseResult.Types.Status.Unknown; 
            if (lease != null)
            {
                if (lease.Item1 == request.Header.ClientName)
                {
                    leaseResultStatus = LeaseUseResult.Types.Status.Ok;
                }
                Console.WriteLine($"RobotCommand - {request.Command}");
            }
            else
            {
                leaseResultStatus = LeaseUseResult.Types.Status.InvalidLease;
            }
            return Task.FromResult(new RobotCommandResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
                Status = RobotCommandResponse.Types.Status.Ok,
                LeaseUseResult = new LeaseUseResult()
                {
                    Status = leaseResultStatus
                },
                RobotCommandId = (uint)generator.Next(Int32.MaxValue)
            });
        }

        public override Task<ClearBehaviorFaultResponse> ClearBehaviorFault(ClearBehaviorFaultRequest request, ServerCallContext context)
        {
            return base.ClearBehaviorFault(request, context);
        }

        public override Task<RobotCommandFeedbackResponse> RobotCommandFeedback(RobotCommandFeedbackRequest request, ServerCallContext context)
        {
            
            return Task.FromResult(new RobotCommandFeedbackResponse
            {
                Header = HeaderBuilder.Build(request.Header, new CommonError{Code = CommonError.Types.Code.Ok}),
                Feedback = new RobotCommandFeedback()
                {
                    MobilityFeedback = new MobilityCommand.Types.Feedback()
                    {
                        StandFeedback = new StandCommand.Types.Feedback()
                        {
                            Status = StandCommand.Types.Feedback.Types.Status.IsStanding
                        }
                    }
                },
                Status = RobotCommandFeedbackResponse.Types.Status.Processing
            });
        }
    }
}