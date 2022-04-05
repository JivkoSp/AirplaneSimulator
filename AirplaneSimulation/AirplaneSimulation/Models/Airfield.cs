using AirplaneSimulation.Data.Structures;
using AirplaneSimulation.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        public Dispatcher Dispatcher { get; }
        public MinHeap<Plane> Planes { get; set; }
        public int Capacity { get; set; }
        public AirfieldType AirfieldType { get; set; }
        public Tuple<int, int, int, int> Coordinates;
        public bool Track { get; set; }
        public HashSet<Plane> PlanesInAirspace { get; set; }

        public Airfield(Map map, AirfieldType type, int capacity, int X, int Y, int Width, int Height)
        {
            Map = map;
            AirfieldType = type;
            Capacity = capacity;
            Coordinates = new Tuple<int, int, int, int>(X, Y, Width, Height);
            Dispatcher = new Dispatcher(this);
            Planes = new MinHeap<Plane>();
            PlanesInAirspace = new HashSet<Plane>();

            for (int i = 0; i < Random.Next(5, 20); i++)
            {
                switch (AirfieldType)
                {
                    case AirfieldType.Cargo:
                        Planes.Insert(new CargoPlane(this, $"NormalPlane{i + 1}", 5));
                        break;
                    case AirfieldType.Public:
                        Planes.Insert(new PublicPlane(this, $"MilitaryPlane{i + 1}", 5));
                        break;
                }
            }
        }

        public Task Landing(Plane plane)
        {
            lock (_lock)
            {
                if (plane is CargoPlane)
                {
                    Planes.Insert(new CargoPlane(this, plane.Name, plane.R));
                }
                else
                {
                    Planes.Insert(new PublicPlane(this, plane.Name, plane.R));
                }

                Task.Run(async () => {

                    await Planes.Heapify();
                });

                Task.Run(() => {

                    Thread.Sleep(2000);
                    Track = false;
                    lock (Plane._lock)
                    {
                        this.Map.PaintAirfield(this.Coordinates, ConsoleColor.Green);
                    }
                });
            }

            return Task.CompletedTask;
        }

        public Task TakeOff()
        {
            lock (_lock)
            {
                if (Planes.Count() > 0)
                {
                    Plane plane = Planes.GetElement();

                    var airfieldCoordinates = Map.AirfieldCoordinates;

                    var targets = airfieldCoordinates.Where(arf => arf.Item1 != Coordinates.Item1
                    && arf.Item2 != Coordinates.Item2).ToList();

                    var target = targets[Random.Next(0, targets.Count)];

                    var targetAirfield = Map.Airfields.FirstOrDefault(arf => arf.Coordinates.Item1 == target.Item1 &&
                        arf.Coordinates.Item2 == target.Item2);

                    plane.TargetAirfield = targetAirfield;
                    var flyingCoordinates = Map.FindPath(Coordinates.Item1, Coordinates.Item2, target);

                    Task.Run(async () => {
                        await Dispatcher.TakeOff(plane, flyingCoordinates);
                    });
                }
            }

            return Task.CompletedTask;
        }
    }
}
