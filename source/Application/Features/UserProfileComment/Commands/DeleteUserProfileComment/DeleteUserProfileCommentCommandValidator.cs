using Project.Application.Common.Localizers;

namespace Project.Application.Features.Commands.DeleteUserProfileComment;

public class DeleteUserProfileCommentCommandValidator : AbstractValidator<DeleteUserProfileCommentCommandRequest>
{
    public DeleteUserProfileCommentCommandValidator(CultureLocalizer localizer)
    {
        RuleFor(x => x.CommentId)
            .NotEmpty().WithMessage(localizer.Text("RequiredField", "CommentId"));
    }
}
