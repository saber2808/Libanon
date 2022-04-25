using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Libanon.Models.EntityConfiguration
{
    public class UserEntityConfiguration : EntityTypeConfiguration<User>
    {
        public UserEntityConfiguration()
        {
            this.HasKey(x => x.Id);
            this.Property(s=>s.Email).IsRequired();
            this.Property(s => s.PhoneNumber).IsRequired();
            this.Property(s => s.FullName).IsRequired();
        }
    }
}