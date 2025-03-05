using Project.Application.Common.Localizers;

namespace Project.Application.Features.Commands.GetRecentTracks;

public class GetRecentTracksQueryValidator : AbstractValidator<GetRecentTracksQuery>
{
    private readonly CultureLocalizer _localizer;

    public GetRecentTracksQueryValidator(CultureLocalizer localizer)
    {

        _localizer = localizer;

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage(_localizer.Text("GreaterThanZero", "PageNumber"));

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage(_localizer.Text("GreaterThanZero", "PageSize"));
    }
}
