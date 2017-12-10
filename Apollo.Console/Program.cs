using System;
using Apollo;

namespace Apollo.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            MicroServiceManage.Run();

            var restul =  MicroServiceFactory<IServerDemo>.Instance.Hello("12");
            System.Console.WriteLine(restul);
        }
    }
}
