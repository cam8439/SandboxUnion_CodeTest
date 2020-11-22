using System;
using System.Net.Sockets;

namespace BoxOfficeClient
{
    public static class Client
    {
        public static void Main() {
            try {
                TcpClient tcpClient = new TcpClient();
                Console.WriteLine("Connecting.....");
            
                tcpClient.Connect("127.00.00.1",8888);

                Console.WriteLine("Connected");

                var stm = tcpClient.GetStream();
                var buff = new byte[tcpClient.ReceiveBufferSize];

                while (true)
                {

                    var k = stm.Read(buff);
                    var str = "";
                    for (int i=0;i<k;i++)
                    {
                        Console.Write(Convert.ToChar(buff[i]));
                        str += Convert.ToChar(buff[i]);
                    }
                    if(str.Equals("END")) break;

                    var ans = Console.ReadLine();
                    var encoded = System.Text.Encoding.ASCII.GetBytes(ans);
                    stm.Write(encoded);
           
                }

                tcpClient.Close();
            }
        
            catch (Exception e) {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

    }
}