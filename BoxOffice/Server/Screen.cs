// File: Screen.cs
// Author: Claire Murray
// Part of the Model code, to represent a screen that shows a film
using System;

namespace BoxOffice.Server
{
    /// <summary>
    /// Screen: Represents a Screen in the theatre with a Title and a certain number of available tickets
    /// </summary>
    public class Screen
    {
        /// <summary> Lock to protect the critical region (purchasing tickets) /// </summary>
        private readonly object _lock = new object();
        
        /// <summary> The title of the film being shown at this screen  /// </summary>
        public string Title { get; }
        
        /// <summary> Total number of tickets remaining for this film /// </summary>
        public int Tickets { get; private set; }
        
        /// <summary> Total number of tickets that have been purchased for this film /// </summary>
        private int Purchased { get; set; }

        /// <summary>
        /// Screen constructor
        /// </summary>
        /// <param name="title"> The title of the film being shown at this screen </param>
        public Screen(string title)
        {
            this.Title = title;
            this.Tickets = 60;
            this.Purchased = 0;
        }

        /// <summary>
        /// Determines equality between two Screens
        /// Two Screens are Equal if they have the same title
        /// </summary>
        /// <param name="other"> Screen that is being compared to the calling Screen object </param>
        /// <returns> True, if the screens have the same Title, False otherwise </returns>
        public bool Equals(Screen other)
        {
            return String.Equals(Title, other.Title);
        }


        /// <summary>
        /// "Purchase" tickets for this screen
        /// Precondition: Number of tickets is > 0
        /// </summary>
        /// <param name="numTickets"> The number of tickets being purchased </param>
        /// <returns> The number of tickets successfully purchased </returns>
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

        /// <summary>
        /// Print out a summary of how this screen performed
        /// </summary>
        public void PrintSummary()
        {
            Console.WriteLine("Movie Shown: {0}", Title);
            Console.WriteLine("Tickets Sold: {0}", Purchased);
            Console.WriteLine("Tickets Remaining: {0}", Tickets);
        }
    }
}