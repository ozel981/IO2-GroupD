using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleSystem.Database.DatabaseModels
{
    public class SubscriberCategory
    {
        public int SubscriberID { get; set; }
        public User Subscriber { get; set; }
        public int CategoryID { get; set; }
        public Category Category { get; set; }
    }
}
