using AirplaneSimulation.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AirplaneSimulation.Models
{
    public class Dispatcher : IDispatcher
    {
        private Random Random = new Random();
        private static object _lock = new object();
        private Airfield Airfield;

        public Dispatcher(Airfield airfield)
        {
            Airfield = airfield;
        }

        public Task<bool> FindRoute(Plane plane)
        {
            return Task.FromResult(false);
        }

        public Task Landing(Plane plane)
        {
            return Task.CompletedTask;
        }

        public Task TakeOff(Plane plane, List<KeyValuePair<int, int>> flyingCoordinates)
        {
            return Task.CompletedTask;
        }
    }
}
