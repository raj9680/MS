using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace RepositoryPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            Customer obj = new Customer();
            IRepository<Customer> repo = null;
            repo.Add(obj);

            Supplier obj1 = new Supplier();
            IRepository<Supplier> repo1 = null;
            repo1.Add(obj1);
        }
    }

    public class Customer
    {
        public string name { get; set; }
    }
    public class Supplier
    {
        public string name { get; set; }
    }

    interface IRepository<T>
    {
        bool Add(T obj);
        bool Update(T obj);
        List<T> Query(int id);
        List<T> Query(string name);
    }

    public abstract class EfCommon<T> : DbContext, IRepository<T>
        where T: class
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

    public class EfDal: EfCommon<Customer>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("localhost"); // should be in conf
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mappings
            base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<Customer>().ToTable("tblCustomer");
            //modelBuilder.Entity<Supplier>().ToTable("tblSupplier");
            // recommened to have separate mapping codes instead to map here. - X
        }
    }

    #region Separate Mappings
    public class EfCustomerContext : EfCommon<Customer>
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().ToTable("tblCustomer"); // -X
        }
    }
    public class EfSupplierContext : EfCommon<Supplier>
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Supplier>().ToTable("tblCustomer"); // -X
        }
    }
    #endregion
}
