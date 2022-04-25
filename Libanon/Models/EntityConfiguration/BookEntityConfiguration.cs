using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Libanon.Models.EntityConfiguration
{
    public class BookEntityConfiguration : EntityTypeConfiguration<Book>
    {
        public BookEntityConfiguration()
        {
            this.ToTable("Book").HasKey(x => x.Id);
            this.Property(s => s.Authors).IsRequired();
            this.Property(s => s.Publisher).IsRequired();
            this.Property(s => s.Category).IsRequired();
            this.Property(s => s.ImageUrl).IsRequired();
            this.Property(s => s.Title).IsRequired().HasMaxLength(100);
            this.HasRequired(s => s.CurrentISBN)
                .WithRequiredPrincipal(isbn => isbn.Book)
                .WillCascadeOnDelete(true);
            this.HasRequired(s => s.CurrentUser)
                .WithRequiredPrincipal(user => user.Book)
                .WillCascadeOnDelete(true);
        }
    }
}