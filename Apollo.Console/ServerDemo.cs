using System;
using Apollo.DemoInterface;

namespace Apollo.Console
{

    public class ServerDemo : IServerDemo
    {
        public string Hello(string input)
        {
            System.Console.WriteLine("you input is "+input);
            return "I get it，乱码？";
        }

        public Person GetPersonById(int id, bool isAdmin)
        {
            System.Console.WriteLine("you input is " + id + ":" + isAdmin);

            return new Person { Id = 1, Name = "xyz", Sex = "F" };
        }

        public void SavePerson(Person person, bool isAdmin)
        {
            
        }

        public string Hello()
        {
            return "Hello Word";
        }
    }
}
