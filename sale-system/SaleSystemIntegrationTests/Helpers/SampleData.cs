using SaleSystem.Database.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleSystemIntegrationTests.Helpers
{
    public static class SampleData
    {
        public static Category[] GetCategories()
        {
            return new Category[]
            {
                new Category()
                {
                    ID = 1,
                    Name = "Buty"
                },
                new Category()
                {
                    ID = 2,
                    Name = "RTV/AGD"
                },
                new Category()
                {
                    ID = 3,
                    Name = "Elektronika"
                },
                new Category()
                {
                    ID = 4,
                    Name = "Ubrania"
                },
                new Category()
                {
                    ID = 5,
                    Name = "Telefony"
                },
                new Category()
                {
                    ID = 6,
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
                    ID = 1,
                    Content = "Tanie buty druga para gratis.",
                    CreationDateTime = new DateTime(2021,02,13),
                    Title = "Buty 2za1",
                    Creator = users[0],
                    Enterpreneur = users[3],
                    Category = categories[0]
                },
                new Post
                {
                    ID = 2,
                    Content = "Tania koszulka bawełniana 50% taniej",
                    CreationDateTime = new DateTime(2021,03,1),
                    Title = "T-shirt -50%",
                    Creator = users[1],
                    Enterpreneur = users[4],
                    Category = categories[1],
                    PromotionEndDateTime = DateTime.Now.AddDays(-5)
                },
                new Post
                {
                    ID = 3,
                    Content = "Bielizna męska i damska 10% taniej",
                    CreationDateTime = new DateTime(2021,03,30),
                    Title = "Bielizna -10%",
                    Creator = users[2],
                    Enterpreneur = users[4],
                    Category = categories[2],
                    PromotionEndDateTime = DateTime.Now.AddDays(5)
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
                }
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

        public static LikePost[] GetLikePost(Post[] posts, User[] users)
        {
            return new LikePost[]
            {
                new LikePost
                {
                    Post = posts[0],
                    User = users[4]
                },
                new LikePost
                {
                    Post = posts[0],
                    User = users[2]
                },
                new LikePost
                {
                    Post = posts[1],
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
