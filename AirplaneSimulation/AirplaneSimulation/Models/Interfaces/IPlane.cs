using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AirplaneSimulation.Models.Interfaces
{
    public interface IPlane
    {
        Task Flying();
        Task Landing(int targetX, int targetY);
    }
}
