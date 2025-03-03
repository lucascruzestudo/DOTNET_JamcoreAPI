using FluentValidation;
using Microsoft.Extensions.Localization;
using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;

namespace Project.Application.Features.Commands.UploadTrack
{
    public class UploadTrackCommandValidator : AbstractValidator<UploadTrackCommandRequest>
    {
        private readonly CultureLocalizer _localizer;
        private readonly IRepositoryBase<Track> _trackRepository;

        public UploadTrackCommandValidator(CultureLocalizer localizer, IRepositoryBase<Track> trackRepository)
        {
            _localizer = localizer;
            _trackRepository = trackRepository;

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage(_localizer.Text("RequiredField", "Title"));

            RuleFor(x => x.IsPublic)
                .NotNull().WithMessage(_localizer.Text("RequiredField", "IsPublic"));

            RuleFor(x => x.Tags)
                .Must(tags => tags == null || tags.All(tag => tag.Length <= 100))
                .WithMessage(_localizer.Text("MaxLength", "Tag", "100"));

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage(_localizer.Text("MaxLength", "Description", "255"))
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}

