using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace InstantChat.Application.Features.Chat.Queries.GetChatHistory;

public class GetChatHistoryQueryValidator : AbstractValidator<GetChatHistoryQuery>
{
    public GetChatHistoryQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(50).WithMessage("Page size cannot exceed 50.");

        RuleFor(x => x.Offset)
            .GreaterThanOrEqualTo(0).WithMessage("Offset must be greater than or equal to 0.");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.OtherUserId) || x.GroupChatId.HasValue)
            .WithMessage("Either OtherUserId or GroupChatId must be provided.");
    }
}
