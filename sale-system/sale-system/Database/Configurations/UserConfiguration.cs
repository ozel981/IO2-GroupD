using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaleSystem.Database.DatabaseModels;

namespace SaleSystem.Database.Configurations
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(c => new { c.ID });

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(64);

            builder.HasIndex(c => c.EmailAddress)
                .IsUnique();

            builder.Property(c => c.EmailAddress)
               .IsRequired()
               .HasMaxLength(64);

            builder.Property(c => c.Type)
               .IsRequired();

            builder.Property(c => c.IsActive)
               .HasDefaultValue(false)
               .IsRequired();

            builder.Property(c => c.IsVerified)
               .HasDefaultValue(false)
               .IsRequired();

            builder.Property(c => c.CreationDateTime)
               .IsRequired();
        }
    }
}
