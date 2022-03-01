using Microsoft.EntityFrameworkCore;
using SaleSystem.Database.DatabaseModels;
using SaleSystem.Models.Newsletters;
using SaleSystem.Services;
using SaleSystem.Database;
using SaleSystemUnitTests.MockData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration;
using Moq;
using SaleSystem.Wrappers;

namespace SaleSystemUnitTests.ServicesTests
{
    public class NewsletterServiceTest : IDisposable
    {
        readonly ConnectionFactory _factory;
        readonly SaleSystemDBContext _context;
        readonly IConfiguration _configuration;
        readonly Mock<SendGridWrapper> _sendGridWrapper;
        public NewsletterServiceTest()
        {
            var inMemorySettings = new Dictionary<string, string> {
                {"SendGrid:SendGridKey", "APIKey"},
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _factory = new ConnectionFactory();
            _context = _factory.CreateContextForSQLite();

            _sendGridWrapper = new Mock<SendGridWrapper>(_configuration);

            var user = new User
            {
                Name = "John",
                EmailAddress = "john@smith.pl",
                Type = 0,
                IsActive = true,
                IsVerified = false,
                CreationDateTime = new DateTime(2021, 3, 21)
            };

            var category = new Category
            {
                Name = "RTV/AGD"
            };

            _context.Users.Add(user);
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        [Fact]
        public void Subscribe_ValidCall()
        {
            NewsletterService service = new NewsletterService(_context, _sendGridWrapper.Object);

            var user = _context.Users.FirstOrDefault();
            var category = _context.Categories.FirstOrDefault();

            var subscriptionInfo = new SubscriptionInfo
            {
                CategoryID = category.ID,
                IsSubscribed = true
            };

            service.Subscribe(user.ID, subscriptionInfo);

            var subscribersCategories = _context.SubscribersCategories.FirstOrDefault();

            Assert.Equal(1, _context.SubscribersCategories.Count());
            Assert.Equal(user.ID, subscribersCategories.SubscriberID);
            Assert.Equal(category.ID, subscribersCategories.CategoryID);
        }

        [Fact]
        public void Add_Subscription_When_Already_Exists()
        {
            NewsletterService service = new NewsletterService(_context, _sendGridWrapper.Object);

            var user = _context.Users.FirstOrDefault();
            var category = _context.Categories.FirstOrDefault();

            var subscriptionInfo = new SubscriptionInfo
            {
                CategoryID = category.ID,
                IsSubscribed = true
            };

            service.Subscribe(user.ID, subscriptionInfo);

            Assert.Throws<InvalidOperationException>(() => service.Subscribe(user.ID, subscriptionInfo));
        }

        [Fact]
        public void Delete_Subscription_ValidCall()
        {
            NewsletterService service = new NewsletterService(_context, _sendGridWrapper.Object);

            var user = _context.Users.FirstOrDefault();
            var category = _context.Categories.FirstOrDefault();

            SubscriberCategory sc = new SubscriberCategory
            {
                CategoryID = category.ID,
                SubscriberID = user.ID,
            };

            _context.SubscribersCategories.Add(sc);
            _context.SaveChanges();

            var subscriptionInfo = new SubscriptionInfo
            {
                CategoryID = category.ID,
                IsSubscribed = false
            };

            service.Subscribe(user.ID, subscriptionInfo);

            Assert.Equal(0, _context.SubscribersCategories.Count());
        }

        [Fact]
        public void Remove_Subscription_When_Not_Exists()
        {
            NewsletterService service = new NewsletterService(_context, _sendGridWrapper.Object);

            var user = _context.Users.FirstOrDefault();
            var category = _context.Categories.FirstOrDefault();

            var subscriptionInfo = new SubscriptionInfo
            {
                CategoryID = category.ID,
                IsSubscribed = false
            };

            Assert.Throws<InvalidOperationException>(() => service.Subscribe(user.ID, subscriptionInfo));
        }

        [Fact]
        public void Notify_Category_Without_Subscription()
        {
            NewsletterService service = new NewsletterService(_context, _sendGridWrapper.Object);

            var category = _context.Categories.FirstOrDefault();

            Task<bool> result = service.Notify(category.ID);

            result.Wait();

            Assert.True(result.Result);
        }

        [Fact]
        public void Notify_Category_With_Subscription()
        {
            NewsletterService service = new NewsletterService(_context, _sendGridWrapper.Object);

            var user = _context.Users.FirstOrDefault();
            var category = _context.Categories.FirstOrDefault();

            var subscriberCategory = new SubscriberCategory
            {
                CategoryID = category.ID,
                SubscriberID = user.ID,
            };

            _context.SubscribersCategories.Add(subscriberCategory);

            Task<bool> result = service.Notify(category.ID);

            result.Wait();

            Assert.True(result.Result);
        }

        public void Dispose()
        {
            _factory.Dispose();
        }
    }
}
