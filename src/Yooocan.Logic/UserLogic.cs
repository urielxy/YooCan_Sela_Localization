using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Yooocan.Dal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Yooocan.Entities;
using Yooocan.Logic.Extensions;
using Yooocan.Models;
using Yooocan.Models.Users;

namespace Yooocan.Logic
{
    public class UserLogic : IUserLogic
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IBlobUploader _blobUploader;
        private readonly IDatabase _redisDatabase;

        public UserLogic(ApplicationDbContext context, ILogger<UserLogic> logger, IMapper mapper, UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, IBlobUploader blobUploader, IDatabase redisDatabase)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _blobUploader = blobUploader;
            _redisDatabase = redisDatabase;
        }

        public async Task EditBioAsync(UserBioModel model, bool adminChange)
        {
            var trimmedAboutMe = model.AboutMe?.Trim();
            if (!string.IsNullOrEmpty(trimmedAboutMe))
            {
                if (new Regex("[A-Z0-9]$", RegexOptions.IgnoreCase).IsMatch(trimmedAboutMe))
                    trimmedAboutMe += ".";
            }
            var user = await _userManager.FindByIdAsync(model.Id);

            user.FirstName = model.FirstName?.Trim().FirstLetterToUpper() ?? "";
            user.LastName = model.LastName?.Trim().FirstLetterToUpper() ?? "";
            user.AboutMe = trimmedAboutMe?.FirstLetterToUpper();
            user.Location = model.Location?.Trim().FirstLetterToUpper();
            user.City = model.City;
            user.PostalCode = model.PostalCode;
            user.State = model.State;
            user.Country = model.Country;
            user.Latitude = model.Latitude;
            user.Longitude = model.Longitude;

            if (adminChange && !string.IsNullOrEmpty(model.Email) && model.Email.Trim() != user.Email)
            {
                model.Email = model.Email.Trim();
                user.Email = model.Email;
                user.UserName = model.Email;
                user.NormalizedEmail = model.Email.ToUpperInvariant();
                user.NormalizedUserName = model.Email.ToUpperInvariant();
            }

            if (model.PictureDataUri != null)
            {
                user.PictureUrl = "";
                //TODO: delete previous picture
            }
            if (model.HeaderImageDataUri != null)
            {
                user.HeaderImageUrl = "";
                //TODO: delete previous picture
            }
            const string containerName = "user-images";
            if (!string.IsNullOrWhiteSpace(model.PictureDataUri))
            {
                user.PictureUrl = await _blobUploader.UploadDataUriImage(model.PictureDataUri, containerName);
            }
            if (!string.IsNullOrWhiteSpace(model.HeaderImageDataUri))
            {
                user.HeaderImageUrl = await _blobUploader.UploadDataUriImage(model.HeaderImageDataUri, containerName);
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var pictureClaim = claims.FirstOrDefault(x => x.Type == "picture");
            var givenNameClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName);
            var surnameClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname);
            var claimsToAdd = new List<Claim>();
            var needToRefreshToken = false;
            if (pictureClaim == null && user.PictureUrl != null)
            {
                needToRefreshToken = true;
                claimsToAdd.Add(new Claim("picture", user.PictureUrl ?? ""));
            }
            else if (pictureClaim != null && pictureClaim.Value != user.PictureUrl)
            {
                needToRefreshToken = true;
                if (!string.IsNullOrWhiteSpace(user.PictureUrl))
                {
                    await _userManager.ReplaceClaimAsync(user, pictureClaim, new Claim("picture", user.PictureUrl));
                }
                else
                {
                    await _userManager.RemoveClaimAsync(user, pictureClaim);
                }
            }

            if (givenNameClaim == null)
            {
                needToRefreshToken = true;
                claimsToAdd.Add(new Claim(ClaimTypes.GivenName, user.FirstName));
            }
            else if (givenNameClaim.Value != user.FirstName)
            {
                needToRefreshToken = true;
                await _userManager.ReplaceClaimAsync(user, givenNameClaim, new Claim(ClaimTypes.GivenName, user.FirstName ?? ""));
            }

            if (surnameClaim == null)
            {
                needToRefreshToken = true;
                claimsToAdd.Add(new Claim(ClaimTypes.Surname, user.LastName));
            }
            else if (surnameClaim.Value != user.LastName)
            {
                needToRefreshToken = true;
                await _userManager.ReplaceClaimAsync(user, surnameClaim, new Claim(ClaimTypes.Surname, user.LastName ?? ""));
            }

            if (claimsToAdd.Any())
                await _userManager.AddClaimsAsync(user, claimsToAdd);

            if (!adminChange && needToRefreshToken)
                await _signInManager.RefreshSignInAsync(user);
            var storiesIds = _context.Stories.Where(x => x.UserId == user.Id).Select(x => x.Id).ToList();
            foreach (var storyId in storiesIds)
            {
                var key = string.Format(RedisKeys.StoryModel, storyId);
                _redisDatabase.HashDelete(key, new RedisValue[] {RedisKeys.Main, RedisKeys.Comments}, CommandFlags.FireAndForget);
            }
            _context.SaveChanges();
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new DBConcurrencyException();
            }            
        }

        public async Task<List<FollowingModel>> GetFollowingAsync(string userId)
        {
            return await _context.Followers
                .Where(x => x.FollowerUserId == userId && !x.IsDeleted)
                .Select(x => new FollowingModel
                {
                    FirstName = x.FollowedUser.FirstName,
                    LastName = x.FollowedUser.LastName,
                    UserId = x.FollowedUserId,
                    ProfilePictureUrl = x.FollowedUser.PictureUrl
                })
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<FollowerModel>> GetFollowersAsync(string userId)
        {
            var followings = await _context.Followers
                .Where(x => x.FollowerUserId == userId && !x.IsDeleted)
                .Select(x => x.FollowedUserId)
                .Distinct().
                ToDictionaryAsync(x => x, y => true);

            return await _context.Followers
                .Where(x => x.FollowedUserId == userId)
                .Select(x => new FollowerModel
                {
                    FirstName = x.FollowerUser.FirstName,
                    LastName = x.FollowerUser.LastName,
                    UserId = x.FollowerUserId,
                    ProfilePictureUrl = x.FollowerUser.PictureUrl,
                    IsFollowed = followings.ContainsKey(x.FollowerUserId)
                })
                .Distinct()
                .ToListAsync();
        }

        public async Task<bool> SubscribeToNewsletterAsync(string email, string ipAddress)
        {
            email = email.ToLowerInvariant();
            var subscruber = await _context.NewsletterSubscribers.SingleOrDefaultAsync(x => x.Email == email);
            if (subscruber != null && !subscruber.Unsubscribed)
                return false;

            if (subscruber != null)
                subscruber.Unsubscribed = false;
            else
                _context.NewsletterSubscribers.Add(new NewsletterSubscriber
                                                   {
                                                       Email = email,
                                                       IpAddress = ipAddress
                                                   });
            await _context.SaveChangesAsync();
            return true;
        }
    }
}