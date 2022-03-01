using Microsoft.EntityFrameworkCore;
using SaleSystem.Database.DatabaseModels;
using SaleSystem.Database.Configurations;

namespace SaleSystem.Database
{
    public class SaleSystemDBContext : DbContext
    {
        public SaleSystemDBContext(DbContextOptions<SaleSystemDBContext> options) : base(options) { }
        public SaleSystemDBContext() : base() { }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<LikeComment> LikesUsersComments { get; set; }
        public virtual DbSet<LikePost> LikesUsersPosts { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<SubscriberCategory> SubscribersCategories { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CategoryEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CommentEntityConfiguration());
            modelBuilder.ApplyConfiguration(new LikeCommentEntityConfiguration());
            modelBuilder.ApplyConfiguration(new LikePostEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PostEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SubscriberCategoryEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        }
    }
}