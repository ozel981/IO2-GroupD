using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaleSystem.Database.DatabaseModels;

namespace SaleSystem.Database.Configurations
{
    public class CategoryEntityConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.ID);

            builder.HasIndex(c => c.Name)
                .IsUnique();

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(32);

            builder.HasMany(c => c.Posts)
              .WithOne(c => c.Category)
              .IsRequired()
              .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
