using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;

namespace Project.Application.Features.Commands.UnfollowUser;

public class UnfollowUserCommandValidator : AbstractValidator<UnfollowUserCommandRequest>
{
    public UnfollowUserCommandValidator(
        CultureLocalizer localizer,
        IRepositoryBase<UserProfile> userProfileRepository)
    {
        RuleFor(x => x.FollowedUserId)
            .NotEmpty().WithMessage(localizer.Text("RequiredField", "FollowedUserId"))
            .Must(id => userProfileRepository.Get(p => p.UserId == id || p.Id == id) != null)
            .WithMessage(localizer.Text("NotFound"));
    }
}
