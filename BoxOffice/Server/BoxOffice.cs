// File: BoxOffice.cs
// Author: Claire Murray
// Model Code for handling various Box Office tasks
using System;
using System.IO;
using System.Net.Sockets;

namespace BoxOffice.Server
{
    /// <summary>
    /// BoxOffice: Class for dealing with schedule change requests and ticket purchases
    /// </summary>
    public class BoxOffice
    {
        /// <summary>
        /// Schedule: Utility class for keeping track of the theater's screens 
        /// </summary>
        private class Schedule
        {
            /// <summary> Internal storage for the theater's Screens /// </summary>
            private readonly Screen[] _screens = new Screen[5];
            
            /// <summary> Represents a screen with no movie being shown /// </summary>
            private static readonly Screen NullScreen = new Screen("");

            /// <summary>
            /// Schedule Constructor
            /// </summary>
            public Schedule()
            {
                for (int i = 0; i < 5; i++)
                {
                    _screens[i] = NullScreen;
                }
            }

            /// <summary>
            /// Swaps the position of two Screens in the list
            /// Precondition: Both screen numbers are in the range 1-5
            /// </summary>
            /// <param name="screenA"> Index of first Screen to swap </param>
            /// <param name="screenB"> Index of second Screen to swap </param>
            public void SwapMovies(int screenA, int screenB)
            {
                var temp = _screens[screenA - 1];
                _screens[screenA - 1] = _screens[screenB - 1];
                _screens[screenB - 1] = temp;
            }

            /// <summary>
            /// Add a new movie to the schedule. Overwrites whatever was in its place
            /// </summary>
            /// <param name="title"> Title of the new movie to add </param>
            /// <param name="screenNumber"> Index of the Screen to replace </param>
            public void AddMovie(string title, int screenNumber)
            {
                _screens[screenNumber - 1] = new Screen(title);
            }

            /// <summary>
            /// Removes a movie from the schedule, replacing it with NullScreen
            /// </summary>
            /// <param name="screenNumber"> Index of the Screen to be replaced </param>
            public void RemoveMovie(int screenNumber)
            {
                _screens[screenNumber - 1] = NullScreen;
            }

            /// <summary>
            /// Checks if there is a movie being shown at a given screen
            /// </summary>
            /// <param name="screenNumber"> The index of the screen to check </param>
            /// <returns> True, if the movie at the screenIndex is NullScreen, False otherwise </returns>
            public bool IsNull(int screenNumber)
            {
                return _screens[screenNumber - 1].Equals(NullScreen);
            }

            /// <summary>
            /// Gets the Screen at a given index
            /// </summary>
            /// <param name="screenNumber"> Index of the Screen to return </param>
            /// <returns> The Screen at the given index </returns>
            public Screen GetScreen(int screenNumber)
            {
                return _screens[screenNumber - 1];
            }

            /// <summary>
            /// Prints out the theater's schedule
            /// </summary>
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

            /// <summary>
            /// Prints a summary of how each screen performed
            /// </summary>
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

        /// <summary> Represents the theater's schedule /// </summary>
        private Schedule Sched { get; }
        
        /// <summary> Tracks whether sales have been opened yet /// </summary>
        public bool Open { get; private set; }
        
        /// <summary> Tracks total number of tickets purchased /// </summary>
        private int Purchased { get; set; }

        /// <summary>
        /// BoxOffice Constructor
        /// </summary>
        public BoxOffice()
        {
            Sched = new Schedule();
            Open = false;
            Purchased = 0;
        }

        /// <summary>
        /// Procedure to swap movies from the command line
        /// </summary>
        public void SwapMovies()
        {
            //Do not allow schedule changes while sales are open
            if (!Open)
            {
                //Read in Screen number A
                Sched.PrintSchedule();
                Console.WriteLine("Please Enter A Screen Number:");
                var screenA = Console.ReadLine();
                //Check Validity
                if (int.TryParse(screenA, out var screenNumA))
                {
                    //Check in range
                    if (screenNumA < 1 || screenNumA > 5)
                    {
                        Console.WriteLine("Invalid numeral entered. Please enter a numeral in range 1-5");
                        return;
                    }

                    //Read in Screen number B
                    Console.WriteLine("Please enter screen number to swap with screen {0}", screenNumA);
                    var screenB = Console.ReadLine();
                    //Check validity
                    if (int.TryParse(screenB, out var screenNumB))
                    {
                        //Check in range
                        if (screenNumB < 1 || screenNumB > 5)
                        {
                            Console.WriteLine("Invalid numeral entered. Please enter a numeral in range 1-5");
                            return;
                        }

                        //Perform Swap
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

        /// <summary>
        /// Procedure to add movies from the command line
        /// </summary>
        public void ScheduleMovie()
        {
            //Do not allow schedule modifications while sales are open
            if (!Open)
            {
                //Read in screen number
                Sched.PrintSchedule();
                Console.WriteLine("Please Enter A Screen Number:");
                var screenA = Console.ReadLine();
                //Check validity
                if (int.TryParse(screenA, out var screenNumA))
                {
                    //Check in range
                    if (screenNumA < 1 || screenNumA > 5)
                    {
                        Console.WriteLine("Invalid numeral entered. Please enter a numeral in range 1-5");
                        return;
                    }

                    //Read in title
                    Console.WriteLine("Enter Film Title to show at screen {0}", screenNumA);
                    var title = Console.ReadLine();
                    
                    //Modify schedule
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

        /// <summary>
        /// Procedure to remove movies from the command line
        /// </summary>
        public void DeScheduleMovie()
        {
            //Do not allow schedule modifications while sales are open
            if (!Open)
            {
                //Read in screen number
                Sched.PrintSchedule();
                Console.WriteLine("Please Enter A Screen Number:");
                var screenA = Console.ReadLine();
                
                //Check validity
                if (int.TryParse(screenA, out var screenNumA))
                {
                    //Check in range
                    if (screenNumA < 1 || screenNumA > 5)
                    {
                        Console.WriteLine("Invalid numeral entered. Please enter a numeral in range 1-5");
                        return;
                    }

                    //Perform operation
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

        /// <summary>
        /// Procedure to open sales from the command line
        /// </summary>
        public void OpenSales()
        {
            Open = true;
        }


        /// <summary>
        /// Method to send a message to a TCP client
        /// Used by PurchaseTickets() to communicate with the customer
        /// </summary>
        /// <param name="s"> The network stream connected to the client </param>
        /// <param name="msg"> the message to send to the client </param>
        private static void SendMsg(Stream s, string msg)
        {
            var encoded = System.Text.Encoding.ASCII.GetBytes(msg);
            s.Write(encoded);
        }

        /// <summary>
        /// Method to receive a message from a TCP client
        /// Used by PurchaseTickets() to communicate with the customer
        /// </summary>
        /// <param name="s"> The network stream connected to the client </param>
        /// <returns> The decoded message from the client </returns>
        private static string ReceiveMsg(Stream s)
        {
            var buff = new byte[1024];
            var k = s.Read(buff);
            var ans = "";
            for (var i = 0; i < k; i++)
            {
                ans += Convert.ToChar(buff[i]);
            }

            return ans;
        }
        
        /// <summary>
        /// Procedure to purchase tickets from the command line
        /// (Used by client)
        /// </summary>
        /// <param name="s"> The network stream connected to the client </param>
        public void PurchaseTickets(NetworkStream s)
        {
            //Do not allow purchases when sales are not open
            if (Open)
            {
                //Read in screen number
                SendMsg(s,"Please enter a screen number (1-5):");
                var screenA = ReceiveMsg(s);
                
                //Check validity
                if (int.TryParse(screenA, out var screenNumberA))
                {
                    //Check in range
                    if (screenNumberA < 1 || screenNumberA > 5)
                    {
                        SendMsg(s,"Invalid numeral entered. Please enter a numeral in range 1-5");
                        return;
                    }

                    //Check if there is a film playing
                    if (Sched.IsNull(screenNumberA))
                    {
                        SendMsg(s,"Currently no showings at given screen");
                        return;
                    }

                    //Get film info
                    var screen = Sched.GetScreen(screenNumberA);
                    var title = screen.Title;
                    var remaining = screen.Tickets.ToString();
                    
                    //Determine amount of purchase
                    var msg = title + " currently has " + remaining + " ticket(s) available (>>)";
                    SendMsg(s, msg);
                    SendMsg(s,"Please enter the number of tickets you would like to purchase:");
                    var tickets = ReceiveMsg(s);
                    
                    //Check validity
                    if (int.TryParse(tickets, out var numTickets))
                    {
                        //Check in range
                        if (numTickets < 1  || numTickets > screen.Tickets)
                        {
                            msg = "Invalid numeral entered. Please enter a numeral in range 1-" + remaining;
                            SendMsg(s, msg);
                            return;
                        }

                        //Perform operation
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

        /// <summary>
        /// Closes Sales and prints out a summary of the day's sales
        /// </summary>
        public void EndDay()
        {
            Open = false;
            Console.WriteLine("Today's Sales:");
            Console.WriteLine("Total tickets purchased: {0}", Purchased);
            Sched.PrintResults();
        }

        /// <summary>
        /// Used to print the theater's schedule from a separate class
        /// </summary>
        public void Print()
        {
            Sched.PrintSchedule();
        }
        
    }
}