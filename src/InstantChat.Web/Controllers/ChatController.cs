using InstantChat.Application.Features.Chat.Commands.SendMessage;
using InstantChat.Application.Features.Chat.Queries.GetChatHistory;
using InstantChat.Application.Features.Groups.Commands.CreateGroup;
using InstantChat.Application.Features.Groups.Queries.GetOnlineUsers;
using InstantChat.Application.Features.Groups.Queries.GetUserGroups;
using InstantChat.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InstantChat.Web.Controllers;

[Authorize]
public class ChatController : Controller
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatController(IMediator mediator, UserManager<ApplicationUser> userManager)
    {
        _mediator = mediator;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return RedirectToAction("Login", "Account");

        var query = new GetOnlineUsersQuery { CurrentUserId = currentUser.Id };
        var result = await _mediator.Send(query);

        ViewBag.Users = result.Users;
        ViewBag.CurrentUserId = currentUser.Id;
        ViewBag.CurrentUserName = currentUser.DisplayName;

        return View();
    }

    public async Task<IActionResult> Groups()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return RedirectToAction("Login", "Account");

        var query = new GetUserGroupsQuery { UserId = currentUser.Id };
        var result = await _mediator.Send(query);

        ViewBag.UserGroups = result.Groups;
        ViewBag.CurrentUserId = currentUser.Id;
        ViewBag.CurrentUserName = currentUser.DisplayName;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroup(string groupName, string? description)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null || string.IsNullOrWhiteSpace(groupName))
            return BadRequest();

        var command = new CreateGroupCommand
        {
            Name = groupName,
            Description = description,
            CreatedById = currentUser.Id
        };

        var result = await _mediator.Send(command);

        if (result.Success)
            return RedirectToAction("Groups");

        TempData["Error"] = result.Message;
        return RedirectToAction("Groups");
    }

    [HttpGet]
    public async Task<IActionResult> GetChatHistory(string? userId, int? groupId, int offset = 0, int? beforeMessageId = null)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return BadRequest("User not authenticated");

        var query = new GetChatHistoryQuery
        {
            UserId = currentUser.Id,
            OtherUserId = userId,
            GroupChatId = groupId,
            Offset = offset,
            BeforeMessageId = beforeMessageId,
            PageSize = 20
        };

        var result = await _mediator.Send(query);

        if (result.Success)
        {
            return Json(new
            {
                success = true,
                messages = result.Messages.Select(m => new
                {
                    id = m.Id,
                    senderId = m.SenderId,
                    senderName = m.SenderName,
                    content = m.Content,
                    timestamp = m.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    type = m.Type.ToString(),
                    imageUrl = m.ImageUrl,
                    originalFileName = m.OriginalFileName,
                    fileSize = m.FileSize
                }),
                hasMoreMessages = result.HasMoreMessages,
                totalCount = result.TotalCount
            });
        }

        return BadRequest(new { success = false, message = result.Message });
    }

    [HttpPost]
    public async Task<IActionResult> SendImageMessage([FromForm] IFormFile imageFile, [FromForm] string? receiverId,
        [FromForm] int? groupId, [FromForm] string? caption)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
            return BadRequest("User not authenticated");

        if (imageFile == null)
            return BadRequest("No image file provided");

        var command = new SendMessageCommand
        {
            SenderId = currentUser.Id,
            ReceiverId = receiverId,
            GroupChatId = groupId,
            Content = caption ?? string.Empty,
            ImageFile = imageFile
        };

        var result = await _mediator.Send(command);

        if (result.Success)
        {
            return Json(new
            {
                success = true,
                message = result.Message,
                messageId = result.MessageId,
                imageUrl = result.ImagePath,
                messageType = result.MessageType.ToString()
            });
        }

        return BadRequest(new { success = false, message = result.Message });
    }
}
