using System;
using Apollo.DemoInterface;

namespace Apollo.DemoService
{

    public class ServerDemo : IServerDemo
    {
        public string Hello(string input)
        {
            System.Console.WriteLine("Hello you input is " + input);
            return "hello word from c#";
        }

        public Person GetPersonById(int id, bool isAdmin)
        {
            System.Console.WriteLine("GetPersonById you input is " + id + ":" + isAdmin);

            return new Person { Id = 1, Name = "xyz", Sex = "F" };
        }

        public void SavePerson(Person person, bool isAdmin)
        {
            System.Console.WriteLine("SavePerson you input is " + person.Id + ":" + person.Name+":"+isAdmin);

        }

        public string Hello()
        {
            return "Hello Word";
        }
    }
}
