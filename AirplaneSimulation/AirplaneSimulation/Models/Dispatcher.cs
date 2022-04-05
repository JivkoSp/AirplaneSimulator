using AirplaneSimulation.Models.Interfaces;
using System;
using System.Collections.Generic;
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

        private Task OnLandingRequest(Object sender, LandingEventArgs args)
        {
            Task.Run(async () => {

                if (sender is Plane)
                {

                    await Landing((Plane)sender);
                }
            });

            return Task.CompletedTask;
        }

        public Dispatcher(Airfield airfield)
        {
            Airfield = airfield;
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
            return Task.FromResult(false);
        }

        public Task Landing(Plane plane)
        {
            Airfield.PlanesInAirspace.Remove(plane);

            if (plane.CurrentFlyingTime <= plane.MaxFlyingTime && Airfield.Track == false &&
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
            }

            return Task.CompletedTask;
        }

        public Task TakeOff(Plane plane, List<KeyValuePair<int, int>> flyingCoordinates)
        {
            if (Airfield.Track == false && Airfield.PlanesInAirspace.Count == 0)
            {           
                Airfield.Track = true;
                plane.TargetAirfield.Dispatcher.AssignPlane(plane);

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
