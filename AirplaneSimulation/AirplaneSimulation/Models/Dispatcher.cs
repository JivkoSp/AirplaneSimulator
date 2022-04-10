using AirplaneSimulation.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AirplaneSimulation.Models
{
    public class Dispatcher : IDispatcher
    {
        private Random Random = new Random();
        private static object _lock = new object();
        private Airfield Airfield;
        private bool CanAcceptOtherKind { get; set; }
        public delegate Task AsyncEventHandler(object sender, EventArgs args);
        public event AsyncEventHandler OnAirfieldUpdate;

        private Task OnLandingRequest(Object sender, LandingEventArgs args)
        {
            Task.Run(async () => {

                if (sender is Plane) {

                    await Landing((Plane)sender);
                }
            });

            return Task.CompletedTask;
        }

        public Dispatcher(Airfield airfield)
        {
            Airfield = airfield;

            if (Airfield.AirfieldType == AirfieldType.Public)
            {
                switch (Random.Next(0, 1))
                {
                    case 0:
                        CanAcceptOtherKind = false;
                        break;
                    case 1:
                        CanAcceptOtherKind = true;
                        break;
                }
            }
        }

        public void AssignPlane(Plane plane)
        {
            plane.OnLanding += OnLandingRequest;
        }

        public void UnSignPlane(Plane plane)
        {
            plane.OnLanding -= OnLandingRequest;
        }

        public Task<bool> FindRoute(Plane plane)
        {
            foreach (var nei in Airfield.Neigbours)
            {
                var targetArf = Airfield.Map.Airfields.FirstOrDefault(arf =>
                 nei[nei.Count - 1].Key >= arf.Coordinates.Item1 - arf.Coordinates.Item3 / 2 &&
                 nei[nei.Count - 1].Key <= arf.Coordinates.Item1 + arf.Coordinates.Item3 / 2 &&
                 nei[nei.Count - 1].Value >= arf.Coordinates.Item2 - arf.Coordinates.Item4 / 2 &&
                 nei[nei.Count - 1].Value <= arf.Coordinates.Item2 + arf.Coordinates.Item4 / 2);

                double fuelCost = ((double)100 / (double)nei.Count) * Random.NextDouble();

                if (fuelCost * nei.Count <= plane.Tank)
                {
                    plane.MarkedForDeletion = false;
                    plane.FlyingPosition = 0;
                    plane.TargetAirfield.Dispatcher.UnSignPlane(plane);
                    plane.TargetAirfield = targetArf;
                    targetArf.Dispatcher.AssignPlane(plane);
                    plane.Airfield.TravelingPlanes.Add(plane);
                    plane.FlyingCoordinates = nei;
                    plane.SetMaxFlyingTime();

                    lock (Plane._lock)
                    {
                        Airfield.Map.PaintAirfield(Airfield.Coordinates, ConsoleColor.Cyan);
                    }

                    Task.Run(async () => {

                        await plane.Flying();
                    });

                    Thread.Sleep(3000);
                    lock (Plane._lock)
                    {
                        Airfield.Map.PaintAirfield(Airfield.Coordinates, ConsoleColor.Green);
                    }

                    break;
                }
            }

            //crash if its still marked
            if (plane.MarkedForDeletion)
            {
                lock (Plane._lock)
                {
                    Airfield.Map.PaintAirfield(Airfield.Coordinates, ConsoleColor.Yellow);
                }

                plane.Airfield.CrashedPlanes.Add(plane);
                CollisionScanner.FlyingPlanes.Remove(plane);
                OnAirfieldUpdate?.Invoke(this, null);
                Thread.Sleep(3000);
                lock (Plane._lock)
                {
                    Airfield.Map.PaintAirfield(Airfield.Coordinates, ConsoleColor.Green);
                }
            }

            return Task.FromResult(false);
        }

        public Task Landing(Plane plane)
        {
            SimulationData.cts.Token.WaitHandle.WaitOne(SimulationData.pauseTime);
            Airfield.PlanesInAirspace.Remove(plane);
            plane.Airfield.TravelingPlanes.Remove(plane);

            if ((Airfield.AirfieldType == plane.Airfield.AirfieldType || CanAcceptOtherKind == true) && 
                plane.CurrentFlyingTime <= plane.MaxFlyingTime && Airfield.Track == false &&
                Airfield.Planes.Count() < Airfield.Capacity && Airfield.PlanesInAirspace.Count == 0)
            {
                lock (Plane._lock)
                {
                    Airfield.Map.PaintAirfield(Airfield.Coordinates, ConsoleColor.Blue);
                }

                Airfield.Track = true;

                Task.Run(async () => {

                    await Airfield.Landing(plane);
                });

                OnAirfieldUpdate?.Invoke(this, null);
                CollisionScanner.FlyingPlanes.Remove(plane);
            }
            else if (plane.Tank > 0)
            {
                Task.Run(async () => {

                    await FindRoute(plane);
                });
            }

            return Task.CompletedTask;
        }

        public Task TakeOff(Plane plane, List<KeyValuePair<int, int>> flyingCoordinates)
        {
            SimulationData.cts.Token.WaitHandle.WaitOne(SimulationData.pauseTime);
            if (Airfield.Track == false && Airfield.PlanesInAirspace.Count == 0)
            {
                CollisionScanner.FlyingPlanes.Add(plane);
                Airfield.TravelingPlanes.Add(plane);
                Airfield.Track = true;
                plane.TargetAirfield.Dispatcher.AssignPlane(plane);

                OnAirfieldUpdate?.Invoke(this, null);

                lock (Plane._lock)
                {
                    Airfield.Map.PaintAirfield(Airfield.Coordinates, ConsoleColor.Red);
                }

                plane.FlyingCoordinates = flyingCoordinates;
                plane.SetMaxFlyingTime();

                Task.Run(async () => {

                    await plane.Airfield.Planes.Remove();
                    return Task.CompletedTask;
                });

                Task.Run(async () => {

                    await plane.Flying();
                });

                Task.Run(() => {

                    Thread.Sleep(2000);
                    Airfield.Track = false;

                    lock (Plane._lock)
                    {
                        Airfield.Map.PaintAirfield(Airfield.Coordinates, ConsoleColor.Green);
                    }
                });
            }

            return Task.CompletedTask;
        }
    }
}
