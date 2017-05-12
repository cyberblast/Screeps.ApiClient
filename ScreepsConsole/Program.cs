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
            Console.Write("Please enter your screeps account email: ");
            string email = Console.ReadLine();
            Console.Write("Please enter your screeps account password: ");
            string password = Console.ReadLine();
            Console.WriteLine("Press \"Ctrl + C\" anytime to quit");

            Client client = new Client(email, password);
            //Console.WriteLine("Token: " + client.token);
            
            while (true)
            {
                dynamic me = client.Me();
                Console.WriteLine("User {0}, GCL: {1}, credits: {2}", me.username, me.gcl, me.credits);
                System.Threading.Thread.Sleep(10000);
            }         
        }
    }
}
