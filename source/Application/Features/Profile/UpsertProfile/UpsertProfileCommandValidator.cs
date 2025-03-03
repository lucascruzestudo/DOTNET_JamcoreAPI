using Project.Application.Common.Localizers;

namespace Project.Application.Features.Commands.UpsertProfile
{
    public class UpsertProfileCommandValidator : AbstractValidator<UpsertProfileCommandRequest>
    {
        private readonly CultureLocalizer _localizer;

        public UpsertProfileCommandValidator(CultureLocalizer localizer)
        {
            _localizer = localizer;

            RuleFor(x => x.DisplayName)
                .MaximumLength(100).WithMessage(_localizer.Text("MaxLength", "DisplayName", 100))
                .When(x => !string.IsNullOrEmpty(x.DisplayName));

            RuleFor(x => x.Bio)
                .MaximumLength(500).WithMessage(_localizer.Text("MaxLength", "Bio", 500))
                .When(x => !string.IsNullOrEmpty(x.Bio));

            RuleFor(x => x.Location)
                .MaximumLength(200).WithMessage(_localizer.Text("MaxLength", "Location", 200))
                .When(x => !string.IsNullOrEmpty(x.Location));
        }
    }
}
