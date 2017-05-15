using System;
using ScreepsApi;
using System.Collections.Generic;

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
                /*
                // testing code upload/download
                dynamic codeSet = client.CodeSet("test", new Dictionary<string, string> 
                {
                    {"main", "dummy"}
                });
                Console.WriteLine("CodeSet: {0} ", client.js.Serialize(codeSet));

                dynamic codeGet = client.CodeGet("test");
                Console.WriteLine("CodeGet: {0} ", client.js.Serialize(codeGet));
                */

                /*
                 * // testing memory access
                dynamic memory = client.UserMemoryGet("creeps");
                Console.WriteLine("memory: {0} ", client.js.Serialize(memory.data));
                 */

                dynamic me = client.Me();
                Console.Write("User {0}, GCL: {1} ", me.username, me.gcl);
                for (int i = 0; i<5; i++)
                {
                    System.Threading.Thread.Sleep(1000);
                    Console.Write(".");
                }
                Console.WriteLine();
            }         
        }
    }
}
