using FluentValidation;
using Microsoft.Extensions.Localization;
using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;

namespace Project.Application.Features.Commands.DeleteTrack
{
    public class DeleteTrackCommandValidator : AbstractValidator<DeleteTrackCommandRequest>
    {
        private readonly CultureLocalizer _localizer;
        private readonly IRepositoryBase<Track> _trackRepository;

        public DeleteTrackCommandValidator(CultureLocalizer localizer, IRepositoryBase<Track> trackRepository)
        {
            _localizer = localizer;
            _trackRepository = trackRepository;

            RuleFor(x => x.TrackId)
                .NotEmpty().WithMessage(_localizer.Text("RequiredField", "Title"));
        }
    }
}

