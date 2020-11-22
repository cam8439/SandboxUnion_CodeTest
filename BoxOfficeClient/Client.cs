// File: Client.cs
// Author: Claire Murray
// Client code for purchasing tickets
using System;
using System.Net.Sockets;

namespace BoxOfficeClient
{
    
    /// <summary>
    /// Client: Creates a TCP client that connects to the admin server
    /// </summary>
    public static class Client
    {
        /// <summary>
        /// Entry point
        /// Connects to the server and continually reads and sends messages until an end signal is read
        /// </summary>
        public static void Main() {
            try {
                
                //Connect to the server
                var tcpClient = new TcpClient();
                Console.WriteLine("Connecting.....");
            
                tcpClient.Connect("127.00.00.1",8888);

                Console.WriteLine("Connected");

                var stm = tcpClient.GetStream();
                var buff = new byte[tcpClient.ReceiveBufferSize];

                //Alternate Reading and writing messages until an exit signal is read in
                while (true)
                {

                    var k = stm.Read(buff);
                    var str = "";
                    for (var i=0; i<k; i++)
                    {
                        Console.Write(Convert.ToChar(buff[i]));
                        str += Convert.ToChar(buff[i]);
                    }
                    if(str.Equals("END")) break;

                    var ans = Console.ReadLine();
                    var encoded = System.Text.Encoding.ASCII.GetBytes(ans);
                    stm.Write(encoded);
           
                }
                
                //Manage resources and wrap up

                tcpClient.Close();
            }
        
            catch (Exception e) {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

    }
}