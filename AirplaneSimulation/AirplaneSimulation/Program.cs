using AirplaneSimulation.Models;
using System;
using System.Threading;

namespace AirplaneSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            Map map = new Map(150, 70, true);
            Simulator simulator = new Simulator(map);

            try
            {
                foreach (var airfield in map.Airfields)
                {
                    map.InsertAirfield(airfield.Coordinates);
                }

                map.Display();

                var mainThread = new Thread(() =>
                {
                    simulator.BeginSimulation();
                });

                mainThread.Start();
                mainThread.Join();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
