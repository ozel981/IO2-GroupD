using SaleSystem.Models.Newsletters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleSystem.Services.Interfaces
{
    public interface INewsletterService
    {
        void Subscribe(int userID, SubscriptionInfo info);
        Task<bool> Notify(int categoryID);
    }
}
