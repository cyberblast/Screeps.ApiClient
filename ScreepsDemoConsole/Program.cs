using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreepsApi;

namespace ScreepsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("This is only a demo (yet)");
            Console.WriteLine("Press \"Ctrl + C\" anytime to quit");
            Console.WriteLine();
            Console.Write("Please enter your screeps account email: ");
            string email = Console.ReadLine();
            Console.Write("Please enter your screeps account password: ");
            string password = Console.ReadLine();
            Console.WriteLine();

            Client client = new Client(email, password);
            
            while (true)
            {
                dynamic me = client.Me();
                Console.Write("User {0}, GCL: {1} ", me.username, me.gcl);
                for (int i = 0; i<6; i++)
                {
                    System.Threading.Thread.Sleep(1000);
                    Console.Write(".");
                }
                Console.WriteLine();
            }         
        }
    }
}
