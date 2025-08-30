using FluentValidation;
using InstantChat.Application.Features.Groups.Commands.CreateGroup;

namespace InstantChat.Application.Features.Groups.Commands.AddUser;
public class AddUserToGroupCommandValidator: AbstractValidator<AddUserToGroupCommand>
{
    public AddUserToGroupCommandValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty().WithMessage("Group ID must be provided.");
        RuleFor(x => x.UserId)
            .NotNull()
            .Must(ids => ids.Any())
            .WithMessage("At least one User ID must be provided.");
        RuleForEach(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID cannot be empty.");
    }
}
