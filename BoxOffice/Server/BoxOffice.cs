using System;
using System.Net.Sockets;

namespace BoxOffice.Server
{
    public class BoxOffice
    {
        private class Schedule
        {
            private readonly Screen[] _screens = new Screen[5];
            private static readonly Screen NullScreen = new Screen("");

            public Schedule()
            {
                for (int i = 0; i < 5; i++)
                {
                    _screens[i] = NullScreen;
                }
            }

            public void SwapMovies(int screenA, int screenB)
            {
                var temp = _screens[screenA - 1];
                _screens[screenA - 1] = _screens[screenB - 1];
                _screens[screenB - 1] = temp;
            }

            public void AddMovie(string title, int screenNumber)
            {
                _screens[screenNumber - 1] = new Screen(title);
            }

            public void RemoveMovie(int screenNumber)
            {
                _screens[screenNumber - 1] = NullScreen;
            }

            public bool IsNull(int screenNumber)
            {
                return _screens[screenNumber - 1].Equals(NullScreen);
            }

            public Screen GetScreen(int screenNumber)
            {
                return _screens[screenNumber - 1];
            }

            public void PrintSchedule()
            {
                Console.WriteLine("Today's Showings:");
                Console.WriteLine("-------------------------------");
                for (int i = 0; i < 5; i++)
                {
                    Console.WriteLine("Screen {0}: {1}", i+1, _screens[i].Title);
                }
                Console.WriteLine("-------------------------------");
            }

            public void PrintResults()
            {
                Console.WriteLine("---------------------------------");
                for (int i = 0; i < 5; i++)
                {
                    if (!IsNull(i + 1))
                    {
                        Console.WriteLine("Screen {0}", i + 1);
                        _screens[i].PrintSummary();
                        Console.WriteLine("---------------------------------");
                    }
                }
                
            }
            
        }

        private Schedule Sched { get; }
        public bool Open { get; private set; }
        private int Purchased { get; set; }

        public BoxOffice()
        {
            this.Sched = new Schedule();
            Open = false;
            this.Purchased = 0;
        }

        public void SwapMovies()
        {
            if (!Open)
            {
                Sched.PrintSchedule();
                Console.WriteLine("Please Enter A Screen Number:");
                var screenA = Console.ReadLine();
                if (Int32.TryParse(screenA, out var screenNumA))
                {
                    if (screenNumA < 1 || screenNumA > 5)
                    {
                        Console.WriteLine("Invalid numeral entered. Please enter a numeral in range 1-5");
                        return;
                    }

                    Console.WriteLine("Please enter screen number to swap with screen {0}", screenNumA);
                    var screenB = Console.ReadLine();
                    if (Int32.TryParse(screenB, out var screenNumB))
                    {
                        if (screenNumB < 1 || screenNumB > 5)
                        {
                            Console.WriteLine("Invalid numeral entered. Please enter a numeral in range 1-5");
                            return;
                        }

                        Sched.SwapMovies(screenNumA, screenNumB);

                    }
                    else
                    {
                        Console.WriteLine("Invalid numeral entered. Please enter a numeral in range 1-5");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid numeral entered. Please enter a numeral in range 1-5");
                }
            }
            else
            {
                Console.WriteLine("Cannot modify schedule while sales are open");
            }

        }

        public void ScheduleMovie()
        {
            if (!Open)
            {
                Sched.PrintSchedule();
                Console.WriteLine("Please Enter A Screen Number:");
                var screenA = Console.ReadLine();
                if (Int32.TryParse(screenA, out var screenNumA))
                {
                    if (screenNumA < 1 || screenNumA > 5)
                    {
                        Console.WriteLine("Invalid numeral entered. Please enter a numeral in range 1-5");
                        return;
                    }

                    Console.WriteLine("Enter Film Title to show at screen {0}", screenNumA);
                    var title = Console.ReadLine();
                    Sched.AddMovie(title, screenNumA);
                }
                else
                {
                    Console.WriteLine("Invalid numeral entered. Please enter a numeral in range 1-5");
                }
            }
            else
            {
                Console.WriteLine("Cannot modify schedule while sales are open");
            }
        }

        public void DeScheduleMovie()
        {
            if (!Open)
            {
                Sched.PrintSchedule();
                Console.WriteLine("Please Enter A Screen Number:");
                var screenA = Console.ReadLine();
                if (Int32.TryParse(screenA, out var screenNumA))
                {
                    if (screenNumA < 1 || screenNumA > 5)
                    {
                        Console.WriteLine("Invalid numeral entered. Please enter a numeral in range 1-5");
                        return;
                    }

                    Sched.RemoveMovie(screenNumA);
                }
                else
                {
                    Console.WriteLine("Invalid numeral entered. Please enter a numeral in range 1-5"); 
                    
                }
            }
            else
            {
                Console.WriteLine("Cannot modify schedule while sales are open");
            }
        }

        public void OpenSales()
        {
            Open = true;
        }


        private void SendMsg(NetworkStream s, string msg)
        {
            var encoded = System.Text.Encoding.ASCII.GetBytes(msg);
            s.Write(encoded);
        }

        private string ReceiveMsg(NetworkStream s)
        {
            var buff = new byte[1024];
            var k = s.Read(buff);
            var ans = "";
            for (int i = 0; i < k; i++)
            {
                ans += Convert.ToChar(buff[i]);
            }

            return ans;
        }
        
        public void PurchaseTickets(NetworkStream s)
        {
            if (Open)
            {
                SendMsg(s,"Please enter a screen number (1-5):");
                var screenA = ReceiveMsg(s);
                if (Int32.TryParse(screenA, out var screenNumberA))
                {
                    if (screenNumberA < 1 || screenNumberA > 5)
                    {
                        SendMsg(s,"Invalid numeral entered. Please enter a numeral in range 1-5");
                        return;
                    }

                    if (Sched.IsNull(screenNumberA))
                    {
                        SendMsg(s,"Currently no showings at given screen");
                        return;
                    }

                    var screen = Sched.GetScreen(screenNumberA);
                    var title = screen.Title;
                    var remaining = screen.Tickets.ToString();
                    
                    var msg = title + " currently has " + remaining + " ticket(s) available (>>)";
                    SendMsg(s, msg);
                    SendMsg(s,"Please enter the number of tickets you would like to purchase:");
                    var tickets = ReceiveMsg(s);
                    if (Int32.TryParse(tickets, out var numTickets))
                    {
                        if (numTickets < 1  || numTickets > screen.Tickets)
                        {
                            msg = "Invalid numeral entered. Please enter a numeral in range 1-" + remaining;
                            SendMsg(s, msg);
                            return;
                        }

                        Purchased += screen.Purchase(numTickets);

                    }
                    else
                    {
                        msg = "Invalid numeral entered. Please enter a numeral in range 1-" + remaining;
                        SendMsg(s, msg);
                    }

                }
                else
                {
                    SendMsg(s, "Invalid numeral entered. Please enter a numeral in range 1-5");
                }
            }
            else
            {
                SendMsg(s,"Sales are not yet open, please try again later");
            }
        }

        public void EndDay()
        {
            Open = false;
            Console.WriteLine("Today's Sales:");
            Console.WriteLine("Total tickets purchased: {0}", Purchased);
            Sched.PrintResults();
        }

        public void Print()
        {
            Sched.PrintSchedule();
        }
        
    }
}