using FluentValidation;
using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;

namespace Project.Application.Features.Commands.DeleteTrackComment;

public class DeleteTrackCommentCommandValidator : AbstractValidator<DeleteTrackCommentCommandRequest>
{
    public DeleteTrackCommentCommandValidator(
        CultureLocalizer localizer,
        IRepositoryBase<Track> trackRepository)
    {
        RuleFor(x => x.TrackId)
            .NotEmpty().WithMessage(localizer.Text("RequiredField", "TrackId"))
            .Must(x => trackRepository.Get(track => track.Id == x) != null)
            .WithMessage(localizer.Text("NotFound"));
    }
}
