using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Yooocan.Dal;
using Yooocan.Entities;
using Yooocan.Enums.Notifications;
using Yooocan.Logic.Messaging;
using Yooocan.Models;
using Yooocan.Models.New.Messages;

namespace Yooocan.Logic
{
    public class PrivateMessageLogic : IPrivateMessageLogic
    {
        private readonly ApplicationDbContext _context;
        //private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly INotificationLogic _notificationLogic;
        private readonly IEmailLogic _emailLogic;
        private readonly IEmailSender _emailSender;

        public PrivateMessageLogic(ApplicationDbContext context, /*ILogger<PrivateMessageLogic> logger,*/
            IMapper mapper, INotificationLogic notificationLogic, IEmailLogic emailLogic,
            IEmailSender emailSender)
        {
            _context = context;
            //_logger = logger;
            _mapper = mapper;
            _notificationLogic = notificationLogic;
            _emailLogic = emailLogic;
            _emailSender = emailSender;
        }

        public async Task SendMessageAsync(ClaimsPrincipal currentUser, PrivateMessageModel messageModel)
        {
            //TODO: Validate whether the recipient user accepted to receive private messages
            messageModel.FromUserId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);

            var message = _mapper.Map<PrivateMessage>(messageModel);
            _context.PrivateMessages.Add(message);
            await _context.SaveChangesAsync();

            await SendNotificationAsync(currentUser, messageModel.ToUserId);
        }

        private async Task SendNotificationAsync(ClaimsPrincipal currentUser, string targetUserId)
        {
            var notification = _notificationLogic.CreateNotification(currentUser, NotificationType.MessageNew);
            notification.AddRecipient(targetUserId);
            //TODO: fix the hardcoded path
            notification.Link = "/Messages";
            await _notificationLogic.SendNotificationAsync(notification);
        }

        public async Task<ConversationModel> GetConversationAsync(string currentUserId, string otherUserId)
        {
            var privateMessages = await _context.PrivateMessages
                .Include(x => x.FromUser)
                .Where(x => (x.FromUserId == currentUserId && x.ToUserId == otherUserId && !x.IsDeletedBySender)
                            || (x.ToUserId == currentUserId && x.FromUserId == otherUserId && !x.IsDeletedByRecipient))
                .ToListAsync();
            
            
            var model = new ConversationModel
                        {
                            Read = _mapper.Map<List<ConversationMessageModel>>(privateMessages.Where(x => x.ReadDate != null || x.FromUserId == currentUserId)),
                            NotRead = _mapper.Map<List<ConversationMessageModel>>(privateMessages.Where(x => x.ReadDate == null && x.ToUserId == currentUserId))
                        };

            var newMessages = privateMessages.Where(x => x.ToUserId == currentUserId && x.ReadDate == null).ToList();
            foreach (var message in newMessages)
            {
                message.ReadDate = DateTime.UtcNow;
            }

            if (newMessages.Any())
                await _context.SaveChangesAsync();
            return model;
        }

        public async Task<IList<PreviewModel>> GetConversationsAsync(string currentUserId, string userId)
        {
            var conversations = await _context.PrivateMessages.FromSql(
                    @"SELECT * 
FROM   (SELECT *, 
               rowid1 = Row_number() 
                          OVER ( 
                            PARTITION BY t.OtherUserId 
                            ORDER BY t.InsertDate DESC) 
        FROM   (SELECT *, 
                       OtherUserId = CASE 
                                       WHEN 
                       touserid = {0}
                                     THEN 
                                       FromUserId 
                                       ELSE ToUserId 
                                     END, 
                       rowid = Row_number() 
                                 OVER ( 
                                   PARTITION BY PM.touserid, PM.FromUserId 
                                   ORDER BY PM.InsertDate DESC) 
                FROM   [dbo].[PrivateMessages] PM 
                WHERE  PM.FromUserId = {0} 
                        OR PM.ToUserId = {0}) t 
        WHERE  rowid = 1) tt 
WHERE  rowid1 = 1", currentUserId)
                .Include(x => x.FromUser)
                .Include(x => x.ToUser)
                .ToListAsync();

            var results = conversations
                .OrderByDescending(x => x.InsertDate)
                .Select(x =>
                {
                    var other = x.FromUserId == currentUserId ? x.ToUser : x.FromUser;
                    return new PreviewModel
                           {
                               Content = x.Content,
                               LastMessageDate = x.InsertDate,
                               OtherUserId = other.Id,
                               OtherName = $"{other.FirstName} {other.LastName}",
                               OtherAvatar = other.PictureUrl
                           };
                }).ToList();
            if (userId != null)
            {
                if (results.Any(x => x.OtherUserId == userId))
                {
                    results = results.OrderBy(x => x.OtherUserId != userId).ToList();
                }
                else
                {
                    var previewModel = await _context.Users.Where(x => x.Id == userId).Select(x => new PreviewModel
                                                                                                   {
                                                                                                       Content = "",
                                                                                                       OtherUserId = userId,
                                                                                                       LastMessageDate = null,
                                                                                                       OtherName = x.FirstName + " " + x.LastName,
                                                                                                       OtherAvatar = x.PictureUrl
                                                                                                   }).SingleAsync();
                    results.Insert(0, previewModel);
                }
            }
            return results;
        }

        public async Task SendNotificationsOnIncomingMessageAsync()
        {
            var now = DateTime.UtcNow;
            var messages = await _context.PrivateMessages.Where(x => x.InsertDate.AddMinutes(10) < now && x.ReadDate == null).ToListAsync();
            var count = messages.Count;
            await _emailSender.SendEmailAsync(null, "evgeny@yoocantech.com", "test PM", $"Messages count {count}", null, null);
        }
    }
}
