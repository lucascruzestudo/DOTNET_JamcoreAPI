using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.CreateTrackPlay;

public class CreateTrackPlayCommand(CreateTrackPlayCommandRequest request) : Command<CreateTrackPlayCommandResponse>, IIdempotentCommand
{
    public CreateTrackPlayCommandRequest Request { get; set; } = request;
}
