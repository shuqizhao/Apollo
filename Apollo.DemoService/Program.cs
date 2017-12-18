using System;

namespace Apollo.DemoService
{
    class Program
    {
        static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("CONSUL_HTTP_ADDR", "127.0.0.1:8500");
            MicroServiceApplication.Run();
            System.Console.Read();
        }
    }
}
