using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace InstantChat.Application.Features.Chat.Commands.SendMessage;

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("Sender ID is required.");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.ReceiverId) || x.GroupChatId.HasValue)
            .WithMessage("Either ReceiverId or GroupChatId must be provided.");

        // At least content or image file must be provided
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Content) || x.ImageFile != null)
            .WithMessage("Either message content or image file must be provided.");

        RuleFor(x => x.Content)
            .MaximumLength(1000).WithMessage("Message content cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Content));

        // Image file validation
        RuleFor(x => x.ImageFile)
            .Must(BeValidImageFile)
            .WithMessage("Invalid image file. Only JPG, JPEG, PNG, GIF files up to 5MB are allowed.")
            .When(x => x.ImageFile != null);
    }

    private bool BeValidImageFile(Microsoft.AspNetCore.Http.IFormFile? file)
    {
        if (file == null) return true;

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return allowedExtensions.Contains(fileExtension) &&
               file.Length > 0 &&
               file.Length <= 5 * 1024 * 1024; // 5MB max
    }
}