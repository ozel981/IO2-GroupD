using System;

namespace SaleSystem.Models.Users
{
    [Serializable]
    public class UserView
    {
        public int ID { get; set; }
        public bool IsVerified { get; set; }
        public bool IsEntrepreneur { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }
}
