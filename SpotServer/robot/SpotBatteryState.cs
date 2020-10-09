using System.Collections.Generic;
using System.Linq;
using Bosdyn.Api;
using Google.Protobuf.WellKnownTypes;

namespace SpotServer.robot
{
    public class SpotBatteryState
    {
        private Dictionary<string, BatteryState> _batteryStates;

        public SpotBatteryState()
        {
            _batteryStates = new Dictionary<string, BatteryState>();
        }
        public void RegisterBattery(string identifier)
        {
            _batteryStates.TryAdd(identifier, new BatteryState
            {
                Identifier = identifier,
                Voltage = 58.8,
                Status = BatteryState.Types.Status.Discharging,
                ChargePercentage = 100,
                EstimatedRuntime = new Duration {Seconds = 90 * 60},
                Current = 7
            });
        }

        public List<BatteryState> GetAllBatteries()
        {
            return _batteryStates.Values.ToList();
        }
    }
}