using Libanon.Models.EntityConfiguration;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Libanon.Models
{
    public class BookContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<ISBN> ISBNs { get; set; }
        public DbSet<User> Users { get; set; }

        public BookContext() : base("name=BookContext")
        {

        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new BookEntityConfiguration());
            modelBuilder.Configurations.Add(new ISBNEntityConfiguration());
            modelBuilder.Configurations.Add(new UserEntityConfiguration());

        }

    }
}