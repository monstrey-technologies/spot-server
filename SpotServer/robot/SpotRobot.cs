using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bosdyn.Api;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;

namespace SpotServer.robot
{
    public sealed class SpotRobot
    {
        private static readonly SpotRobot Instance = new SpotRobot();

        public PowerCommandStatus PowerCommandStatus = PowerCommandStatus.StatusUnknown;
        public EstopConfig EstopConfig;
        public List<EstopEndpointWithStatus> RegisteredEndpointsByConfig = new List<EstopEndpointWithStatus>();
        public SpotRobotStatus SpotRobotStatus = new SpotRobotStatus();
        public Dictionary<string, Tuple<String, Lease>> Leases = new Dictionary<string, Tuple<String, Lease>>();
        
        static SpotRobot(){}
        private SpotRobot(){}

        public void StartMotors()
        {
            SpotInstance.PowerCommandStatus = PowerCommandStatus.StatusInProgress;
            new Thread(() =>
            {
                Thread.Sleep(2000);
                SpotInstance.PowerCommandStatus = PowerCommandStatus.StatusSuccess;
                SpotRobotStatus.UpdatePowerState(PowerState.Types.MotorPowerState.StateOn);
            }).Start();
        }

        public void StopMotors()
        {
            SpotInstance.PowerCommandStatus = PowerCommandStatus.StatusSuccess;
            SpotRobotStatus.UpdatePowerState(PowerState.Types.MotorPowerState.StateOff);
        }

        public void HandleEstopCheckin()
        {
            if (GetStopLevels().Min() == EstopStopLevel.EstopLevelCut ||
                GetStopLevels().Min() == EstopStopLevel.EstopLevelSettleThenCut)
            {
                SpotInstance.PowerCommandStatus = PowerCommandStatus.StatusEstopped;
            }
        }
        
        public HashSet<EstopStopLevel> GetStopLevels()
        {
            HashSet<EstopStopLevel> estopList = new HashSet<EstopStopLevel>();
            foreach (var estopEndpointWithStatuse in SpotInstance.RegisteredEndpointsByConfig)
            {
                estopList.Add(estopEndpointWithStatuse.StopLevel);
            }
            return estopList;
        }
        
        public static SpotRobot SpotInstance => Instance;
    }
}