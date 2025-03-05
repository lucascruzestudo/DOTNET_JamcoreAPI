using Project.Application.Common.Localizers;

namespace Project.Application.Features.Commands.GetTracksByUser;

public class GetTracksByUserQueryValidator : AbstractValidator<GetTracksByUserQuery>
{
    private readonly CultureLocalizer _localizer;

    public GetTracksByUserQueryValidator(CultureLocalizer localizer)
    {

        _localizer = localizer;

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage(_localizer.Text("RequiredField", "Username"));

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage(_localizer.Text("GreaterThanZero", "PageNumber"));

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage(_localizer.Text("GreaterThanZero", "PageSize"));
    }
}
