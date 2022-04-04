using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AirplaneSimulation.Models.Interfaces
{
    public interface IAirfield
    {
        Task TakeOff();
        Task Landing(Plane plane);
    }
}
