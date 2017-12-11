using System;
using System.Threading;
using Apollo.DemoInterface;

namespace Apollo.DemoClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("CONSUL_HTTP_ADDR", "127.0.0.1:8500");
            while (true)
            {
                var restul = MicroServiceFactory<IServerDemo>.Instance.Hello("12");
                System.Console.WriteLine(restul);
                Thread.Sleep(1000);
            }
        }
    }
}
