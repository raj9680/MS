using MS1;
using Ninject;
using Ninject.Modules;
using System;
using System.Reflection;

namespace MS1
{
    class Program
    {
        static public IKernel kernel = new StandardKernel();    // Ninject register kernel
        static void Main(string[] args)
        {

            #region Interaction for CQRS with Events
            
            kernel.Load(Assembly.GetExecutingAssembly());   // Ninject to perform lookups

            CreateCustomer newCustomer = new CreateCustomer();
            newCustomer.Name = "Raj";

            IDispatcher dispatcher = new CustomerDispatcher();
            dispatcher.Send<CreateCustomer>(newCustomer);
            
            #endregion
        }
    }


    #region Ninject for handling CQRS mappings
    public class Binding : NinjectModule
    {
        public override void Load()
        {
            Bind(typeof(ICommandHandler<CreateCustomer>)).
                To(typeof(CreateCustomerHandler));
        }
    }

    #endregion



    /* DTO vs Entity
     * DTO can only have set; and get; while DTO can have business logic i.e validation, aggregate counts etc.
     */

    public class Customer  // Entity classes
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    class Product   // Entity classes
    {

    }
    class MyDate    // Value object classes
    {
        public string Month { get; set; }
        public string Year { get; set; }

        public override bool Equals(object obj)
        {
            if(this.Month == ((MyDate)obj).Month)
                return true;
            return false;
        }
    }
    class Repository   // Service classes
    {
        
    }
    class Logger     // Service classes
    {

    }



    #region For CQRS without Events

    public class CreateCustomer : Customer, ICommand
    {
        public DateTime CreatedDate { get; set; }
    }

    public class UpdateCustomer : Customer, ICommand
    {
        public string UpdatedBy { get; set; }
    }

    #endregion

}
