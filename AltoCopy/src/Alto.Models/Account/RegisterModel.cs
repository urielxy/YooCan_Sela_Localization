using Alto.Models.Account.Claims;

namespace Alto.Models.Account
{
    public class RegisterModel
    {
        public UserDetailsModel UserDetailsModel { get; set; }
        public PaymentModel PaymentModel { get; set; }
        public MembershipState MembershipState { get; set; }
    }
}
