using FluentValidation;
using Microsoft.Extensions.Localization;
using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;

namespace Project.Application.Features.Commands.UpdateTrack
{
    public class UpdateTrackCommandValidator : AbstractValidator<UpdateTrackCommandRequest>
    {
        private readonly CultureLocalizer _localizer;
        private readonly IRepositoryBase<Track> _trackRepository;

        public UpdateTrackCommandValidator(CultureLocalizer localizer, IRepositoryBase<Track> trackRepository)
        {
            _localizer = localizer;
            _trackRepository = trackRepository;

            RuleFor(x => x.Tags)
                .Must(tags => tags == null || tags.All(tag => tag.Length <= 100))
                .WithMessage(_localizer.Text("MaxLength", "Tag", "100"));

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage(_localizer.Text("MaxLength", "Description", "255"))
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}

