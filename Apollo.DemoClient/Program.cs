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
                var restul = MicroServiceFactory<IServerDemo>.Instance.Hello("Hello_string");
                System.Console.WriteLine(restul);

                // var person = MicroServiceFactory<IServerDemo>.Instance.GetPersonById(1, true);
                // if (person != null)
                // {
                //     System.Console.WriteLine(person.Name);

                // }

                // MicroServiceFactory<IServerDemo>.Instance.SavePerson(new Person { Id = 125, Name = "shuqizhao" }, false);

                // var hello = MicroServiceFactory<IServerDemo>.Instance.Hello();

                // System.Console.WriteLine(hello);

                // var person = MicroServiceFactory<IValues>.Instance.Hello();

                // System.Console.WriteLine(person.Name);
                Thread.Sleep(1000);
            }
        }
    }
}
