using Microsoft.AspNetCore.Mvc;
using Project.Domain.Notifications;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using Project.Application.Features.Commands.CreateTrackComment;
using Project.Application.Features.Commands.DeleteTrackComment;
using Project.Application.Features.Commands.GetRecentCommentsByUser;

namespace Project.WebApi.Controllers
{
    public class TrackCommentController(
        INotificationHandler<DomainNotification> notifications,
        INotificationHandler<DomainSuccessNotification> successNotifications,
        IHttpContextAccessor httpContextAccessor,
        IMediator mediatorHandler) : BaseController(notifications, successNotifications, mediatorHandler, httpContextAccessor)
    {
        private readonly IMediator _mediatorHandler = mediatorHandler;
        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        [SwaggerOperation(Summary = "Create a track comment.")]
        [ProducesResponseType(typeof(CreateTrackCommentCommandResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateTrackComment([FromBody] CreateTrackCommentCommandRequest request)
        {
            return Response(await _mediatorHandler.Send(new CreateTrackCommentCommand(request)));
        }

        [Authorize(Roles = "Admin, User")]
        [HttpDelete]
        [SwaggerOperation(Summary = "Delete a track comment.")]
        [ProducesResponseType(typeof(DeleteTrackCommentCommandResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteTrackComment([FromBody] DeleteTrackCommentCommandRequest request)
        {
            return Response(await _mediatorHandler.Send(new DeleteTrackCommentCommand(request)));
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("byUser")]
        [SwaggerOperation(Summary = "Get public tracks for a tag.")]
        [ProducesResponseType(typeof(GetRecentCommentsByUserQueryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRecentCommentsByUser([FromQuery] Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 3)
        {
            return Response(await _mediatorHandler.Send(new GetRecentCommentsByUserQuery(pageNumber, pageSize, userId)));
        }
    }
}