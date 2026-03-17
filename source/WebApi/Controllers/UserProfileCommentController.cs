using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Application.Features.Commands.CreateUserProfileComment;
using Project.Application.Features.Commands.DeleteUserProfileComment;
using Project.Application.Features.Commands.GetUserProfileComments;
using Project.Application.Features.Commands.UpdateUserProfileComment;
using Project.Domain.Notifications;
using Swashbuckle.AspNetCore.Annotations;

namespace Project.WebApi.Controllers;

public class UserProfileCommentController(
    INotificationHandler<DomainNotification> notifications,
    INotificationHandler<DomainSuccessNotification> successNotifications,
    IHttpContextAccessor httpContextAccessor,
    IMediator mediatorHandler) : BaseController(notifications, successNotifications, mediatorHandler, httpContextAccessor)
{
    private readonly IMediator _mediatorHandler = mediatorHandler;

    [Authorize(Roles = "Admin, User")]
    [HttpPost]
    [SwaggerOperation(Summary = "Create a user profile comment.")]
    [ProducesResponseType(typeof(CreateUserProfileCommentCommandResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateUserProfileComment([FromBody] CreateUserProfileCommentCommandRequest request)
    {
        return Response(await _mediatorHandler.Send(new CreateUserProfileCommentCommand(request)));
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPut]
    [SwaggerOperation(Summary = "Update a user profile comment.")]
    [ProducesResponseType(typeof(UpdateUserProfileCommentCommandResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUserProfileComment([FromBody] UpdateUserProfileCommentCommandRequest request)
    {
        return Response(await _mediatorHandler.Send(new UpdateUserProfileCommentCommand(request)));
    }

    [Authorize(Roles = "Admin, User")]
    [HttpDelete]
    [SwaggerOperation(Summary = "Delete a user profile comment.")]
    [ProducesResponseType(typeof(DeleteUserProfileCommentCommandResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteUserProfileComment([FromBody] DeleteUserProfileCommentCommandRequest request)
    {
        return Response(await _mediatorHandler.Send(new DeleteUserProfileCommentCommand(request)));
    }

    [Authorize(Roles = "Admin, User")]
    [HttpGet("byProfile")]
    [SwaggerOperation(Summary = "Get comments by user profile.")]
    [ProducesResponseType(typeof(GetUserProfileCommentsQueryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserProfileComments(
        [FromQuery] Guid userProfileId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        return Response(await _mediatorHandler.Send(new GetUserProfileCommentsQuery(userProfileId, pageNumber, pageSize)));
    }
}
