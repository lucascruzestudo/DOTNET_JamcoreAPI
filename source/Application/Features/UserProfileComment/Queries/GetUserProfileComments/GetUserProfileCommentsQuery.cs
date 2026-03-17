using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.GetUserProfileComments;

public class GetUserProfileCommentsQuery : Command<GetUserProfileCommentsQueryResponse>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public Guid UserProfileId { get; init; }

    public GetUserProfileCommentsQuery(Guid userProfileId, int pageNumber, int pageSize)
    {
        UserProfileId = userProfileId;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
