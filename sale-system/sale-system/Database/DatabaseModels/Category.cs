using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleSystem.Database.DatabaseModels
{
    public class Category
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<SubscriberCategory> SubscriberCategory { get; set; }
    }
}
