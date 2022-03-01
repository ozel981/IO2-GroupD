using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaleSystem.Database.DatabaseModels;

namespace SaleSystem.Database.Configurations
{
    public class LikeCommentEntityConfiguration : IEntityTypeConfiguration<LikeComment>
    {
        public void Configure(EntityTypeBuilder<LikeComment> builder)
        {
            builder.HasKey(c => new { c.UserID, c.CommentID });

            builder.HasOne(c => c.User)
                .WithMany(c => c.LikeComment)
                .HasForeignKey(c => c.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.Comment)
                .WithMany(c => c.LikeComment)
                .HasForeignKey(c => c.CommentID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
