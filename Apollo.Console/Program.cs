using System;
using Apollo;

namespace Apollo.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("CONSUL_HTTP_ADDR", "172.17.40.26:8500");

            MicroServiceManage.Run();

            //var restul =  MicroServiceFactory<IServerDemo>.Instance.Hello("12");
            //System.Console.WriteLine(restul);
            System.Console.Read();
        }
    }
}
