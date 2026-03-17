using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;

namespace Project.Application.Features.Commands.CreateUserProfileComment;

public class CreateUserProfileCommentCommandValidator : AbstractValidator<CreateUserProfileCommentCommandRequest>
{
    public CreateUserProfileCommentCommandValidator(
        CultureLocalizer localizer,
        IRepositoryBase<UserProfile> userProfileRepository)
    {
        RuleFor(x => x.UserProfileId)
            .NotEmpty().WithMessage(localizer.Text("RequiredField", "UserProfileId"))
            .Must(x => userProfileRepository.Get(profile => profile.Id == x || profile.UserId == x) != null)
            .WithMessage(localizer.Text("NotFound"));

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage(localizer.Text("RequiredField", "Comment"))
            .MaximumLength(500).WithMessage(localizer.Text("MaxLength", "Comment", 500));
    }
}
