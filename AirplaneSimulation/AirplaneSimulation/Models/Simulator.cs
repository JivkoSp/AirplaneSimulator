using AirplaneSimulation.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AirplaneSimulation.Models
{
    public class Simulator : ISimulation
    {
        private Random Random;
        private Map Map;
        public bool End { get; set; } = false;

        public Simulator(Map map)
        {
            Random = new Random();
            Map = map;
        }

        public Task BeginSimulation()
        {
            while (!End)
            {
                try
                {
                    var airfield = Map.Airfields[Random.Next(0, Map.Airfields.Count)];

                    airfield.TakeOff();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                Thread.Sleep(Random.Next(2000, 5000));
            }

            return Task.CompletedTask;
        }

        public Task EndSimulation()
        {
            End = true;
            return Task.CompletedTask;
        }
    }
}
