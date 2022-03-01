using SaleSystem.Database.DatabaseModels;
using System;
using System.Linq;

namespace SaleSystem.Database
{
    public static class DatabaseInitializer
    {
        public static void Initialize(SaleSystemDBContext context)
        {
            context.Database.EnsureCreated();

            if (context.Posts.Any())
                return;

            Category[] categories = GetCategories();
            User[] users = GetUsers();
            Post[] posts = GetPosts(users, categories);
            Comment[] comments = GetComments(posts, users);
            LikeComment[] likeComments = GetLikeComment(comments, users);
            SubscriberCategory[] subscriberCategories = GetSubscriberCategories(categories, users);

            context.Categories.AddRange(categories);
            context.Users.AddRange(users);
            context.Posts.AddRange(posts);
            context.Comments.AddRange(comments);
            context.LikesUsersComments.AddRange(likeComments);
            context.SubscribersCategories.AddRange(subscriberCategories);
            context.SaveChanges();
        }

        public static Category[] GetCategories()
        {
            return new Category[]
            {
                new Category()
                {
                    Name = "Buty"
                },
                new Category()
                {
                    Name = "RTV/AGD"
                },
                new Category()
                {
                    Name = "Elektronika"
                },
                new Category()
                {
                    Name = "Ubrania"
                },
                new Category()
                {
                    Name = "Telefony"
                },
                new Category()
                {
                    Name = "Jedzenie"
                }
            };
        }

        public static User[] GetUsers()
        {
            return new User[]
            {
                new User
                {
                    Name = "Kuba",
                    EmailAddress = "jakub@mailbox.com",
                    Type = UserType.Entrepreneur,
                    IsActive = true,
                    IsVerified = true,
                    CreationDateTime = new DateTime(2020,1,1)
                },
                new User
                {
                    Name = "Damian",
                    EmailAddress = "damian@mailbox.com",
                    Type = UserType.Entrepreneur,
                    IsActive = true,
                    IsVerified = true,
                    CreationDateTime = new DateTime(2020,2,1)
                },
                new User
                {
                    Name = "Adam",
                    EmailAddress = "adam@mailbox.com",
                    Type = UserType.Individual,
                    IsActive = true,
                    IsVerified = true,
                    CreationDateTime = new DateTime(2020,3,1)
                },
                new User
                {
                    Name = "Wojtek",
                    EmailAddress = "wpodmokly@gmail.com",
                    Type = UserType.Admin,
                    IsActive = true,
                    IsVerified = true,
                    CreationDateTime = new DateTime(2018,1,1)
                },
                new User
                {
                    Name = "Piotrek",
                    EmailAddress = "piotrek@mailbox.com",
                    Type = UserType.Individual,
                    IsActive = false,
                    IsVerified = false,
                    CreationDateTime = new DateTime(2021,3,30)
                },

            };
        }

        public static Post[] GetPosts(User[] users, Category[] categories)
        {
            return new Post[]
            {
                new Post
                {
                    Content = "Tanie buty druga para gratis.",
                    CreationDateTime = new DateTime(2021,02,13),
                    Title = "Buty 2za1",
                    Creator = users[0],
                    Enterpreneur = users[3],
                    Category = categories[0]
                },
                new Post
                {
                    Content = "Tania koszulka bawełniana 50% taniej",
                    CreationDateTime = new DateTime(2021,03,1),
                    Title = "T-shirt -50%",
                    Creator = users[1],
                    Enterpreneur = users[4],
                    Category = categories[1]
                },
                new Post
                {
                    Content = "Bielizna męska i damska 10% taniej",
                    CreationDateTime = new DateTime(2021,03,30),
                    Title = "Bielizna -10%",
                    Creator = users[2],
                    Enterpreneur = users[4],
                    Category = categories[2]
                }
            };
        }

        public static Comment[] GetComments(Post[] posts, User[] users)
        {
            return new Comment[]
            {
                new Comment
                {
                    Content = "Dziękujemy za udostępnienie naszej promocji ;). Polecamy zobaczeyć też inne nasze oferty.",
                    CreationDateTime = new DateTime(2021,3,30),
                    Post = posts[0],
                    User = users[2]
                },
                new Comment
                {
                    Content = "U nas lepsze.",
                    CreationDateTime = new DateTime(2021,3,18),
                    Post = posts[1],
                    User = users[3]
                },
                new Comment
                {
                    Content = "Co za scam???!!!! Ta koszulka 3 dni temu kosztowała tylesamo co teraz na promocji.",
                    CreationDateTime = new DateTime(2021,3,30),
                    Post = posts[2],
                    User = users[4]
                },

            };
        }

        public static LikeComment[] GetLikeComment(Comment[] comments, User[] users)
        {
            return new LikeComment[]
            {
                new LikeComment
                {
                    Comment = comments[0],
                    User = users[4]
                },
                new LikeComment
                {
                    Comment = comments[0],
                    User = users[2]
                },
                new LikeComment
                {
                    Comment = comments[1],
                    User = users[3]
                }
            };
        }

        public static SubscriberCategory[] GetSubscriberCategories(Category[] categories, User[] users)
        {
            return new SubscriberCategory[]
            {
                new SubscriberCategory
                {
                    Category = categories[4],
                    Subscriber = users[2]
                },
                new SubscriberCategory
                {
                    Category = categories[1],
                    Subscriber = users[0]
                }
            };
        }
    }
}