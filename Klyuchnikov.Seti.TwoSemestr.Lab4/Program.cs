using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Klyuchnikov.Seti.TwoSemestr.Lab4
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                do
                {
                    Console.WriteLine("Klyuchnikov Dmitry - Telnet");
                    var newstr = Console.ReadLine();
                    var arr = newstr.Split(' ');
                    if (arr.Length > 0)
                        if (arr[0] == "open")
                        {
                            var address = arr[1];
                            var port = arr[2];
                            Client.Current.ConnectAsync(address, int.Parse(port));
                            do
                            {

                                var str = Console.ReadLine();
                                if (str != "quit")
                                    Client.Current.SendAsync(str);
                                else
                                {
                                    Client.Current.Close();
                                    Console.WriteLine("Disconnect to " + address + "...");
                                    //    Client.Current.SendBytes(new byte[] { });
                                }
                                if (!Client.Current.IsConnected)
                                    break;
                            } while (true);
                        }

                } while (true);
            }
            catch (Exception exception)
            {

            }

        }
    }
}
