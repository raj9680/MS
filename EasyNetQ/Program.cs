using System;
using EasyNetQ;

namespace EasyNetQ
{
    class Program
    {
        static void Main(string[] args)
        {
            Customer x = new Customer();
            x.Name = "Created Message";
            var bus = RabbitHutch.CreateBus("host=localhost");
            bus.Publish<Customer>(x);
            bus.Subscribe<Customer>(
                "my_subscription_id", obj => Console.WriteLine(obj.Name)
                );

            Console.WriteLine("Hello World");

        }
    }


    public class Customer
    {
        public string Name { get; set; }
    }
}
