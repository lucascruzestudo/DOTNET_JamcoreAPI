using Microsoft.AspNetCore.Mvc;
using Project.Domain.Notifications;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using Project.Application.Features.Commands.UpdateTrackLike;

namespace Project.WebApi.Controllers
{
    public class TrackLikeController(
        INotificationHandler<DomainNotification> notifications,
        INotificationHandler<DomainSuccessNotification> successNotifications,
        IHttpContextAccessor httpContextAccessor,
        IMediator mediatorHandler) : BaseController(notifications, successNotifications, mediatorHandler, httpContextAccessor)
    {
        private readonly IMediator _mediatorHandler = mediatorHandler;
        [Authorize(Roles = "Admin, User")]
        [HttpPut]
        [SwaggerOperation(Summary = "Toggle a track like for the current user.")]
        [ProducesResponseType(typeof(UpdateTrackLikeCommandResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTrackLike([FromBody] UpdateTrackLikeCommandRequest request)
        {
            return Response(await _mediatorHandler.Send(new UpdateTrackLikeCommand(request)));
        }
    }
}