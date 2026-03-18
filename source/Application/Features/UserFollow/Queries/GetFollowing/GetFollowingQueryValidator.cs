using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;

namespace Project.Application.Features.Commands.GetFollowing;

public class GetFollowingQueryValidator : AbstractValidator<GetFollowingQuery>
{
    public GetFollowingQueryValidator(
        CultureLocalizer localizer,
        IRepositoryBase<UserProfile> userProfileRepository)
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage(localizer.Text("RequiredField", "UserId"))
            .Must(id => userProfileRepository.Get(p => p.UserId == id || p.Id == id) != null)
            .WithMessage(localizer.Text("NotFound"));

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage(localizer.Text("RequiredField", "PageNumber"));

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage(localizer.Text("RequiredField", "PageSize"));
    }
}
