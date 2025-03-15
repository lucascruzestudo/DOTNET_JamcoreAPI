using Project.Application.Common.Localizers;

namespace Project.Application.Features.Commands.GetTrack;

public class GetTrackQueryValidator : AbstractValidator<GetTrackQuery>
{
    private readonly CultureLocalizer _localizer;

    public GetTrackQueryValidator(CultureLocalizer localizer)
    {

        _localizer = localizer;

        RuleFor(x => x.TrackId)
            .NotEmpty().WithMessage(_localizer.Text("RequiredField", "TrackId"));
    }
}
