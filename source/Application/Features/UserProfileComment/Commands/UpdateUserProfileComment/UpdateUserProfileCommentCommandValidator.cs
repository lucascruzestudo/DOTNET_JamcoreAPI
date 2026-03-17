using Project.Application.Common.Localizers;

namespace Project.Application.Features.Commands.UpdateUserProfileComment;

public class UpdateUserProfileCommentCommandValidator : AbstractValidator<UpdateUserProfileCommentCommandRequest>
{
    public UpdateUserProfileCommentCommandValidator(CultureLocalizer localizer)
    {
        RuleFor(x => x.CommentId)
            .NotEmpty().WithMessage(localizer.Text("RequiredField", "CommentId"));

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage(localizer.Text("RequiredField", "Comment"))
            .MaximumLength(500).WithMessage(localizer.Text("MaxLength", "Comment", 500));
    }
}
