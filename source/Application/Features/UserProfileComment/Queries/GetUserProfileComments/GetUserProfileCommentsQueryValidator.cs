using Project.Application.Common.Localizers;

namespace Project.Application.Features.Commands.GetUserProfileComments;

public class GetUserProfileCommentsQueryValidator : AbstractValidator<GetUserProfileCommentsQuery>
{
    public GetUserProfileCommentsQueryValidator(CultureLocalizer localizer)
    {
        RuleFor(x => x.UserProfileId)
            .NotEmpty().WithMessage(localizer.Text("RequiredField", "UserProfileId"));

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage(localizer.Text("GreaterThanZero", "PageNumber"));

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage(localizer.Text("GreaterThanZero", "PageSize"));
    }
}
