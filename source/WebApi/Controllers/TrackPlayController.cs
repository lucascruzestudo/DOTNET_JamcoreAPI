using Microsoft.AspNetCore.Mvc;
using Project.Domain.Notifications;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using Project.Application.Features.Commands.CreateTrackPlay;
using Project.Application.Features.Queries.GetRecentTrackPlaysByUser;

namespace Project.WebApi.Controllers
{
    public class TrackPlayController(
        INotificationHandler<DomainNotification> notifications,
        INotificationHandler<DomainSuccessNotification> successNotifications,
        IHttpContextAccessor httpContextAccessor,
        IMediator mediatorHandler) : BaseController(notifications, successNotifications, mediatorHandler, httpContextAccessor)
    {
        private readonly IMediator _mediatorHandler = mediatorHandler;
        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        [SwaggerOperation(Summary = "Create a track play for the current user.")]
        [ProducesResponseType(typeof(CreateTrackPlayCommandResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateTrackPlay([FromBody] CreateTrackPlayCommandRequest request)
        {
            return Response(await _mediatorHandler.Send(new CreateTrackPlayCommand(request)));
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("byUser")]
        [SwaggerOperation(Summary = "Get recent track plays by user.")]
        [ProducesResponseType(typeof(GetRecentTrackPlaysByUserQueryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRecentTrackPlaysByUser([FromQuery] Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 3)
        {
            return Response(await _mediatorHandler.Send(new GetRecentTrackPlaysByUserQuery(userId, pageNumber, pageSize)));
        }
    }
}