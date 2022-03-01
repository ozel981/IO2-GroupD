using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaleSystem.Database.DatabaseModels;

namespace SaleSystem.Database.Configurations
{
    public class PostEntityConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(c => c.ID);

            builder.Property(c => c.Title)
               .IsRequired()
               .HasMaxLength(64);

            builder.Property(c => c.Content)
               .HasMaxLength(4096);

            builder.Property(c => c.CreationDateTime)
               .IsRequired();

            builder.HasOne(c => c.Creator)
              .WithMany(c => c.PostsCreated)
              .IsRequired()
              .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Category)
              .WithMany(c => c.Posts)
              .IsRequired()
              .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
