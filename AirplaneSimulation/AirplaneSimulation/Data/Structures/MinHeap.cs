using AirplaneSimulation.Extensions;
using AirplaneSimulation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirplaneSimulation.Data.Structures
{
    public class MinHeap<T> where T : Plane
    {
        private static object _lock = new object();
        private List<T> list;

        private int leftPos(int pos)
        {
            return pos * 2 + 1;
        }

        private int rightPos(int pos)
        {
            return pos * 2 + 2;
        }

        private Task heapify(int pos)
        {
            int left = leftPos(pos);
            int right = rightPos(pos);
            int currentPos = 0;

            if (left < list.Count && list[left].MaintenanceTime < list[pos].MaintenanceTime)
            {
                currentPos = left;
                list.Swap(left, pos);
            }

            if (right < list.Count && list[right].MaintenanceTime < list[pos].MaintenanceTime)
            {
                currentPos = right;
                list.Swap(right, pos);
            }

            if (currentPos != 0)
            {
                heapify(currentPos);
                heapify(currentPos / 2);
            }

            return Task.CompletedTask;
        }

        public MinHeap()
        {
            list = new List<T>();
        }

        public void Insert(T element)
        {
            lock (_lock)
            {
                list.Add(element);
            }
        }

        public Task Heapify()
        {
            for (int i = 0; i < list.Count; i++)
            {
                lock (_lock)
                {
                    heapify(i);
                }
            }

            return Task.CompletedTask;
        }

        public T GetElement()
        {
            return list[0];
        }

        public Task<T> FindElement(T element)
        {
            T result = list.FirstOrDefault(p => p == element);
            return Task.FromResult(result);
        }

        public int Count()
        {
            return list.Count;
        }

        public Task Remove()
        {
            lock (_lock)
            {
                list[0] = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);

                Task.Run(async () => {

                    await heapify(0);
                });
            }

            return Task.CompletedTask;
        }
    }
}
