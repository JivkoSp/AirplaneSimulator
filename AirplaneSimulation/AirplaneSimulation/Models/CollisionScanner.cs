using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirplaneSimulation.Models
{
    public static class CollisionScanner
    {
        public static Map Map { get; set; }
        public static HashSet<Plane> FlyingPlanes { get; set; } = new HashSet<Plane>();

        public static Task PlaneCollisionScan(Plane plane)
        {
            var otherPlanes = FlyingPlanes.Where(p => p != plane).ToList();

            foreach (var otherPlane in otherPlanes)
            {
                double dx = plane.X - otherPlane.X;
                double dy = plane.Y - otherPlane.Y;
                double dist = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

                if (dist <= (double)plane.R * 0.1 + (double)otherPlane.R * 0.1)
                {
                    if (FlyingPlanes.Contains(plane))
                    {
                        plane.Airfield.TravelingPlanes.Remove(plane);
                        FlyingPlanes.Remove(plane);
                        plane.MarkedForDeletion = true;
                    }

                    otherPlane.Airfield.TravelingPlanes.Remove(otherPlane);
                    FlyingPlanes.Remove(otherPlane);
                    otherPlane.MarkedForDeletion = true;
                }
            }

            return Task.CompletedTask;
        }

        public static Task AirfieldCollisionScan(Plane plane, List<Airfield> Airfields)
        {
            foreach (var airfield in Airfields)
            {
                //collision detection between circle and rectangle
                double tempArfX = plane.X, tempArfY = plane.Y;

                //left
                if (plane.X < airfield.Coordinates.Item1 - airfield.Coordinates.Item3 / 2)
                {
                    tempArfX = airfield.Coordinates.Item1 - airfield.Coordinates.Item3 / 2;
                }
                //right
                else if (plane.X > airfield.Coordinates.Item1 + airfield.Coordinates.Item3 / 2)
                {
                    tempArfX = airfield.Coordinates.Item1 + airfield.Coordinates.Item3 / 2;
                }
                //top
                if (plane.Y < airfield.Coordinates.Item2 - airfield.Coordinates.Item4 / 2)
                {
                    tempArfY = airfield.Coordinates.Item2 - airfield.Coordinates.Item4 / 2;
                }
                //bottom
                else if (plane.Y > airfield.Coordinates.Item2 + airfield.Coordinates.Item4 / 2)
                {
                    tempArfY = airfield.Coordinates.Item2 + airfield.Coordinates.Item4 / 2;
                }

                double dx = plane.X - tempArfX;
                double dy = plane.Y - tempArfY;
                double dist = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

                if (dist <= plane.R && !plane.MarkedForDeletion)
                {
                    airfield.PlanesInAirspace.Add(plane);
                }
                else
                {
                    if (airfield.PlanesInAirspace.Contains(plane))
                    {
                        airfield.PlanesInAirspace.Remove(plane);
                    }
                }
            }

            return Task.CompletedTask;
        }

        public static Task<bool> PredictPlaneCollisionScan(Plane plane)
        {
            List<KeyValuePair<int, int>> route = new List<KeyValuePair<int, int>>();
            var otherPlanes = FlyingPlanes.Where(p => p != plane).ToList();

            foreach (var otherPlane in otherPlanes)
            {
                double dx = plane.X - otherPlane.X;
                double dy = plane.Y - otherPlane.Y;
                double dist = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

                if (dist <= (double)plane.R * 0.4 + (double)otherPlane.R * 0.4)
                {
                    if (!plane.PreventCollisionSet && !otherPlane.PreventCollisionSet)
                    {
                        plane.PreventCollision = (double)plane.R / 2;
                        otherPlane.PreventCollision -= (double)otherPlane.R / 2;
                        plane.PreventCollisionSet = true;
                    }

                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }
    }
}
