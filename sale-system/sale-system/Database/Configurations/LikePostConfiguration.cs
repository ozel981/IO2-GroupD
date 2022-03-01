using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaleSystem.Database.DatabaseModels;

namespace SaleSystem.Database.Configurations
{
    public class LikePostEntityConfiguration : IEntityTypeConfiguration<LikePost>
    {
        public void Configure(EntityTypeBuilder<LikePost> builder)
        {
            builder.HasKey(c => new { c.UserID, c.PostID });

            builder.HasOne(c => c.User)
                .WithMany(c => c.LikePost)
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.Post)
                .WithMany(c => c.LikePost)
                .HasForeignKey(c => c.PostID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
