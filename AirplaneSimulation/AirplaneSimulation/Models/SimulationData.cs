using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AirplaneSimulation.Models
{
    public static class SimulationData
    {
        private static Task showAirfieldData(string message)
        {
            Console.SetCursorPosition(180, 2);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"***{message}***");
            Console.Beep(350, 100);

            int Y = 5;

            foreach (var airfield in Map.Airfields)
            {
                Console.SetCursorPosition(160, Y++);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"Airfield => {airfield.Coordinates.Item1} {airfield.Coordinates.Item2}");
                Console.SetCursorPosition(160, Y++);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Has:");
                Console.SetCursorPosition(160, Y++);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{airfield.Planes.Count()} landed planes.");
                Console.SetCursorPosition(160, Y++);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{airfield.TravelingPlanes.Count} traveling planes.");
                Console.SetCursorPosition(160, Y++);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{airfield.CrashedPlanes.Count} crashed planes.");
                Console.SetCursorPosition(160, Y++);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("--------------------------------------------------------");
                Y += 3;
            }

            return Task.CompletedTask;
        }

        private static Task showPlainesData()
        {
            foreach (var plane in CollisionScanner.FlyingPlanes)
            {
                for (int i = plane.FlyingPosition; i < plane.FlyingCoordinates.Count - 1; i++)
                {
                    Console.SetCursorPosition(plane.FlyingCoordinates[i].Key, plane.FlyingCoordinates[i].Value);

                    if (plane.Airfield.AirfieldType == AirfieldType.Public)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                    }

                    Console.Write("*");
                    Thread.Sleep(10);
                }
            }

            foreach (var plane in CollisionScanner.FlyingPlanes)
            {
                Console.SetCursorPosition((int)plane.X, (int)plane.Y);

                if (plane.Airfield.AirfieldType == AirfieldType.Public)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                Console.Write("o");
            }

            return Task.CompletedTask;
        }

        private static Task removePlainesData()
        {
            lock (Plane._lock)
            {
                foreach (var plane in CollisionScanner.FlyingPlanes)
                {
                    for (int i = plane.FlyingPosition - 1; i < plane.FlyingCoordinates.Count - 1; i++)
                    {
                        Console.SetCursorPosition(plane.FlyingCoordinates[i].Key, plane.FlyingCoordinates[i].Value);
                        Console.Write(" ");
                        Thread.Sleep(01);
                    }
                }
            }

            return Task.CompletedTask;
        }

        private static Task removeAirfieldData()
        {
            lock (Plane._lock)
            {
                Console.SetCursorPosition(180, 2);
                Console.Write("                                                        ");

                int Y = 5;

                foreach (var airfield in Map.Airfields)
                {
                    Console.SetCursorPosition(160, Y++);
                    Console.Write("                                                        ");
                    Console.SetCursorPosition(160, Y++);
                    Console.Write("                                                        ");
                    Console.SetCursorPosition(160, Y++);
                    Console.Write($"                                                       ");
                    Console.SetCursorPosition(160, Y++);
                    Console.Write($"                                                       ");
                    Console.SetCursorPosition(160, Y++);
                    Console.Write($"                                                       ");
                    Console.SetCursorPosition(160, Y++);
                    Console.Write("                                                        ");
                    Y += 3;
                }
            }

            return Task.CompletedTask;
        }

        public static Task AirfieldUpdate(object sender, EventArgs args)
        {
            if (sender is Dispatcher)
            {
                lock (Plane._lock)
                {
                    showAirfieldData("REAL TIME");
                }
            }

            return Task.CompletedTask;
        }

        private static void SubscribeToAirfieldUpdate()
        {
            foreach (var airfield in Map.Airfields)
            {
                airfield.Dispatcher.OnAirfieldUpdate += AirfieldUpdate;
            }
        }

        private static void UnSubscribeToAirfieldUpdate()
        {
            foreach (var airfield in Map.Airfields)
            {
                airfield.Dispatcher.OnAirfieldUpdate -= AirfieldUpdate;
            }
        }

        private static void ShowUserInterface()
        {
            lock (Plane._lock)
            {
                Console.SetCursorPosition(160, 15);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Press 1 to change plane speed");
                Console.SetCursorPosition(160, 18);
                Console.Write("Press 2 to change wether in airfield area");
                ShowUserInterfaceOptions();
            }
        }

        private static void HideUserInterface()
        {
            Console.SetCursorPosition(160, 15);
            Console.Write("                                         ");
            Console.SetCursorPosition(160, 18);
            Console.Write("                                         ");
            Console.SetCursorPosition(160, 21);
            Console.Write("                                         ");
        }

        private static void HidePlaneOptions()
        {
            Console.SetCursorPosition(160, 21);
            Console.Write("                                                ");
            Console.SetCursorPosition(160, 24);
            Console.Write("                                                ");
        }

        private static void ShowUserInterfaceOptions()
        {
            string X = string.Empty, Y = string.Empty;

            switch (Console.ReadKey(true).KeyChar)
            {
                case '1':
                    HideUserInterface();
                    Console.SetCursorPosition(160, 21);
                    Console.Write("Press 'q' to go back");
                    Console.SetCursorPosition(160, 15);
                    Console.Write("Select X, Y coordinates of airfield");
                    Console.SetCursorPosition(160, 18);
                    Console.Write("Enter X >> ");
                    X = Console.ReadLine();

                    if (X == "q")
                    {
                        HideUserInterface();
                        ShowUserInterface();
                        break;
                    }
                    else if (X.Length > 0)
                    {
                        Console.SetCursorPosition(160, 18);
                        Console.Write("Enter Y >> ");
                        Y = Console.ReadLine();
                    }

                    int x = 0, y = 0;

                    if (int.TryParse(X, out x) && int.TryParse(Y, out y))
                    {
                        var airfield = Map.Airfields.FirstOrDefault(arf => arf.Coordinates.Item1 == x
                            && arf.Coordinates.Item2 == y);

                        Console.SetCursorPosition(160, 21);
                        Console.Write("                                         ");

                        if (airfield != null)
                        {
                            double value = 0;
                            Console.SetCursorPosition(160, 21);
                            Console.Write("Press 1 for speed up");
                            Console.SetCursorPosition(160, 24);
                            Console.Write("Press 2 for speed down");

                            switch (Console.ReadKey(true).KeyChar)
                            {
                                case '1':

                                    HidePlaneOptions();
                                    Console.SetCursorPosition(160, 21);
                                    Console.Write("Enter speed up % >> ");
                                    string percent = Console.ReadLine();

                                    if (double.TryParse(percent, NumberStyles.Number,
                                        CultureInfo.CreateSpecificCulture("en-US"), out value))
                                    {
                                        airfield.ChangeSpeed = true;
                                        airfield.Up = true;
                                        airfield.ChangeSpeedPercent = value;
                                    }

                                    break;

                                case '2':

                                    HidePlaneOptions();
                                    Console.SetCursorPosition(160, 21);
                                    Console.Write("Enter speed down % >> ");
                                    percent = Console.ReadLine();

                                    if (double.TryParse(percent, NumberStyles.Number,
                                        CultureInfo.CreateSpecificCulture("en-US"), out value))
                                    {
                                        airfield.ChangeSpeed = true;
                                        airfield.Up = false;
                                        airfield.ChangeSpeedPercent = value;
                                    }

                                    break;
                            }
                        }
                        else
                        {
                            Console.SetCursorPosition(160, 21);
                            Console.Write("Airfield whith this coordinates does not exist!");
                            Console.SetCursorPosition(160, 24);
                            Console.Write("Press 'q' to go back >> ");
                            string answ = Console.ReadLine();

                            if (answ == "q")
                            {
                                HidePlaneOptions();
                                ShowUserInterface();
                            }
                        }
                    }
                    else
                    {
                        Console.SetCursorPosition(160, 21);
                        Console.Write("Airfield coordinates are not correct!");
                        Console.SetCursorPosition(160, 24);
                        Console.Write("Press 'q' to go back >> ");
                        string answ = Console.ReadLine();

                        if (answ == "q")
                        {
                            HidePlaneOptions();
                            ShowUserInterface();
                        }
                    }

                    break;

                case '2':

                    break;
            }
        }


        private static bool Read { get; set; }
        private static bool UserInfo { get; set; }
        public static CancellationTokenSource cts = new CancellationTokenSource();
        public static int pauseTime = 0; //time in ms
        public static Map Map { get; set; }

        public static Task ShowData(bool end)
        {
            while (!end)
            {
                switch (Console.ReadKey(true).KeyChar)
                {
                    case 'p':

                        if (pauseTime == 0)
                        {
                            pauseTime = 40000;

                            Task.Run(async () => {

                                await showAirfieldData("PAUSE");
                                await showPlainesData();
                            });
                        }
                        else
                        {
                            Console.Beep(550, 100);

                            Task.Run(async () => {

                                await removePlainesData();
                                await removeAirfieldData();
                                pauseTime = 0;
                                cts.Cancel();
                                cts = new CancellationTokenSource();
                            });
                        }
                        break;

                    case 'u':

                        Task.Run(async () => {

                            await removeAirfieldData();
                        });

                        if (!Read)
                        {
                            Read = true;
                            SubscribeToAirfieldUpdate();
                        }
                        else
                        {

                            Read = false;
                            UnSubscribeToAirfieldUpdate();
                        }

                        break;

                    case 'i':

                        if (!UserInfo)
                        {
                            pauseTime = 40000;
                            UserInfo = true;
                            ShowUserInterface();
                        }
                        else
                        {
                            Console.Beep(550, 100);
                            UserInfo = false;
                            HideUserInterface();
                            pauseTime = 0;
                            cts.Cancel();
                            cts = new CancellationTokenSource();
                        }

                        break;
                }
            }

            return Task.CompletedTask;
        }
    }
}
