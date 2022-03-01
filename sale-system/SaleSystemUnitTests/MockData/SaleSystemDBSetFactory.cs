using Microsoft.EntityFrameworkCore;
using Moq;
using SaleSystem.Database;
using SaleSystem.Database.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleSystemUnitTests.MockData
{
    public class SaleSystemDBSetFactory
    {
        #region attributes - data
        List<Category> categories = new List<Category>();
        List<Comment> comments = new List<Comment>();
        List<LikeComment> likeComments = new List<LikeComment>();
        List<LikePost> likePosts = new List<LikePost>();
        List<Post> posts = new List<Post>();
        List<SubscriberCategory> subscriberCategories = new List<SubscriberCategory>();
        List<User> users = new List<User>();
        #endregion
        public SaleSystemDBSetFactory()
        {
            InitSampleData();
        }
        #region methods - get last init data 
        public List<Category> GetLastInitCategories() => new List<Category>(categories);
        public List<Comment> GetLastInitComments() => new List<Comment>(comments);
        public List<LikeComment> GetLastInitLikeComments() => new List<LikeComment>(likeComments);
        public List<LikePost> GetLastInitLikePosts() => new List<LikePost>(likePosts);
        public List<Post> GetLastInitPosts() => new List<Post>(posts);
        public List<SubscriberCategory> GetLastInitSubscriberCategories() => new List<SubscriberCategory>(subscriberCategories);
        public List<User> GetLastInitUsers() => new List<User>(users);
        #endregion
        #region methods - get last init sets
        public Mock<DbSet<Category>> GetCategoriesMockDBSet()
            => GetMockDBSet(new List<Category>(categories));
        public Mock<DbSet<Comment>> GetCommentsMockDBSet()
            => GetMockDBSet(new List<Comment>(comments));
        public Mock<DbSet<LikeComment>> GetLikeCommentsMockDBSet()
            => GetMockDBSet(new List<LikeComment>(likeComments));
        public Mock<DbSet<LikePost>> GetLikePostsMockDBSet()
            => GetMockDBSet(new List<LikePost>(likePosts));
        public Mock<DbSet<Post>> GetPostsMockDBSet()
            => GetMockDBSet(new List<Post>(posts));
        public Mock<DbSet<SubscriberCategory>> GetSubscriberCategoriesMockDBSet()
            => GetMockDBSet(new List<SubscriberCategory>(subscriberCategories));
        public Mock<DbSet<User>> GetUsersMockDBSet()
            => GetMockDBSet(new List<User>(users));
        #endregion
        private void InitSampleData()
        {
            categories = GetSampleCategories();
            users = GetSampleUsers();
            posts = GetSamplePosts(ref users, ref categories);
            likePosts = GetSampleLikePosts(ref users, ref posts);
            comments = GetSampleComments(ref users, ref posts);
            likeComments = GetSampleLikeComments(ref users, ref comments);
            subscriberCategories = GetSampleSubscriberCategory(ref users, ref categories);
        }
        private static Mock<DbSet<T>> GetMockDBSet<T>(List<T> list)
            where T : class
        {
            Mock<DbSet<T>> mock = new Mock<DbSet<T>>();
            mock.As<IQueryable<T>>().Setup(c => c.Provider).Returns(list.AsQueryable().Provider);
            mock.As<IQueryable<T>>().Setup(c => c.Expression).Returns(list.AsQueryable().Expression);
            mock.As<IQueryable<T>>().Setup(c => c.ElementType).Returns(list.AsQueryable().ElementType);
            mock.As<IQueryable<T>>().Setup(c => c.GetEnumerator()).Returns(list.AsQueryable().GetEnumerator());
            mock.Setup(m => m.AsQueryable()).Returns(list.AsQueryable());

            return mock;
        }
        #region methods - get samlpe data for init
        private static List<Category> GetSampleCategories()
        {
            return new List<Category>
            {
                new Category
                {
                    ID = 1,
                    Name = "Buty",
                    SubscriberCategory = new List<SubscriberCategory>(),
                    Posts = new List<Post>()
                },
                new Category
                {
                    ID = 2,
                    Name = "RTV/AGD",
                    SubscriberCategory = new List<SubscriberCategory>(),
                    Posts = new List<Post>()
                },
                new Category
                {
                    ID = 3,
                    Name = "Elektronika",
                    SubscriberCategory = new List<SubscriberCategory>(),
                    Posts = new List<Post>()
                },
                new Category
                {
                    ID = 4,
                    Name = "Ubrania",
                    SubscriberCategory = new List<SubscriberCategory>(),
                    Posts = new List<Post>()
                },
                new Category
                {
                    ID = 5,
                    Name = "Telefony",
                    SubscriberCategory = new List<SubscriberCategory>(),
                    Posts = new List<Post>()
                },
                new Category
                {
                    ID = 6,
                    Name = "Jedzenie",
                    SubscriberCategory = new List<SubscriberCategory>(),
                    Posts = new List<Post>()
                },
                new Category
                {
                    ID = 7,
                    Name = "?$5%",
                    SubscriberCategory = new List<SubscriberCategory>(),
                    Posts = new List<Post>()
                },
                new Category
                {
                    ID = 8,
                    Name = ",",
                    SubscriberCategory = new List<SubscriberCategory>(),
                    Posts = new List<Post>()
                },
            };
        }
        private static List<Comment> GetSampleComments(ref List<User> users, ref List<Post> posts)
        {
            List<Comment> comments = new List<Comment>
            {
                new Comment
                {
                    ID = 0,
                    Content = "content0",
                    CreationDateTime = new DateTime(2021,03,18),
                    LikeComment = new List<LikeComment>(),
                    Post = posts[0],
                    PostID = 0,
                    User = users[3]
                },
                new Comment
                {
                    ID = 1,
                    Content = "u nas lepsze",
                    CreationDateTime = new DateTime(2021,03,18),
                    LikeComment = new List<LikeComment>(),
                    Post = posts[0],
                    PostID = 0,
                    User = users[1]
                },
                new Comment
                {
                    ID = 2,
                    Content = "content2",
                    CreationDateTime = DateTime.Now,
                    LikeComment = new List<LikeComment>(),
                    Post = posts[1],
                    PostID = 1,
                    User = users[3]
                }
            };
            users[3].Comments.Add(comments[0]);
            users[3].Comments.Add(comments[2]);
            users[1].Comments.Add(comments[1]);
            posts[0].Comments.Add(comments[0]);
            posts[0].Comments.Add(comments[1]);
            posts[1].Comments.Add(comments[2]);
            return comments;
        }
        private static List<LikeComment> GetSampleLikeComments(ref List<User> users, ref List<Comment> comments)
        {
            List<LikeComment> likeComments = new List<LikeComment>
            {
                new LikeComment
                {
                    Comment = comments[0],
                    CommentID = 0,
                    UserID = 2,
                    User = users[2]
                },
                new LikeComment
                {
                    Comment = comments[0],
                    CommentID = 0,
                    UserID = 0,
                    User = users[0]
                },
                new LikeComment
                {
                    Comment = comments[2],
                    CommentID = 2,
                    UserID = 2,
                    User = users[2]
                }
            };
            comments[0].LikeComment.Add(likeComments[0]);
            comments[0].LikeComment.Add(likeComments[1]);
            comments[2].LikeComment.Add(likeComments[2]);
            users[2].LikeComment.Add(likeComments[0]);
            users[2].LikeComment.Add(likeComments[2]);
            users[0].LikeComment.Add(likeComments[1]);
            return likeComments;
        }
        private static List<LikePost> GetSampleLikePosts(ref List<User> users, ref List<Post> posts)
        {
            List<LikePost> likePosts = new List<LikePost>
            {
                new LikePost
                {
                    Post = posts[0],
                    PostID = 0,
                    User = users[2],
                    UserID = 2,
                },
                new LikePost
                {
                    Post = posts[0],
                    PostID = 0,
                    User = users[3],
                    UserID = 3,
                },
                new LikePost
                {
                    Post = posts[1],
                    PostID = 1,
                    User = users[2],
                    UserID = 2,
                },
            };
            posts[0].LikePost.Add(likePosts[0]);
            posts[0].LikePost.Add(likePosts[1]);
            posts[1].LikePost.Add(likePosts[2]);
            users[2].LikePost.Add(likePosts[0]);
            users[3].LikePost.Add(likePosts[1]);
            users[2].LikePost.Add(likePosts[2]);
            return likePosts;
        }
        private static List<Post> GetSamplePosts(ref List<User> users, ref List<Category> categories)
        {
            List<Post> posts = new List<Post>
            {
                new Post
                {
                    ID = 0,
                    Content = "post0",
                    CreationDateTime = new DateTime(2021,2,13),
                    Title = "title0",
                    Creator = users[2],
                    Enterpreneur = users[0],
                    Comments = new List<Comment> (),
                    LikePost = new List<LikePost> (),
                    Category = categories[0]
                },
                new Post
                {
                    ID = 1,
                    Content = "post1",
                    CreationDateTime = new DateTime(2021,3,1),
                    PromotionEndDateTime = new DateTime(2021,3,1).AddDays(-5),
                    Title = "title1",
                    Creator = users[1],
                    Enterpreneur = users[1],
                    Comments = new List<Comment> (),
                    LikePost = new List<LikePost> (),
                    Category = categories[2]
                },
                new Post
                {
                    ID = 2,
                    Content = "post2",
                    CreationDateTime = DateTime.Now,
                    PromotionEndDateTime = DateTime.Now.AddDays(30),
                    Title = "title2",
                    Creator = users[2],
                    Enterpreneur = users[1],
                    Comments = new List<Comment> (),
                    LikePost = new List<LikePost> (),
                    Category = categories[2]
                }
            };
            users[2].PostsCreated.Add(posts[0]);
            users[0].PostsAboutMyEnterprise.Add(posts[0]);
            users[1].PostsCreated.Add(posts[1]);
            users[1].PostsAboutMyEnterprise.Add(posts[1]);
            users[2].PostsCreated.Add(posts[2]);
            users[1].PostsAboutMyEnterprise.Add(posts[2]);
            categories[0].Posts.Add(posts[0]);
            categories[2].Posts.Add(posts[1]);
            categories[2].Posts.Add(posts[2]);
            return posts;
        }

        private static List<SubscriberCategory> GetSampleSubscriberCategory(ref List<User> users, ref List<Category> categories)
        {
            List<SubscriberCategory> subscriberCategories = new List<SubscriberCategory>
            {
                new SubscriberCategory
                {
                    CategoryID = 7,
                    Category = categories[6],
                    Subscriber = users[2],
                    SubscriberID = 2
                },
                new SubscriberCategory
                {
                    CategoryID = 1,
                    Category = categories[0],
                    Subscriber = users[2],
                    SubscriberID = 2
                },
            };
            users[2].SubscriberCategory.Add(subscriberCategories[0]);
            users[2].SubscriberCategory.Add(subscriberCategories[1]);
            categories[6].SubscriberCategory.Add(subscriberCategories[0]);
            categories[0].SubscriberCategory.Add(subscriberCategories[1]);
            return subscriberCategories;
        }

        private static List<User> GetSampleUsers()
        {
            return new List<User>
            {
                new User
                {
                    ID = 0,
                    Name = "name0",
                    EmailAddress = "email0@adress.com",
                    Type = UserType.Entrepreneur,
                    IsActive = true,
                    IsVerified = true,
                    CreationDateTime = new DateTime(2020,1,1),
                    Comments = new List<Comment>(),
                    LikeComment = new List<LikeComment>(),
                    SubscriberCategory = new List<SubscriberCategory>(),
                    LikePost = new List<LikePost>(),
                    PostsCreated = new List<Post>(),
                    PostsAboutMyEnterprise = new List<Post>()
                },
                new User
                {
                    ID = 1,
                    Name = "name1",
                    EmailAddress = "email1@adress.com",
                    Type = UserType.Entrepreneur,
                    IsActive = true,
                    IsVerified = true,
                    CreationDateTime = new DateTime(2020,2,1),
                    Comments = new List<Comment>(),
                    LikeComment = new List<LikeComment>(),
                    SubscriberCategory = new List<SubscriberCategory>(),
                    LikePost = new List<LikePost>(),
                    PostsCreated = new List<Post>(),
                    PostsAboutMyEnterprise = new List<Post>()
                },
                new User
                {
                    ID = 2,
                    Name = "name2",
                    EmailAddress = "email2@adress.com",
                    Type = UserType.Individual,
                    IsActive = true,
                    IsVerified = true,
                    CreationDateTime = new DateTime(2020,3,1),
                    Comments = new List<Comment>(),
                    LikeComment = new List<LikeComment>(),
                    SubscriberCategory = new List<SubscriberCategory>(),
                    LikePost = new List<LikePost>(),
                    PostsCreated = new List<Post>(),
                    PostsAboutMyEnterprise = new List<Post>()
                },
                new User
                {
                    ID = 3,
                    Name = "name3",
                    EmailAddress = "email3@adress.com",
                    Type = UserType.Admin,
                    IsActive = true,
                    IsVerified = true,
                    CreationDateTime = new DateTime(2018,1,1),
                    Comments = new List<Comment>(),
                    LikeComment = new List<LikeComment>(),
                    SubscriberCategory = new List<SubscriberCategory>(),
                    LikePost = new List<LikePost>(),
                    PostsCreated = new List<Post>(),
                    PostsAboutMyEnterprise = new List<Post>()
                },
                new User
                {
                    ID = 4,
                    Name = "name4",
                    EmailAddress = "email4@adress.com",
                    Type = UserType.Individual,
                    IsActive = false,
                    IsVerified = false,
                    CreationDateTime = DateTime.Now,
                    Comments = new List<Comment>(),
                    LikeComment = new List<LikeComment>(),
                    SubscriberCategory = new List<SubscriberCategory>(),
                    LikePost = new List<LikePost>(),
                    PostsCreated = new List<Post>(),
                    PostsAboutMyEnterprise = new List<Post>()
                }

            };
        }
        #endregion
    }
}
