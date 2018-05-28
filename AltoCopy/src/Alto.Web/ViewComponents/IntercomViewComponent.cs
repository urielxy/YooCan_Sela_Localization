using System;
using System.Globalization;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Alto.Dal;
using Alto.Logic.External;
using Alto.Models.Account;
using Alto.Models.Account.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Alto.Web.ViewComponents
{
    public class IntercomViewComponent : BaseViewComponent
    {
        private readonly IntercomOptions _options;

        public IntercomViewComponent(AltoDbContext context, MapperConfiguration mapperConfiguration, IOptions<IntercomOptions> options) : base(context, mapperConfiguration)
        {
            _options = options.Value;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new IntercomModel
            {
                AppId = _options.IntercomAppId,
                Email = UserClaimsPrincipal.FindFirstValue(ClaimTypes.Email)
            };
            var membershipExpiryString = UserClaimsPrincipal.FindFirstValue(UserMembership.MembershipExpiryDateClaimType);
            if (!string.IsNullOrEmpty(membershipExpiryString))
            {
                model.MembershipExpiryDate = DateTime.ParseExact(membershipExpiryString, "u", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
            }
            if (!string.IsNullOrEmpty(model.Email))
            {
                model.Token = CreateToken(model.Email, _options.IntercomAppSecret);
            }
            var firstName = UserClaimsPrincipal.FindFirstValue(ClaimTypes.GivenName);
            var lastName = UserClaimsPrincipal.FindFirstValue(ClaimTypes.Surname);
            if (!string.IsNullOrEmpty(firstName))
            {
                model.Name = $"{firstName} {lastName}";
            }

            return View(model);
        }

        private static string CreateToken(string message, string secret)
        {
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);

                var sb = new System.Text.StringBuilder();
                for (var i = 0; i <= hashmessage.Length - 1; i++)
                {
                    sb.Append(hashmessage[i].ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }
    }
}
