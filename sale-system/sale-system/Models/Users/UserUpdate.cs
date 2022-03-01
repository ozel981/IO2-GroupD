using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleSystem.Models.Users
{
    public class UserUpdate
    {
        public bool IsVerified { get; set; }
        public bool IsEntrepreneur { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }
}
