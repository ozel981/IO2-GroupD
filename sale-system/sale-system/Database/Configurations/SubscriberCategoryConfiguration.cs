using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaleSystem.Database.DatabaseModels;

namespace SaleSystem.Database.Configurations
{
    public class SubscriberCategoryEntityConfiguration : IEntityTypeConfiguration<SubscriberCategory>
    {
        public void Configure(EntityTypeBuilder<SubscriberCategory> builder)
        {
            builder.HasKey(c => new { c.CategoryID, c.SubscriberID });

            builder.HasOne(c => c.Category)
                .WithMany(c => c.SubscriberCategory)
                .HasForeignKey(c => c.CategoryID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.Subscriber)
                .WithMany(c => c.SubscriberCategory)
                .HasForeignKey(c => c.SubscriberID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
