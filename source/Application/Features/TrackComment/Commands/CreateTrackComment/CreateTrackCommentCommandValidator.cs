using FluentValidation;
using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;

namespace Project.Application.Features.Commands.CreateTrackComment;

public class CreateTrackCommentCommandValidator : AbstractValidator<CreateTrackCommentCommandRequest>
{
    public CreateTrackCommentCommandValidator(
        CultureLocalizer localizer, 
        IRepositoryBase<Track> trackRepository, 
        IRepositoryBase<TrackComment> commentRepository)
    {
        RuleFor(x => x.TrackId)
            .NotEmpty().WithMessage(localizer.Text("RequiredField", "TrackId"))
            .Must(x => trackRepository.Get(track => track.Id == x) != null)
            .WithMessage(localizer.Text("NotFound"));

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage(localizer.Text("RequiredField", "Comment"))
            .MaximumLength(500).WithMessage(localizer.Text("MaxLength", "Comment", 500));
    }
}
