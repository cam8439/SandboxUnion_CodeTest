using System;
using System.Net.Sockets;
using System.Threading;

namespace BoxOffice.Server
{
    internal static class Admin
    {
        private static readonly BoxOffice Office = new BoxOffice();

        private static void Main()
        {
            var ip = System.Net.IPAddress.Loopback;
            var serverSocket = new TcpListener(ip , 8888);
            var clientSocket = default(TcpClient);

            serverSocket.Start();
            Console.WriteLine(" >> " + "Server Started");
            
            var adminThread = new Thread(AdminLoop);
            adminThread.Start();
            while (!Office.Open) {}

            while (Office.Open)
            {
                if (serverSocket.Pending())
                {
                    clientSocket = serverSocket.AcceptTcpClient();
                    var client = new HandleClient();
                    client.StartClient(clientSocket);
                }
            }
            Console.WriteLine("Waiting for all clients to disconnect...");
            adminThread.Join();
            clientSocket?.Close();
            serverSocket.Stop();
            Console.WriteLine(" >> " + "exit");
        }

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
        private static void AdminLoop()
        {
            while (true)
            {
                Console.Write(" >>");
                var command = Console.ReadLine();
                if (command != null)
                {
                    command = command.ToLower();
                    command = command.Trim();
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

        private class HandleClient
        {
            private TcpClient _clientSocket;
            
            public void StartClient(TcpClient inClientSocket)
            {
                this._clientSocket = inClientSocket;
                var ctThread = new Thread(Communicate);
                ctThread.Start();
                ctThread.Join();
            }
            private void Communicate()
            {
                using var s =_clientSocket.GetStream();
                Office.PurchaseTickets(s);
                var endCode = System.Text.Encoding.ASCII.GetBytes("END");
                s.Write(endCode);
            }
        } 
        
    }
}