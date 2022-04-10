using AirplaneSimulation.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AirplaneSimulation.Models
{
    public class LandingEventArgs
    {
        public int X;
        public int Y;
    }

    public abstract class Plane : IPlane
    {
        public static object _lock = new object();
        protected Random Random = new Random();
        public Airfield Airfield { get; set; }
        public Airfield TargetAirfield { get; set; }
        public double Tank { get; protected set; }
        protected double FuelCost { get; set; }
        protected double FuelCorrection { get; set; }
        protected double MaxSpeed { get; set; }
        public double MaxFlyingTime { get; protected set; }
        public double CurrentFlyingTime { get; protected set; }
        public double MaintenanceTime { get; protected set; }
        protected double FlyingSpeed { get; set; }
        public double Weight { get; set; }
        public double Speed { get; set; }
        public string Name { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int R { get; set; }
        public int FlyingPosition { get; set; }
        public bool MarkedForDeletion { get; set; }
        public double PreventCollision { get; set; }
        public bool PreventCollisionSet { get; set; }
        protected List<Airfield> Airfields { get; set; }
        public List<KeyValuePair<int, int>> FlyingCoordinates { get; set; }
        public delegate Task AsyncEventHandler<LandingEventArgs>(object sender, LandingEventArgs args);
        public virtual event AsyncEventHandler<LandingEventArgs> OnLanding;

        public Plane(Airfield airfield, string name, int r)
        {
            Airfield = airfield;
            Name = name;
            Tank = 100;
            FuelCost = 0;
            R = r;
            MaxSpeed = 0;
            Speed = 0;
            FuelCorrection = 0;
            MaxFlyingTime = 0;
            CurrentFlyingTime = 0;
            FlyingSpeed = 0;
            MaintenanceTime = 0;
            FlyingPosition = 0;
            MarkedForDeletion = false;
            Airfields = Airfield.Map.Airfields;
            FlyingCoordinates = new List<KeyValuePair<int, int>>();
        }

        public abstract void SetMaxFlyingTime();

        public virtual void SpeedUp(double percent)
        {
            this.Speed += this.Speed * percent;
            this.FuelCorrection = percent;
        }

        public virtual void SpeedDown(double percent)
        {
            this.Speed -= this.Speed * percent;
            this.FuelCorrection = percent;
        }

        public virtual void DrawPlane(double X, double Y)
        {
            Console.CursorVisible = true;
            Console.SetCursorPosition((int)this.X, (int)this.Y);

            if (this.Airfield.Map._map[(int)this.Y][(int)this.X].Value)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("#");
            }
            else { Console.Write(" "); }

            if (this.X != X)
            {
                Y += PreventCollision;
            }
            else
            {
                X += PreventCollision;
            }

            this.X = X;
            this.Y = Y;

            Console.SetCursorPosition((int)this.X, (int)this.Y);
            Console.ForegroundColor = ConsoleColor.Blue;
        }

        public Task Flying()
        {
            foreach (var coordinate in FlyingCoordinates)
            {
                SimulationData.cts.Token.WaitHandle.WaitOne(SimulationData.pauseTime);

                if (!MarkedForDeletion)
                {
                    if (FlyingPosition == FlyingCoordinates.Count - 4)
                    {
                        Task.Run(async () =>
                        {
                            lock (_lock)
                            {
                                MarkedForDeletion = true;
                                Console.SetCursorPosition((int)this.X, (int)this.Y);
                                Console.Write(" ");
                            }

                            var coordinates = FlyingCoordinates[FlyingCoordinates.Count - 1];
                            await Landing(coordinate.Key, coordinate.Value);
                        });

                        break;
                    }

                    lock (_lock)
                    {
                        DrawPlane(coordinate.Key, coordinate.Value);

                        Tank -= FuelCost * Random.NextDouble();
                        FlyingPosition++;

                        if (Airfield.ChangeSpeed)
                        {
                            if (Airfield.Up)
                            {
                                SpeedUp(Airfield.ChangeSpeedPercent);
                            }
                            else
                            {
                                SpeedDown(Airfield.ChangeSpeedPercent);
                            }

                            Airfield.ChangeSpeed = false;
                        }

                        Tank -= Airfield.Up ? (FuelCost * FuelCorrection) * -1 : FuelCorrection * FuelCorrection;

                        if (Tank < 10)
                        {
                            Console.SetCursorPosition((int)this.X, (int)this.Y);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("o");
                        }

                        if (Tank <= 0)
                        {
                            MarkedForDeletion = true;
                            Airfield.TravelingPlanes.Remove(this);
                            Airfield.CrashedPlanes.Add(this);
                            continue;
                        }
                    }

                    Task.Run(async () => {

                        await CollisionScanner.AirfieldCollisionScan(this, Airfields);
                    });


                    Task.Run(async () => {

                        await CollisionScanner.PlaneCollisionScan(this);
                    });

                    Task.Run(async () => {

                        var collision = await CollisionScanner.PredictPlaneCollisionScan(this);

                        if (!collision)
                        {

                            PreventCollision = 0;
                            PreventCollisionSet = false;
                        }
                    });

                    CurrentFlyingTime += FlyingSpeed;
                    Thread.Sleep((int)FlyingSpeed);
                }
                else
                {
                    lock (_lock)
                    {
                        Console.SetCursorPosition((int)this.X, (int)this.Y);
                        Console.Write("X");
                    }
                }
            }

            return Task.CompletedTask;
        }

        public async Task Landing(int targetX, int targetY)
        {
            SimulationData.cts.Token.WaitHandle.WaitOne(SimulationData.pauseTime);

            if (OnLanding != null)
            {
                await OnLanding(this, new LandingEventArgs() { X = targetX, Y = targetY });
            }
        }
    }

    public class CargoPlane : Plane, IPlane
    {
        //for one unit of cargo increase time of flying by some constant
        //i.e for 1 unit increase 0.001 ms maxFlyingTime
        private const double CargoTime_Constant = 0.001;
        private int Cargo;

        private void SetFlyingSpeed()
        {
            //dist/speed*1000 => ms + % of cargo weight / distance parts
            FlyingSpeed = ((100 / Speed) * 1000) + ((Cargo * CargoTime_Constant) / FlyingCoordinates.Count);
        }

        public CargoPlane(Airfield airfield, string name, int r) : base(airfield, name, r)
        {
            MaxSpeed = Random.Next(550, 750);
            Speed = MaxSpeed * 0.5;
            MaintenanceTime = Random.Next(4000, 8000);
            Cargo = Random.Next(100, 1000);
        }

        public override void SetMaxFlyingTime()
        {
            //time = dist/speed => time*1000 => time in ms
            MaxFlyingTime = ((FlyingCoordinates.Count * 100) / Speed) * 1000;
            MaxFlyingTime += Cargo * CargoTime_Constant;
            FuelCost = (double)100 / (double)FlyingCoordinates.Count;
            CurrentFlyingTime = 0;
            SetFlyingSpeed();
        }

        public override void DrawPlane(double X, double Y)
        {
            base.DrawPlane(X, Y);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("o");
        }

        public override void SpeedUp(double percent)
        {
            base.SpeedUp(percent);
            this.SetFlyingSpeed();
        }

        public override void SpeedDown(double percent)
        {
            base.SpeedDown(percent);
            this.SetFlyingSpeed();
        }
    }

    public class PublicPlane : Plane, IPlane
    {
        private const double SeatWeight = 0.8;
        private int SeatNumber;

        private void SetFlyingSpeed()
        {
            //dist/speed*1000 => ms + % of cargo weight / distance parts
            FlyingSpeed = ((100 / Speed) * 1000) + ((SeatNumber * SeatWeight) / FlyingCoordinates.Count);
        }

        public PublicPlane(Airfield airfield, string name, int r) : base(airfield, name, r)
        {
            MaxSpeed = Random.Next(650, 900);
            Speed = MaxSpeed * 0.6;
            MaintenanceTime = Random.Next(2000, 6000);
            SeatNumber = Random.Next(80, 280);
        }

        public override void SetMaxFlyingTime()
        {
            //time = dist/speed => time*1000 => time in ms
            MaxFlyingTime = ((FlyingCoordinates.Count * 100) / Speed) * 1000;
            MaxFlyingTime += SeatNumber * SeatWeight;
            FuelCost = (double)100 / (double)FlyingCoordinates.Count;
            CurrentFlyingTime = 0;
            SetFlyingSpeed();
        }

        public override void DrawPlane(double X, double Y)
        {
            base.DrawPlane(X, Y);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("o");
        }

        public override void SpeedUp(double percent)
        {
            base.SpeedUp(percent);
            this.SetFlyingSpeed();
        }

        public override void SpeedDown(double percent)
        {
            base.SpeedDown(percent);
            this.SetFlyingSpeed();
        }
    }
}
