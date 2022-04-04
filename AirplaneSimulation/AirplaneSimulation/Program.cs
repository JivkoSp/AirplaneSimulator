using AirplaneSimulation.Models;
using System;

namespace AirplaneSimulation
{
    class Program
    {
        static void Main(string[] args)
        {
            Map map = new Map(150, 70, true);

            try
            {
                foreach (var airfield in map.Airfields)
                {
                    map.InsertAirfield(airfield.Coordinates);
                }

                map.Display();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
