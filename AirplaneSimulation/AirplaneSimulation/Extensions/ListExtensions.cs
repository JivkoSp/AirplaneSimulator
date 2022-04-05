using System;
using System.Collections.Generic;
using System.Text;

namespace AirplaneSimulation.Extensions
{
    public static class ListExtensions
    {
        public static void Swap<T>(this IList<T> list, int pos1, int pos2)
        {
            T tmp = list[pos1];
            list[pos1] = list[pos2];
            list[pos2] = tmp;
        }
    }
}
