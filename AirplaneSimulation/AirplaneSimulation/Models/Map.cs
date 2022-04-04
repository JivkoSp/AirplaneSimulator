using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirplaneSimulation.Models
{
    public class Map
    {
        public List<List<KeyValuePair<int, bool>>> _map { get; set; }
        private bool _create { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public List<Tuple<int, int, int, int>> AirfieldCoordinates;
        public List<Airfield> Airfields { get; set; }

        public Map(int widht, int height, bool create)
        {
            Width = widht;
            Height = height;
            _create = create;
            _map = new List<List<KeyValuePair<int, bool>>>();
            AirfieldCoordinates = new List<Tuple<int, int, int, int>>();
            Airfields = new List<Airfield>
            {
                new Airfield(this, AirfieldType.Public, 35, 50, 30, 5, 3),
                new Airfield(this, AirfieldType.Public, 50, 80, 10, 5, 3),
                new Airfield(this, AirfieldType.Cargo, 50, 100, 60, 5, 3),
                new Airfield(this, AirfieldType.Public, 65, 80, 65, 5, 3),
                new Airfield(this, AirfieldType.Cargo, 60, 10, 50, 5, 3),
                new Airfield(this, AirfieldType.Public, 65, 15, 20, 5, 3),
                new Airfield(this, AirfieldType.Cargo, 60, 140, 40, 5, 3),
                new Airfield(this, AirfieldType.Public, 50, 90, 30, 5, 3)
            };

            if (_create)
            {
                Create();
            }
        }

        public void Create()
        {
            for (int row = 0; row < Height; row++)
            {
                List<KeyValuePair<int, bool>> column = new List<KeyValuePair<int, bool>>();

                for (int col = 0; col < Width; col++)
                {
                    if (row == 0)
                    {
                        column.Add(new KeyValuePair<int, bool>(col, true));
                    }
                    else if (col == 0 || col == Width - 1)
                    {
                        column.Add(new KeyValuePair<int, bool>(col, true));
                    }
                    else if (row == Height - 1)
                    {
                        column.Add(new KeyValuePair<int, bool>(col, true));
                    }
                    else
                    {
                        column.Add(new KeyValuePair<int, bool>(col, false));
                    }
                }

                _map.Add(column);
            }
        }

        public bool InsertAirfield(Tuple<int, int, int, int> Coordinates)
        {
            if (!_create)
            {
                throw new Exception("Map-not-created Exception");
            }

            if (Coordinates.Item1 - Coordinates.Item3 / 2 < 0 ||
                Coordinates.Item2 - Coordinates.Item4 / 2 < 0 ||
                Coordinates.Item1 + Coordinates.Item3 >= _map[0].Count ||
                Coordinates.Item2 + Coordinates.Item4 >= _map.Count)
            {
                return false;
            }

            int startH = Coordinates.Item2 - Coordinates.Item4 / 2;
            int Height = Coordinates.Item2 + Coordinates.Item4 / 2;

            int startW = Coordinates.Item1 - Coordinates.Item3 / 2;
            int Width = Coordinates.Item1 + Coordinates.Item3 / 2;

            for (int h = startH; h <= Height; h++)
            {
                for (int w = startW; w <= Width; w++)
                {
                    _map[h][w] = new KeyValuePair<int, bool>(w, true);
                }
            }

            AirfieldCoordinates.Add(Coordinates);
            return true;
        }

        private bool XRange(int X, Tuple<int, int, int, int> target)
        {
            return X >= target.Item1 - target.Item3 / 2
                && X <= target.Item1 + target.Item3 / 2;
        }

        private bool YRange(int Y, Tuple<int, int, int, int> target)
        {
            return Y >= target.Item2 - target.Item4 / 2
                && Y <= target.Item2 + target.Item4 / 2;
        }

        public List<KeyValuePair<int, int>> FindPath(int X1, int Y1, Tuple<int, int, int, int> target)
        {
            Queue<List<KeyValuePair<int, int>>> allPaths = new Queue<List<KeyValuePair<int, int>>>();
            Queue<KeyValuePair<int, int>> bfsQ = new Queue<KeyValuePair<int, int>>();
            HashSet<KeyValuePair<int, int>> visited = new HashSet<KeyValuePair<int, int>>();

            bfsQ.Enqueue(new KeyValuePair<int, int>(X1, Y1));
            visited.Add(new KeyValuePair<int, int>(X1, Y1));
            allPaths.Enqueue(new List<KeyValuePair<int, int>>() { new KeyValuePair<int, int>(X1, Y1) });

            while (bfsQ.Count > 0)
            {
                int size = bfsQ.Count;

                while (size > 0)
                {
                    var node = bfsQ.Dequeue();
                    var list = allPaths.Dequeue();

                    if (XRange(node.Key, target) && YRange(node.Value, target))
                    {
                        return list;
                    }

                    if (node.Value > 0
                        && !visited.Contains(new KeyValuePair<int, int>(node.Value - 1, node.Key)))
                    {
                        bfsQ.Enqueue(new KeyValuePair<int, int>(node.Key, node.Value - 1));
                        visited.Add(new KeyValuePair<int, int>(node.Value - 1, node.Key));
                        var cpyList = list.Select(pair => new KeyValuePair<int, int>(pair.Key, pair.Value)).ToList();
                        cpyList.Add(new KeyValuePair<int, int>(node.Key, node.Value - 1));
                        allPaths.Enqueue(cpyList);
                    }
                    if (node.Value < _map.Count - 1
                        && !visited.Contains(new KeyValuePair<int, int>(node.Value + 1, node.Key)))
                    {
                        bfsQ.Enqueue(new KeyValuePair<int, int>(node.Key, node.Value + 1));
                        visited.Add(new KeyValuePair<int, int>(node.Value + 1, node.Key));
                        var cpyList = list.Select(pair => new KeyValuePair<int, int>(pair.Key, pair.Value)).ToList();
                        cpyList.Add(new KeyValuePair<int, int>(node.Key, node.Value + 1));
                        allPaths.Enqueue(cpyList);
                    }
                    if (node.Key > 0
                        && !visited.Contains(new KeyValuePair<int, int>(node.Value, node.Key - 1)))
                    {
                        bfsQ.Enqueue(new KeyValuePair<int, int>(node.Key - 1, node.Value));
                        visited.Add(new KeyValuePair<int, int>(node.Value, node.Key - 1));
                        var cpyList = list.Select(pair => new KeyValuePair<int, int>(pair.Key, pair.Value)).ToList();
                        cpyList.Add(new KeyValuePair<int, int>(node.Key - 1, node.Value));
                        allPaths.Enqueue(cpyList);
                    }
                    if (node.Key < _map[0].Count - 1
                         && !visited.Contains(new KeyValuePair<int, int>(node.Value, node.Key + 1)))
                    {
                        bfsQ.Enqueue(new KeyValuePair<int, int>(node.Key + 1, node.Value));
                        visited.Add(new KeyValuePair<int, int>(node.Value, node.Key + 1));
                        var cpyList = list.Select(pair => new KeyValuePair<int, int>(pair.Key, pair.Value)).ToList();
                        cpyList.Add(new KeyValuePair<int, int>(node.Key + 1, node.Value));
                        allPaths.Enqueue(cpyList);
                    }

                    size--;
                }
            }

            return null;
        }

        public Task PaintAirfield(Tuple<int, int, int, int> Coordinates, ConsoleColor color)
        {
            Console.CursorVisible = false;
            int startH = Coordinates.Item2 - Coordinates.Item4 / 2;
            int Height = Coordinates.Item2 + Coordinates.Item4 / 2;

            int startW = Coordinates.Item1 - Coordinates.Item3 / 2;
            int Width = Coordinates.Item1 + Coordinates.Item3 / 2;

            for (int h = startH; h <= Height; h++)
            {
                for (int w = startW; w <= Width; w++)
                {
                    Console.SetCursorPosition(w, h);
                    Console.ForegroundColor = color;
                    Console.Write("#");
                }
            }

            return Task.CompletedTask;
        }

        public void Display()
        {
            for (int i = 0; i < _map.Count; i++)
            {
                for (int j = 0; j < _map[i].Count; j++)
                {
                    if (_map[i][j].Value == true)
                    {
                        Console.Write("#");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
