using Project.Application.Common.Localizers;

namespace Project.Application.Features.Commands.GetRecentCommentsByUser;

public class GetRecentCommentsByUserQueryValidator : AbstractValidator<GetRecentCommentsByUserQuery>
{
    private readonly CultureLocalizer _localizer;

    public GetRecentCommentsByUserQueryValidator(CultureLocalizer localizer)
    {

        _localizer = localizer;

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage(_localizer.Text("GreaterThanZero", "PageNumber"));

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage(_localizer.Text("GreaterThanZero", "PageSize"));
    }
}
