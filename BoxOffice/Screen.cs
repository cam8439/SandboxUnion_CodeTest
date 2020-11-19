using System;

namespace BoxOffice
{
    public class Screen
    {
        private readonly object _lock = new object();
        public string Title { get; }
        public int Tickets { get; private set; }
        private int Purchased { get; set; }

        public Screen(string title)
        {
            this.Title = title;
            this.Tickets = 60;
            this.Purchased = 0;
        }

        public bool Equals(Screen other)
        {
            return String.Equals(Title, other.Title);
        }

        public void PrintInfo()
        {
            Console.WriteLine("----------------------------");
            Console.WriteLine("    Currently Showing: {0}", Title);
            Console.WriteLine("    Tickets Available: {0}", Tickets);
            Console.WriteLine("-----------------------------");
        }

        
        /// <summary>
        ///
        /// Precondition: Number of tickets is > 0
        /// </summary>
        /// <param name="numTickets"></param>
        public int Purchase(int numTickets)
        {
            lock (_lock)
            {
                if (numTickets > Tickets)
                {
                    Console.WriteLine(
                        "The number of tickets you entered is greater than the current available maximum");
                    Console.WriteLine("Please try again");
                    return 0;
                }
                else
                {
                    Tickets -= numTickets;
                    Purchased += numTickets;
                    return numTickets;
                }
            }
        }

        public void PrintSummary()
        {
            Console.WriteLine("Movie Shown: {0}", Title);
            Console.WriteLine("Tickets Sold: {0}", Purchased);
            Console.WriteLine("Tickets Remaining: {0}", Tickets);
        }
    }
}