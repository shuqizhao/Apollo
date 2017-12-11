using System;
using System.Threading;
using Apollo;
using Apollo.DemoInterface;

namespace Apollo.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("CONSUL_HTTP_ADDR", "127.0.0.1:8500");

            MicroServiceManage.Run();

            while (true)
            {
                var restul = MicroServiceFactory<IServerDemo>.Instance.Hello("12");
                System.Console.WriteLine(restul);

                var person = MicroServiceFactory<IServerDemo>.Instance.GetPersonById(1, true);
                if (person != null)
                {
                    System.Console.WriteLine(person.Name);

                }

                MicroServiceFactory<IServerDemo>.Instance.SavePerson(new Person{Id=125,Name="shuqizhao"}, false);

                Thread.Sleep(1000);
            }
        }
    }
}
