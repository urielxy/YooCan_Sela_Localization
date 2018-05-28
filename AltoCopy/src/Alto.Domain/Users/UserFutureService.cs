using System;
using Alto.Enums;

namespace Alto.Domain.Users
{
    public class UserFutureService
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public FutureService FutureService { get; set; }

        public AltoUser User { get; set; }
        public int? UserId { get; set; }
        public string Ip { get; set; }
        public DateTime InsertDate { get; set; }
    }
}