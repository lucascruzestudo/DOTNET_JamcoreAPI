using Project.Application.Common.Localizers;

namespace Project.Application.Features.Commands.GetTracksByTag;

public class GetTracksByTagQueryValidator : AbstractValidator<GetTracksByTagQuery>
{
    private readonly CultureLocalizer _localizer;

    public GetTracksByTagQueryValidator(CultureLocalizer localizer)
    {

        _localizer = localizer;

        RuleFor(x => x.Tag)
            .NotEmpty().WithMessage(_localizer.Text("RequiredField", "Tag"));

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage(_localizer.Text("GreaterThanZero", "PageNumber"));

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage(_localizer.Text("GreaterThanZero", "PageSize"));
    }
}
