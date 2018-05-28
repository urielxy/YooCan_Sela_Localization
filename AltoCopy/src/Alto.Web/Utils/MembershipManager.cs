using System;
using System.Globalization;
using System.Security.Claims;
using Alto.Models.Account.Claims;
using Alto.Web.Utils.Yoocan;
using Microsoft.AspNetCore.Http;

namespace Alto.Web.Utils
{
    public class MembershipManager
    {
        private readonly YoocanUsersManager _yoocanUsersManager;
        private readonly HttpContext _httpContext;

        public MembershipManager(IHttpContextAccessor httpContextAccessor, YoocanUsersManager yoocanUsersManager)
        {
            _yoocanUsersManager = yoocanUsersManager;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public MembershipState GetMembershipState()
        {
            var user = _httpContext.User;
            var yoocanUser = _yoocanUsersManager.GetYoocanUserCookie();
            if (yoocanUser?.WasAuthorized == true)
                return MembershipState.Payed;

            if (!user.Identity.IsAuthenticated)
            {
                return yoocanUser != null ? MembershipState.YoocanUnregistered : MembershipState.Unregistered;
            }
            var membershipStateString = user.FindFirstValue(UserMembership.MembershipStateClaimType);
            if (membershipStateString == null)
            {
                return MembershipState.Registered;
            }           
            var membershipState = (MembershipState)Enum.Parse(typeof(MembershipState), membershipStateString);
            if(membershipState == MembershipState.FilledDetails)
                return MembershipState.Payed;
            
            if (membershipState == MembershipState.Payed)
            {
                var membershipExpiryDate = DateTime.ParseExact(user.FindFirstValue(UserMembership.MembershipExpiryDateClaimType), "u", CultureInfo.InvariantCulture);
                if (membershipExpiryDate < DateTime.UtcNow
                        //prevent time zone issues
                        .AddDays(1))
                {
                    membershipState = MembershipState.Payed;
                }
            }
            return membershipState;
        }
    }
}
