using FluentValidation;

namespace InstantChat.Application.Features.Groups.Commands.CreateGroup;

public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
{
    public CreateGroupCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Group name is required.")
            .MaximumLength(100).WithMessage("Group name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Group description cannot exceed 500 characters.");

        RuleFor(x => x.CreatedById)
            .NotEmpty().WithMessage("Creator ID is required.");
    }
}
