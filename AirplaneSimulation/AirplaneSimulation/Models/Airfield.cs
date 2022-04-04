using AirplaneSimulation.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AirplaneSimulation.Models
{
    public enum AirfieldType
    {
        Public = 1,
        Cargo = 2
    }

    public class Airfield : IAirfield
    {
        private Random Random = new Random();
        private object _lock = new object();
        public Map Map { get; }
        public int Capacity { get; set; }
        public AirfieldType AirfieldType { get; set; }
        public Tuple<int, int, int, int> Coordinates;
        public bool Track { get; set; }

        public Airfield(Map map, AirfieldType type, int capacity, int X, int Y, int Width, int Height)
        {
            Map = map;
            AirfieldType = type;
            Capacity = capacity;
            Coordinates = new Tuple<int, int, int, int>(X, Y, Width, Height);
        }

        public Task Landing(Plane plane)
        {
            return Task.CompletedTask;
        }

        public Task TakeOff()
        {
            return Task.CompletedTask;
        }
    }
}
