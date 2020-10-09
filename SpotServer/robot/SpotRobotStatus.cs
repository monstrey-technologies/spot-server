using System;
using System.Collections.Generic;
using Bosdyn.Api;
using Google.Protobuf.WellKnownTypes;

namespace SpotServer.robot
{
    public class SpotRobotStatus
    {
        private readonly RobotState _robotState;

        public SpotRobotStatus()
        {
            var spotBatteryState = new SpotBatteryState();
            spotBatteryState.RegisterBattery("v-bat-1");
            
            _robotState = new RobotState
            {
                BatteryStates =
                {
                    spotBatteryState.GetAllBatteries()
                },
                
                CommsStates = { new CommsState
                    {
                        Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                        WifiState = new WiFiState
                        {
                            Essid = "v-wifi",
                            CurrentMode = WiFiState.Types.Mode.AccessPoint,
                        }
                    }
                },
                
                PowerState = new PowerState
                {
                    Timestamp = Timestamp.FromDateTime(DateTime.UtcNow),
                    MotorPowerState = PowerState.Types.MotorPowerState.StateUnknown
                }
            };
        }

        public void UpdatePowerState(PowerState.Types.MotorPowerState motorPowerState)
        {
            _robotState.PowerState.Timestamp = Timestamp.FromDateTime(DateTime.UtcNow);
            _robotState.PowerState.MotorPowerState = motorPowerState;
        }
        
        public RobotState RobotState
        {
            get => _robotState;
        }
    }
}