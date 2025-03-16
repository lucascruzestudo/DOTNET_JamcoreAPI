using FluentValidation;
using Project.Application.Common.Localizers;

namespace Project.Application.Features.Commands.GetUserProfileById;

public class GetUserProfileByIdQueryValidator : AbstractValidator<GetUserProfileByIdQuery>
{
            private readonly CultureLocalizer _localizer;


    public GetUserProfileByIdQueryValidator(CultureLocalizer localizer)
    {
        _localizer = localizer;

        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(_localizer.Text("RequiredField", "Id"));

    }
}
