using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using SaleSystem;
using SaleSystem.Database;
using SaleSystem.Database.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SaleSystemIntegrationTests.Helpers
{
    public static class Utilities
    {
        public static void InitializeDbForTests(SaleSystemDBContext db)
        {
            Category[] categories = SampleData.GetCategories();
            User[] users = SampleData.GetUsers();
            Post[] posts = SampleData.GetPosts(users, categories);
            Comment[] comments = SampleData.GetComments(posts, users);
            LikeComment[] likeComments = SampleData.GetLikeComment(comments, users);
            LikePost[] likePosts = SampleData.GetLikePost(posts, users);
            SubscriberCategory[] subscriberCategories = SampleData.GetSubscriberCategories(categories, users);

            db.Categories.AddRange(categories);
            db.Users.AddRange(users);
            db.Posts.AddRange(posts);
            db.Comments.AddRange(comments);
            db.LikesUsersComments.AddRange(likeComments);
            db.LikesUsersPosts.AddRange(likePosts);
            db.SubscribersCategories.AddRange(subscriberCategories);
            db.SaveChanges();
        }
    }
}
