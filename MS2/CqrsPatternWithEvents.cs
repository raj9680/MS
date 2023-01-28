using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MS1
{
    #region Step 1
    public interface ICommand
    {
        // Common things which is required in performing handler operation forex: we need Id to perform crud operation. so
        // public int IdCommon { get; set; }
        public Guid Guid { get; set; }     // To uniquely identify the record.
    }

    public interface IDispatcher
    {
        void Send<T>(T Command) where T : ICommand; // Here generics at method level
        public IEventBus _eventPublisher { get; set; }
    }

    #region Events
    public interface IEvent
    {
        public string Guid { get; set; }
    }

    public class CustomerCreated : IEvent
    {
        public string Guid { get; set; }
        public CustomerCreated(string _guid)
        {
            this.Guid = _guid;
        }
    }

    public interface IEventHandler
    {

    }

    public interface CustomerCreatedEventHandler<TEvent> : IEventHandler where TEvent : IEvent
    {
        void Handle(TEvent tevent);
    }

    public interface IEventHandler<TEvent> : IEventHandler where TEvent: IEvent
    {
        void Handle(TEvent tevent);
    }

    public class CustomerCreatedEventHandler : IEventHandler<CustomerCreated>
    {
        public void Handle(CustomerCreated tevent)
        {
            Console.WriteLine($"User was created {tevent.Guid} - event");
        }
    }

    public interface IEventBus
    {
        void Publish<T>(Guid guid, T @event) where T : IEvent;
        List<IEvent> GetEvents(Guid aggregateid);
        List<IEvent> GetEvents();
    }

    public class EventBus : IEventBus
    {
        IEventStore eventsrc = new EventStore();

        public void Publish<T>(Guid g, T @event) where T : IEvent
        {
            var handler = Program.kernel.Get<IEventHandler<T>>();
            handler.Handle(@event);
            this.eventsrc.SaveEvents(g, @event);
        }

        public List<IEvent> GetEvents(Guid aggregateid)
        {
            return eventsrc.GetEvents(aggregateid);
        }

        public List<IEvent> GetEvents()
        {
            return eventsrc.GetEvents();
        }
        
    }

    public interface IEventStore
    {
        void SaveEvents(Guid aggregateId, IEvent e);
        List<IEvent> GetEvents(Guid aggregateid);
        List<IEvent> GetEvents();
    }

    public class EventStore : IEventStore
    {
        private readonly Dictionary<Guid, List<IEvent>> _eventstore = new Dictionary<Guid, List<IEvent>>();
        
        public List<IEvent> GetEvents(Guid aggregateid)
        {
            return _eventstore[aggregateid];
        }

        public List<IEvent> GetEvents()
        {
            return _eventstore.SelectMany(d => d.Value).ToList();
        }

        public void SaveEvents(Guid aggregateId, IEvent e)
        {
            List<IEvent> events = null;
            if (!_eventstore.ContainsKey(aggregateId))
            {
                events = new List<IEvent>();
                _eventstore.Add(aggregateId, events);
            }
            else
            {
                events = _eventstore[aggregateId];
            }
            events.Add(e);
        }
    }

    #endregion

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
            // Repo code goes here.
            IRepository<Customer> repo = new EfCustomerContext();
            Customer x = new Customer();
            x.Id = Command.Id;
            x.Name = Command.Name;

            //var mapper = new Mapper(Program.mappConfig);
            //Customer x = mapper.Map<Customer>(Command);

            repo.Add(x);

            Console.WriteLine(Command.Name + " Inserted into DB using EF");
            // Event Queue, kafka/rabbitMq
        }
    }

    public class CustomerDispatcher : IDispatcher
    {
        public IEventBus _eventPublisher { get; set ; }
        public Guid Guid { get; set; }
        public void Send<T>(T Command) where T : ICommand
        {
            var handler = Program.kernel.Get<ICommandHandler<T>>();
            this.Guid = Command.Guid;
            handler.Handle(Command);
            // Calls Repo
            // call eventBus

            _eventPublisher = new EventBus();

            Guid g = new Guid();
            string guid = g.ToString();
            _eventPublisher.Publish(new Guid(), new CustomerCreated(guid));
        }
    }

    #region Repository
    interface IRepository<T>
    {
        bool Add(T obj);
        bool Update(T obj);
        List<T> Query(int id);
        List<T> Query(string name);
    }

    public abstract class EfCommon<T> : DbContext, IRepository<T>
        where T : class
    {
        // DbContext dbContext = null;
        public bool Add(T obj)
        {
            Set<T>().Add(obj);
            return true;
        }

        public List<T> Query(int id)
        {
            throw new NotImplementedException();
        }

        public List<T> Query(string name)
        {
            throw new NotImplementedException();
        }

        public bool Update(T obj)
        {
            throw new NotImplementedException();
        }
    }

    public class EfCustomerContext : EfCommon<Customer>
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().ToTable("tblCustomer"); // -X
        }
    }
    #endregion Repository End

    #endregion Step 2
}
