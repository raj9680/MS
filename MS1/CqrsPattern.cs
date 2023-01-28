using Ninject;
using System;
using System.Collections.Generic;
using System.Text;

namespace MS1
{
    #region CQRS without Events
    /*
    #region Step 1
    public interface ICommand
    {
        // Common things which is required in performing handler operation forex: we need Id to perform crud operation. so
        // public int IdCommon { get; set; }
        // public string Guid { get; set; }     // To uniquely identify the record.
    }

    public interface IDispatcher
    {
        void Send<T>(T Command) where T : ICommand; // Here generics at method level
    }

    public interface ICommandHandler<T> where T : ICommand  // Here generics at class level
    {
        void Handle(T Command);
    }
    #endregion Step 1

    #region Step 2
    public class CreateCustomerHandler : ICommandHandler<CreateCustomer>
    {
        public void Handle(CreateCustomer Command)
        {
            Console.WriteLine(Command.Name + " Inserted into DB using EF");
            // Event Queue
        }
    }
    public class CustomerDispatcher : IDispatcher
    {
        public void Send<T>(T Command) where T : ICommand
        {
            var handler = Program.kernel.Get<ICommandHandler<T>>();
            handler.Handle(Command);
        }
    }

    #endregion Step 2
    */
    #endregion



    #region CQRS with Events

    #region Step 1
    public interface ICommand
    {
        // Common things which is required in performing handler operation forex: we need Id to perform crud operation. so
        // public int IdCommon { get; set; }
        // public string Guid { get; set; }     // To uniquely identify the record.
    }

    public interface IDispatcher
    {
        void Send<T>(T Command) where T : ICommand; // Here generics at method level
    }

    public interface ICommandHandler<T> where T : ICommand  // Here generics at class level
    {
        void Handle(T Command);
    }
    #endregion Step 1

    #region Step 2
    public class CreateCustomerHandler : ICommandHandler<CreateCustomer>
    {
        public void Handle(CreateCustomer Command)
        {
            Console.WriteLine(Command.Name + " Inserted into DB using EF");
            // Event Queue
        }
    }
    public class CustomerDispatcher : IDispatcher
    {
        public void Send<T>(T Command) where T : ICommand
        {
            var handler = Program.kernel.Get<ICommandHandler<T>>();
            handler.Handle(Command);
        }
    }

    #endregion Step 2

    #endregion
}
