using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaleSystem.Database.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleSystem.Database.Configurations
{
    public class CommentEntityConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(c => c.ID);

            builder.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(4096);

            builder.Property(c => c.CreationDateTime)
               .IsRequired();

            builder.HasOne(c => c.Post)
               .WithMany(c => c.Comments)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.User)
              .WithMany(c => c.Comments)
              .OnDelete(DeleteBehavior.NoAction)
              .IsRequired();

        }
    }
}
