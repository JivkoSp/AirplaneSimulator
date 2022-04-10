using AirplaneSimulation.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private void UserInformation()
        {
            Console.SetCursorPosition(160, 5);
            Console.WriteLine("Press 'u' to receive real time updates");
            Console.SetCursorPosition(160, 8);
            Console.WriteLine("Press 'p' to pause");
            Console.SetCursorPosition(160, 11);
            Console.WriteLine("Press 'i' for user input");
        }

        public Simulator(Map map)
        {
            Random = new Random();
            Map = map;
            CollisionScanner.Map = map;
            SimulationData.Map = map;
        }

        public Task BeginSimulation()
        {
            UserInformation();

            foreach (var airfield in Map.Airfields)
            {
                List<List<KeyValuePair<int, int>>> neigbours = new List<List<KeyValuePair<int, int>>>();

                var others = Map.Airfields.Where(arf => arf.Coordinates.Item1 != airfield.Coordinates.Item1
                && arf.Coordinates.Item2 != airfield.Coordinates.Item2).ToList();

                foreach (var other in others)
                {
                    neigbours.Add(Map.FindPath(airfield.Coordinates.Item1, airfield.Coordinates.Item2,
                         other.Coordinates));
                }

                airfield.Neigbours = neigbours.OrderBy(prop => prop.Count).ToList();
            }

            while (!End)
            {
                try
                {
                    var airfield = Map.Airfields[Random.Next(0, Map.Airfields.Count)];

                    airfield.TakeOff();

                    Task.Run(async () => {

                        await SimulationData.ShowData(End);
                    });
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
