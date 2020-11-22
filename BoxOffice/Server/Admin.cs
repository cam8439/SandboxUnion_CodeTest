// File: Admin.cs
// Author: Claire Murray
// Runs a server locally to manage the Box Office
using System;
using System.Net.Sockets;
using System.Threading;

namespace BoxOffice.Server
{
    /// <summary>
    /// Admin: Runs a server that manages the Box Office and allows clients to connect and purchase tickets
    /// </summary>
    internal static class Admin
    {
        /// <summary> The server's box office /// </summary>
        private static readonly BoxOffice Office = new BoxOffice();

        /// <summary>
        /// Entry Point
        /// Opens a server, starts the admin thread, and handles client connections
        /// </summary>
        private static void Main()
        {
            //Connection information
            var ip = System.Net.IPAddress.Loopback;
            var serverSocket = new TcpListener(ip , 8888);
            var clientSocket = default(TcpClient);

            serverSocket.Start();
            Console.WriteLine(" >> " + "Server Started");
            
            var adminThread = new Thread(AdminLoop);
            adminThread.Start();
            while (!Office.Open)
            {
                //Wait for sales to open
            }
            
            //Continually loop and wait for client connections
            while (Office.Open)
            {
                if (!serverSocket.Pending()) continue;
                clientSocket = serverSocket.AcceptTcpClient();
                var client = new HandleClient();
                client.StartClient(clientSocket);
            }
            
            //Manage resources and wrap up
            Console.WriteLine("Waiting for all clients to disconnect...");
            adminThread.Join();
            clientSocket?.Close();
            serverSocket.Stop();
            Console.WriteLine(" >> " + "exit");
        }

        /// <summary>
        /// Prints a usage message to the console
        /// </summary>
        private static void Usage()
        {
            Console.WriteLine("help -    Display this message");
            Console.WriteLine("print -   Print the current schedule");
            Console.WriteLine("swap -    Swap the films for two screens");
            Console.WriteLine("add -     Replace the film being shown on one screen");
            Console.WriteLine("remove -  Remove the film being shown on one screen");
            Console.WriteLine("open -    Open sales for the day");
            Console.WriteLine("end -     End sales for the day");
        }
        
        /// <summary>
        /// Method used by the admin thread
        /// Loops handling a few basic commands until the user ends the day's sales
        /// </summary>
        private static void AdminLoop()
        {
            while (true)
            {
                //Read message
                Console.Write(" >>");
                var command = Console.ReadLine();
                if (command != null)
                {
                    //String management
                    command = command.ToLower();
                    command = command.Trim();
                    
                    //Handle command
                    switch (command)
                    {
                        case "help":
                            Usage();
                            break;
                        case "open":
                            Office.OpenSales();
                            break;
                        case "swap":
                            Office.SwapMovies();
                            break;
                        case "add":
                            Office.ScheduleMovie();
                            break;
                        case "remove":
                            Office.DeScheduleMovie();
                            break;
                        case "end":
                            Office.EndDay();
                            return;
                        case "print":
                            Office.Print();
                            break;
                        default:
                            Console.WriteLine("Unrecognized command. Type \"help\" for list of valid commands.");
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// HandleClient: Class for handling client requests separately
        /// </summary>
        private class HandleClient
        {
            /// <summary> The socket that this client is connected to /// </summary>
            private TcpClient _clientSocket;
            
            /// <summary>
            /// Starts a new thread for a client
            /// </summary>
            /// <param name="inClientSocket"> The socket that this client is connected to </param>
            public void StartClient(TcpClient inClientSocket)
            {
                this._clientSocket = inClientSocket;
                var ctThread = new Thread(Communicate);
                ctThread.Start();
                ctThread.Join();
            }
            
            /// <summary>
            /// Method used by client threads for purchasing tickets
            /// </summary>
            private void Communicate()
            {
                using var s =_clientSocket.GetStream();
                Office.PurchaseTickets(s);
                
                //Tell the client to exit
                var endCode = System.Text.Encoding.ASCII.GetBytes("END");
                s.Write(endCode);
            }
        } 
        
    }
}