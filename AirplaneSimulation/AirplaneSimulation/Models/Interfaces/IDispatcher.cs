using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AirplaneSimulation.Models.Interfaces
{
    public interface IDispatcher
    {
        Task TakeOff(Plane plane, List<KeyValuePair<int, int>> flyingCoordinates);
        Task Landing(Plane plane);
        Task<bool> FindRoute(Plane plane);
    }
}
