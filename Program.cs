using System;

namespace web_server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start server on port :80 ...");
            new Server();
        }
    }
}
