using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;

namespace Project.Application.Features.Commands.UpdateTrackLike;

public class UpdateTrackLikeCommandValidator : AbstractValidator<UpdateTrackLikeCommand>
{
    public readonly CultureLocalizer _localizer;
    private readonly IRepositoryBase<Track> _trackRepository;
    public UpdateTrackLikeCommandValidator(CultureLocalizer localizer, IRepositoryBase<Track> trackRepository)
    {
        _localizer = localizer;
        _trackRepository = trackRepository;

        RuleFor(x => x.Request)
                .NotNull().WithMessage(_localizer.Text("RequiredField", "Request"))
                .DependentRules(() =>
                {
                    RuleFor(x => x.Request.TrackId)
                        .NotEmpty().WithMessage(_localizer.Text("RequiredField", "TrackId"))
                        .Must(x =>
                        {
                            var existingTrack = _trackRepository.Get(track => track.Id == x);
                            return existingTrack != null;
                        }).WithMessage(_localizer.Text("NotFound"));
                });
    }
}
