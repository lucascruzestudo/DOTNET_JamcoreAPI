using Project.Application.Common.Models;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.GetUserProfileComments;

public record GetUserProfileCommentsQueryResponse
{
    public PaginatedList<UserProfileCommentViewModel> Comments { get; set; } = default!;
}
