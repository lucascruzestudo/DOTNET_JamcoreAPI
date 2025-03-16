using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.GetUserProfileById;

public class GetUserProfileByIdQuery(Guid id) : Command<GetUserProfileByIdQueryResponse>
{
    public Guid Id { get; set; } = id;
}
