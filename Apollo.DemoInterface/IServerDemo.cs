using System;
using Apollo;

namespace Apollo.DemoInterface
{
    [MicroService(Name = "MyApollo", Port = 8081)]
    public interface IServerDemo
    {
        [MicroServiceMethod(Name = "helloDaiIput")]
        string Hello(string input);

        string Hello();

        Person GetPersonById(int id, bool isAdmin);

        void SavePerson(Person person, bool isAdmin);
    }
}
