using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SaleSystem.Database;
using SendGrid;
using SendGrid.Helpers.Mail;
using SaleSystem.Database.DatabaseModels;
using SaleSystem.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SaleSystem.Models.Newsletters;
using SaleSystem.Wrappers;

namespace SaleSystem.Services
{
    public class NewsletterService : INewsletterService
    {
        private readonly SaleSystemDBContext _context;
        private readonly SendGridWrapper _sendGridWrapper;
        public NewsletterService(SaleSystemDBContext context, SendGridWrapper sendGridWrapper)
        {
            _context = context;
            _sendGridWrapper = sendGridWrapper;
        }

        public void Subscribe(int userID, SubscriptionInfo info)
        {
            var subscriberCategory = _context.SubscribersCategories
                    .Where((sc) => sc.CategoryID == info.CategoryID && sc.SubscriberID == userID)
                    .FirstOrDefault();

            if (info.IsSubscribed)
            {
                if (subscriberCategory != null)
                    throw new InvalidOperationException($"Category subscription {info.CategoryID} for user {userID} already exists");

                var newSubscriberCategory = new SubscriberCategory
                {
                    SubscriberID = userID,
                    CategoryID = info.CategoryID
                };
                _context.SubscribersCategories.Add(newSubscriberCategory);
            }
            else
            {

                if (subscriberCategory == null)
                    throw new InvalidOperationException($"Category subscription {info.CategoryID} for user {userID} does not exist");
                _context.SubscribersCategories.Remove(subscriberCategory);
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new DbUpdateException("Cannot update database", ex);
            }
        }

        public async Task<bool> Notify(int categoryID)
        {
            try
            {
                List<EmailAddress> tos = new List<EmailAddress>();

                var subscriberCategory = _context.SubscribersCategories
                    .Include(sc => sc.Subscriber)
                    .Where(sc => sc.CategoryID == categoryID).AsQueryable();

                foreach (var sc in subscriberCategory)
                {
                    tos.Add(new EmailAddress(sc.Subscriber.EmailAddress, sc.Subscriber.Name));
                }

                await _sendGridWrapper.SendMails(tos);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
