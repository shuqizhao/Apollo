using System;
using Apollo;

namespace Apollo.DemoInterface
{
    [MicroService]
    public interface IServerDemo
    {
        string Hello(string input);

        string Hello1();

        Person GetPersonById(int id, bool isAdmin);

        void SavePerson(Person person, bool isAdmin);
    }
}
