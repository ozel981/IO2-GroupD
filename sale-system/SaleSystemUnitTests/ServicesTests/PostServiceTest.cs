using Microsoft.EntityFrameworkCore;
using Moq;
using SaleSystem.Database.DatabaseModels;
using SaleSystem.Models.Posts;
using SaleSystem.Models.Users;
using SaleSystem.Services;
using SaleSystem.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using SaleSystemUnitTests.MockData;
using SendGrid.Helpers.Errors.Model;
using SaleSystem.Models.Comments;

namespace SaleSystemUnitTests.ServicesTests
{
    public class PostServiceTest
    {
        private readonly SaleSystemDBSetFactory DBSetFactory;

        public PostServiceTest()
        {
            DBSetFactory = new SaleSystemDBSetFactory();
            examplePosts3[0].LikePost = new List<LikePost>(exampleLikePost);

            examplePosts = new List<Post>
            {
                new Post{
                    ID = 1,
                    Creator = jeff,
                    Content = "blue",
                    Category =  new Category
                    {
                        Name = "colors",
                    },
                    LikePost = new List<LikePost>(),
                    Comments = new List<Comment>()
                }
            };

            jeff.PostsCreated = new List<Post>(examplePosts);
        }

        private static readonly User jeff = new User
        {
            ID = 42,
            Name = "Jeff",
            Type = UserType.Individual
        };

        private static readonly UserView jeffView = new UserView
        {
            UserName = "Jeff"
        };

        private static readonly User mikeAdmin = new User
        {
            ID = 52,
            Name = "Mike",
            Type = UserType.Admin
        };

        private static readonly User john = new User
        {
            ID = 1,
            Name = "John",
            Type = UserType.Individual
        };

        private static readonly List<User> exampleUsers = new List<User>
        {
            jeff, mikeAdmin, john
        };

        private static readonly List<LikePost> exampleLikePost = new List<LikePost>
        {
            new LikePost
            {
                PostID = 1,
                UserID = 1
            },

            new LikePost
            {
                PostID = 3,
                UserID = 52
            },

            new LikePost
            {
                PostID = 1,
                UserID = 42
            }
        };

        private readonly List<Post> examplePosts;

        private static readonly List<PostView> examplePostViews = new List<PostView>
        {
            new PostView{
                ID = 1,
                AuthorName = jeffView.UserName,
                AuthorID = jeff.ID,
                Content = "blue",
                Category = "colors"
            }
        };

        private static readonly List<Post> examplePosts2 = new List<Post>
        {
            new Post{
                ID = 1,
                Creator = jeff,
                Content = "blue",
                CreationDateTime = new DateTime(2021, 1, 1),
                PromotionEndDateTime = new DateTime(2025, 5, 25),
                Category =  new Category
                {
                    ID = 1,
                    Name = "Colors",
                },
                LikePost = new List<LikePost>(),
                Comments = new List<Comment>()
            },
            new Post{
                ID = 2,
                Creator = jeff,
                Content = "Pralka",
                CreationDateTime = new DateTime(2022, 2, 2),
                PromotionEndDateTime = null,
                Category =  new Category
                {
                    ID = 2,
                    Name = "RTV/AGD",
                },
                LikePost = new List<LikePost>(),
                Comments = new List<Comment>()
            },
            new Post{
                ID = 3,
                Creator = jeff,
                Content = "Telefon",
                CreationDateTime = new DateTime(2023, 3, 3),
                PromotionEndDateTime = new DateTime(2025, 5, 25),
                Category =  new Category
                {
                    ID = 3,
                    Name = "Elektronika",
                },
                LikePost = new List<LikePost>(),
                Comments = new List<Comment>()
            }
        };

        private readonly List<PostView> examplePostViews2 = new List<PostView>
        {
            new PostView{
                ID = 3,
                AuthorName = jeffView.UserName,
                AuthorID = jeff.ID,
                Content = "Telefon",
                Category = "Elektronika"
            },
            new PostView{
                ID = 1,
                AuthorName = jeffView.UserName,
                AuthorID = jeff.ID,
                Content = "blue",
                Category = "Colors"
            },
            new PostView{
                ID = 2,
                AuthorName = jeffView.UserName,
                AuthorID = jeff.ID,
                Content = "Pralka",
                Category = "RTV/AGD"
            },
        };

        private readonly List<PostView> examplePostViews22 = new List<PostView>
        {
            new PostView{
                ID = 1,
                AuthorName = jeffView.UserName,
                AuthorID = jeff.ID,
                Content = "blue",
                Category = "Colors"
            },
            new PostView{
                ID = 2,
                AuthorName = jeffView.UserName,
                AuthorID = jeff.ID,
                Content = "Pralka",
                Category = "RTV/AGD"
            }
        };

        private readonly List<PostView> examplePostViews23 = new List<PostView>
        {
            new PostView{
                ID = 3,
                AuthorName = jeffView.UserName,
                AuthorID = jeff.ID,
                Content = "Telefon",
                Category = "Elektronika"
            },
            new PostView{
                ID = 1,
                AuthorName = jeffView.UserName,
                AuthorID = jeff.ID,
                Content = "blue",
                Category = "Colors"
            }
        };

        private readonly List<PostView> examplePostViews24 = new List<PostView>
        {
            new PostView{
                ID = 3,
                AuthorName = jeffView.UserName,
                AuthorID = jeff.ID,
                Content = "Telefon",
                Category = "Elektronika"
            }
        };

        private static readonly List<Post> examplePosts3 = new List<Post>
        {
            new Post{
                ID = 1,
                Creator = jeff,
                Content = "blue",
                Category =  new Category
                {
                    Name = "colors",
                },
                LikePost = new List<LikePost>(),
                Comments = new List<Comment>()
            }
        };

        private static readonly List<Category> exampleCategory = new List<Category>
        {
            new Category
            {
                ID=1,
                Name="Buty"
            }
        };

        [Fact(DisplayName = "All posts")]
        public void GetAllPosts()
        {
            var mockPosts = GetMockDbSet(examplePosts.AsQueryable());
            var mockUsers = GetMockDbSet(exampleUsers.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);
            mockDBContext.Setup(o => o.Users).Returns(mockUsers.Object);

            var svc = new PostService(mockDBContext.Object);
            var actual = svc.GetAll(jeff.ID).ToList();
            Assert.Equal(examplePostViews.Count, actual.Count);
            for (int i = 0; i < examplePostViews.Count; i++)
            {
                Assert.Equal(examplePostViews[i], actual[i], new Comparers.PostViewComparer());
            }
        }

        [Fact(DisplayName = "Posts ordered by date and promotion")]
        public void GetAllOrderedPosts()
        {
            var mockPosts = GetMockDbSet(examplePosts2.AsQueryable());
            var mockUsers = GetMockDbSet(exampleUsers.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);
            mockDBContext.Setup(o => o.Users).Returns(mockUsers.Object);

            var svc = new PostService(mockDBContext.Object);
            var actual = svc.GetAll(jeff.ID).ToList();
            Assert.Equal(examplePostViews2.Count, actual.Count);
            for (int i = 0; i < examplePostViews2.Count; i++)
            {
                Assert.Equal(examplePostViews2[i], actual[i], new Comparers.PostViewComparer());
            }
        }

        [Fact(DisplayName = "No categories selected")]
        public void GetPostsFilteredByNoCategories()
        {
            var mockPosts = GetMockDbSet(examplePosts2.AsQueryable());
            var mockUsers = GetMockDbSet(exampleUsers.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);
            mockDBContext.Setup(o => o.Users).Returns(mockUsers.Object);

            var svc = new PostService(mockDBContext.Object);
            var actual = svc.GetFilteredPosts(jeff.ID, new List<int>()).ToList();
            Assert.Empty(actual);
        }

        [Fact(DisplayName = "Selected categories without linked posts")]
        public void GetPostsFilteredByCategoriesThatNotHaveLinkedPosts()
        {
            var mockPosts = GetMockDbSet(examplePosts2.AsQueryable());
            var mockUsers = GetMockDbSet(exampleUsers.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);
            mockDBContext.Setup(o => o.Users).Returns(mockUsers.Object);

            var svc = new PostService(mockDBContext.Object);
            var actual = svc.GetFilteredPosts(jeff.ID, new List<int>() { 4 }).ToList();
            Assert.Empty(actual);
        }

        [Fact(DisplayName = "Selected categories linked with all posts")]
        public void GetPostsFilteredByCategoriesLinkedWithAllPosts()
        {
            var mockPosts = GetMockDbSet(examplePosts2.AsQueryable());
            var mockUsers = GetMockDbSet(exampleUsers.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);
            mockDBContext.Setup(o => o.Users).Returns(mockUsers.Object);

            var svc = new PostService(mockDBContext.Object);
            var actual = svc.GetFilteredPosts(jeff.ID, new List<int>() { 1, 2, 3, 4 }).ToList();
            Assert.Equal(examplePostViews2.Count, actual.Count);
            for (int i = 0; i < examplePostViews2.Count; i++)
            {
                Assert.Equal(examplePostViews2[i], actual[i], new Comparers.PostViewComparer());
            }
        }

        [Fact(DisplayName = "Selected few categories linked with few posts")]
        public void GetPostsFilteredByFewCategories()
        {
            var mockPosts = GetMockDbSet(examplePosts2.AsQueryable());
            var mockUsers = GetMockDbSet(exampleUsers.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);
            mockDBContext.Setup(o => o.Users).Returns(mockUsers.Object);

            var svc = new PostService(mockDBContext.Object);
            var actual = svc.GetFilteredPosts(jeff.ID, new List<int>() { 1, 2, 4 }).ToList();
            Assert.Equal(examplePostViews22.Count, actual.Count);
            for (int i = 0; i < examplePostViews22.Count; i++)
            {
                Assert.Equal(examplePostViews22[i], actual[i], new Comparers.PostViewComparer());
            }
        }

        [Fact(DisplayName = "No categories selected, posts amount = 10 -> last page")]
        public void GetPostsFilteredPagedByNoCategories()
        {
            var mockPosts = GetMockDbSet(examplePosts2.AsQueryable());
            var mockUsers = GetMockDbSet(exampleUsers.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);
            mockDBContext.Setup(o => o.Users).Returns(mockUsers.Object);

            var svc = new PostService(mockDBContext.Object);
            var actual = svc.GetFilteredPosts(jeff.ID, new List<int>(), 1, 10);
            Assert.Empty(actual.Posts);
            Assert.True(actual.IsLastPage);
        }

        [Fact(DisplayName = "Selected categories without linked posts, posts amount = 10 -> last page")]
        public void GetPostsFilteredPagedByCategoriesThatNotHaveLinkedPosts()
        {
            var mockPosts = GetMockDbSet(examplePosts2.AsQueryable());
            var mockUsers = GetMockDbSet(exampleUsers.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);
            mockDBContext.Setup(o => o.Users).Returns(mockUsers.Object);

            var svc = new PostService(mockDBContext.Object);
            var actual = svc.GetFilteredPosts(jeff.ID, new List<int>() { 4 }, 1, 10);
            Assert.Empty(actual.Posts);
            Assert.True(actual.IsLastPage);
        }

        [Fact(DisplayName = "Selected categories linked with all posts, posts amount = 2 -> not last page")]
        public void GetPostsFilteredPagedByCategoriesLinkedWithAllPosts()
        {
            var mockPosts = GetMockDbSet(examplePosts2.AsQueryable());
            var mockUsers = GetMockDbSet(exampleUsers.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);
            mockDBContext.Setup(o => o.Users).Returns(mockUsers.Object);

            var svc = new PostService(mockDBContext.Object);
            var actual = svc.GetFilteredPosts(jeff.ID, new List<int>() { 1, 2, 3, 4 }, 1, 2);
            Assert.Equal(examplePostViews23.Count, actual.Posts.Count);
            for (int i = 0; i < examplePostViews23.Count; i++)
            {
                Assert.Equal(examplePostViews23[i], actual.Posts[i], new Comparers.PostViewComparer());
            }
            Assert.False(actual.IsLastPage);
        }

        [Fact(DisplayName = "Selected few categories linked with few posts, posts amount = 1 -> not last page")]
        public void GetPostsFilteredPagedByFewCategories()
        {
            var mockPosts = GetMockDbSet(examplePosts2.AsQueryable());
            var mockUsers = GetMockDbSet(exampleUsers.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);
            mockDBContext.Setup(o => o.Users).Returns(mockUsers.Object);

            var svc = new PostService(mockDBContext.Object);
            var actual = svc.GetFilteredPosts(jeff.ID, new List<int>() { 1, 3, 4 }, 1, 1);
            Assert.Equal(examplePostViews24.Count, actual.Posts.Count);
            for (int i = 0; i < examplePostViews24.Count; i++)
            {
                Assert.Equal(examplePostViews24[i], actual.Posts[i], new Comparers.PostViewComparer());
            }
            Assert.False(actual.IsLastPage);
        }

        [Fact]
        public void GetPost()
        {
            var mockPosts = GetMockDbSet(examplePosts.AsQueryable());
            var mockUsers = GetMockDbSet(exampleUsers.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);
            mockDBContext.Setup(o => o.Users).Returns(mockUsers.Object);

            var svc = new PostService(mockDBContext.Object);
            var actual = svc.GetPost(examplePosts[0].ID, jeff.ID);
            Assert.Equal(examplePostViews[0], actual, new Comparers.PostViewComparer());
        }

        [Fact]
        public void GetPostThatDoesNotExist()
        {
            var mockPosts = GetMockDbSet(examplePosts.AsQueryable());
            var mockUsers = GetMockDbSet(exampleUsers.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);
            mockDBContext.Setup(o => o.Users).Returns(mockUsers.Object);

            var svc = new PostService(mockDBContext.Object);
            Assert.Throws<Exception>(() => svc.GetPost(-1, jeff.ID));
        }

        [Fact]
        public void UpdatePost()
        {
            var mockPosts = GetMockDbSet(examplePosts.AsQueryable());
            var mockCategories = GetMockDbSet(exampleCategory.AsQueryable());

            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);
            mockDBContext.Setup(c => c.Categories).Returns(mockCategories.Object);

            var svc = new PostService(mockDBContext.Object);
            var post = examplePosts[0];
            svc.UpdatePost(post.ID, new PostUpdate
            {
                CategoryID = 1,
                Content = "new content"
            }, post.Creator.ID);

            mockPosts.Verify(m => m.Update(It.IsAny<Post>()), Times.Once);
            mockDBContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public void UpdatePostThatDoesNotExist()
        {
            var mockPosts = GetMockDbSet(examplePosts.AsQueryable());

            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);

            var svc = new PostService(mockDBContext.Object);
            var post = examplePosts[0];
            Assert.Throws<Exception>(() => svc.UpdatePost(-1, new PostUpdate
            {
                Content = "new content"
            }, post.Creator.ID)
            );
        }

        [Fact]
        public void UpdatePostByUnauthorizedUser()
        {
            var mockPosts = GetMockDbSet(examplePosts.AsQueryable());

            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);

            var svc = new PostService(mockDBContext.Object);
            var post = examplePosts[0];
            Assert.Throws<UnauthorizedAccessException>(() => svc.UpdatePost(post.ID, new PostUpdate
            {
                Content = "new content"
            }, -1)
            );
        }

        [Fact]
        public void DeletePostByAdmin()
        {
            var mockUsers = GetMockDbSet(exampleUsers.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Users).Returns(mockUsers.Object);

            var mockPosts = GetMockDbSet(examplePosts.AsQueryable());

            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);

            var svc = new PostService(mockDBContext.Object);
            svc.DeletePost(examplePosts[0].ID, mikeAdmin.ID);

            mockPosts.Verify(m => m.Remove(It.IsAny<Post>()), Times.Once);
            mockDBContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact]
        public void DeletePostByNonExistingUser()
        {
            var mockUsers = GetMockDbSet(exampleUsers.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Users).Returns(mockUsers.Object);

            var mockPosts = GetMockDbSet(examplePosts.AsQueryable());

            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);

            var svc = new PostService(mockDBContext.Object);
            Assert.Throws<Exception>(() => svc.DeletePost(examplePosts[0].ID, 10));
        }

        [Fact]
        public void DeletePostByNonAdmin()
        {
            var mockUsers = GetMockDbSet(exampleUsers.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Users).Returns(mockUsers.Object);

            var mockPosts = GetMockDbSet(examplePosts.AsQueryable());

            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);

            var svc = new PostService(mockDBContext.Object);
            Assert.Throws<UnauthorizedAccessException>(() => svc.DeletePost(examplePosts[0].ID, 1));
        }

        [Fact]
        public void DeleteNonExistentPost()
        {
            var mockUsers = GetMockDbSet(exampleUsers.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Users).Returns(mockUsers.Object);

            var mockPosts = GetMockDbSet(examplePosts.AsQueryable());

            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);

            var svc = new PostService(mockDBContext.Object);
            Assert.Throws<Exception>(() => svc.DeletePost(-1, mikeAdmin.ID));
        }

        [Fact(DisplayName = "Post promotion - never promoted, 7 days, success")]
        public void PromotePostNeverPromoted7DaysSuccess()
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();

            Mock<DbSet<Post>> mockPostsSet = DBSetFactory.GetPostsMockDBSet();

            mockContext.Setup(c => c.Posts).Returns(mockPostsSet.Object);

            PostService service = new PostService(mockContext.Object);
            var DBposts = DBSetFactory.GetLastInitPosts();

            Post post = DBposts.Where(p => p.PromotionEndDateTime.HasValue == false).FirstOrDefault();

            service.PromotePost(new PostPromotion
            {
                Duration = 7,
                ID = post.ID
            }, post.Creator.ID);

            mockPostsSet.Verify(m => m.Update(It.IsAny<Post>()), Times.Once);
            Assert.True(mockPostsSet.Object.Where(p => p.ID == post.ID).FirstOrDefault().PromotionEndDateTime > DateTime.Now.AddDays(6));
            Assert.True(mockPostsSet.Object.Where(p => p.ID == post.ID).FirstOrDefault().PromotionEndDateTime < DateTime.Now.AddDays(8));
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact(DisplayName = "Post promotion - once promoted, 7 days, success")]
        public void PromotePostOncePromoted7DaysSuccess()
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();

            Mock<DbSet<Post>> mockPostsSet = DBSetFactory.GetPostsMockDBSet();

            mockContext.Setup(c => c.Posts).Returns(mockPostsSet.Object);

            PostService service = new PostService(mockContext.Object);
            var DBposts = DBSetFactory.GetLastInitPosts();

            Post post = DBposts.Where(p => p.PromotionEndDateTime.HasValue == true && p.PromotionEndDateTime < DateTime.Now).FirstOrDefault();

            service.PromotePost(new PostPromotion
            {
                Duration = 7,
                ID = post.ID
            }, post.Creator.ID);

            mockPostsSet.Verify(m => m.Update(It.IsAny<Post>()), Times.Once);
            Assert.True(mockPostsSet.Object.Where(p => p.ID == post.ID).FirstOrDefault().PromotionEndDateTime > DateTime.Now.AddDays(6));
            Assert.True(mockPostsSet.Object.Where(p => p.ID == post.ID).FirstOrDefault().PromotionEndDateTime < DateTime.Now.AddDays(8));
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact(DisplayName = "Post promotion - 7 more days, success")]
        public void PromotePost7MoreDaysSuccess()
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();

            Mock<DbSet<Post>> mockPostsSet = DBSetFactory.GetPostsMockDBSet();

            mockContext.Setup(c => c.Posts).Returns(mockPostsSet.Object);

            PostService service = new PostService(mockContext.Object);
            var DBposts = DBSetFactory.GetLastInitPosts();

            Post post = DBposts.Where(p => p.PromotionEndDateTime.HasValue == true && p.PromotionEndDateTime > DateTime.Now).FirstOrDefault();
            DateTime actualPromotionDateTime = post.PromotionEndDateTime.Value;

            service.PromotePost(new PostPromotion
            {
                Duration = 7,
                ID = post.ID
            }, post.Creator.ID);

            mockPostsSet.Verify(m => m.Update(It.IsAny<Post>()), Times.Once);
            Assert.True(mockPostsSet.Object.Where(p => p.ID == post.ID).FirstOrDefault().PromotionEndDateTime > actualPromotionDateTime.AddDays(6));
            Assert.True(mockPostsSet.Object.Where(p => p.ID == post.ID).FirstOrDefault().PromotionEndDateTime < actualPromotionDateTime.AddDays(8));
            mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [Fact(DisplayName = "Post promotion - negative number of days, failure")]
        public void PromotePostNegativeDaysFailure()
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();

            Mock<DbSet<Post>> mockPostsSet = DBSetFactory.GetPostsMockDBSet();

            mockContext.Setup(c => c.Posts).Returns(mockPostsSet.Object);

            PostService service = new PostService(mockContext.Object);
            var DBposts = DBSetFactory.GetLastInitPosts();

            Post post = DBposts.FirstOrDefault();

            Assert.Throws<Exception>(() => service.PromotePost(new PostPromotion
            {
                Duration = -1,
                ID = post.ID
            }, post.Creator.ID));
        }

        [Fact(DisplayName = "Post promotion - post doesn't exist, failure")]
        public void PromotePostThatDoesntExistFailure()
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();

            Mock<DbSet<Post>> mockPostsSet = DBSetFactory.GetPostsMockDBSet();

            mockContext.Setup(c => c.Posts).Returns(mockPostsSet.Object);

            PostService service = new PostService(mockContext.Object);
            var DBposts = DBSetFactory.GetLastInitPosts();

            Post post = DBposts.FirstOrDefault();

            Assert.Throws<Exception>(() => service.PromotePost(new PostPromotion
            {
                Duration = -1,
                ID = -1
            }, post.Creator.ID));
        }

        [Fact(DisplayName = "Post promotion - not mine post, failure")]
        public void PromoteNotMinePostFailure()
        {
            Mock<SaleSystemDBContext> mockContext = new Mock<SaleSystemDBContext>();

            Mock<DbSet<Post>> mockPostsSet = DBSetFactory.GetPostsMockDBSet();

            mockContext.Setup(c => c.Posts).Returns(mockPostsSet.Object);

            PostService service = new PostService(mockContext.Object);
            var DBposts = DBSetFactory.GetLastInitPosts();

            Post post = DBposts.FirstOrDefault();

            Assert.Throws<Exception>(() => service.PromotePost(new PostPromotion
            {
                Duration = -1,
                ID = post.ID
            }, post.Creator.ID + 1));
        }

        [Fact]
        public void GetPostLikers()
        {
            var mockPosts = GetMockDbSet(examplePosts3.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);

            var service = new PostService(mockDBContext.Object);
            int postID = 1;

            var result = service.GetPostLikers(postID);

            Assert.Equal(exampleLikePost.Count, result.Count());
        }

        [Fact]
        public void GetPostLikers_NoLikers()
        {
            var mockPosts = GetMockDbSet(examplePosts2.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);

            var service = new PostService(mockDBContext.Object);
            int postID = 1;

            var result = service.GetPostLikers(postID);

            Assert.Empty(result);
        }

        [Fact]
        public void GetPostLikers_PostNotExists()
        {
            var mockPosts = GetMockDbSet(examplePosts2.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Posts).Returns(mockPosts.Object);

            var service = new PostService(mockDBContext.Object);
            int postID = 999;

            Assert.Throws<Exception>(() => service.GetPostLikers(postID));
        }

        [Fact]
        public void GetUserPosts()
        {
            var mockUsers = GetMockDbSet(exampleUsers.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Users).Returns(mockUsers.Object);

            var service = new PostService(mockDBContext.Object);
            int userID = 42;

            var result = service.GetUserPosts(userID);

            Assert.Equal(examplePostViews.Count, result.Count());
            for (int i = 0; i < examplePostViews.Count; i++)
            {
                Assert.Equal(examplePostViews[i], result.ToList()[i], new Comparers.PostViewComparer());
            }
        }

        [Fact]
        public void GetUserPosts_UserNotExists()
        {
            var mockUsers = GetMockDbSet(exampleUsers.AsQueryable());
            var mockDBContext = new Mock<SaleSystemDBContext>();
            mockDBContext.Setup(o => o.Users).Returns(mockUsers.Object);

            var service = new PostService(mockDBContext.Object);
            int userID = 999;

            Assert.Throws<Exception>(() => service.GetUserPosts(userID));
        }

        private static Mock<DbSet<T>> GetMockDbSet<T>(IQueryable<T> entities) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(entities.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(entities.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(entities.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(entities.GetEnumerator());
            return mockSet;
        }
    }
}
