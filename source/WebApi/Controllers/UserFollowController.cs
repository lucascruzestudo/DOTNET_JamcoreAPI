using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Application.Features.Commands.FollowUser;
using Project.Application.Features.Commands.GetFollowers;
using Project.Application.Features.Commands.GetFollowing;
using Project.Application.Features.Commands.IsFollowing;
using Project.Application.Features.Commands.UnfollowUser;
using Project.Domain.Notifications;
using Swashbuckle.AspNetCore.Annotations;

namespace Project.WebApi.Controllers;

public class UserFollowController(
    INotificationHandler<DomainNotification> notifications,
    INotificationHandler<DomainSuccessNotification> successNotifications,
    IHttpContextAccessor httpContextAccessor,
    IMediator mediatorHandler) : BaseController(notifications, successNotifications, mediatorHandler, httpContextAccessor)
{
    private readonly IMediator _mediatorHandler = mediatorHandler;

    [Authorize(Roles = "Admin, User")]
    [HttpPost]
    [SwaggerOperation(Summary = "Follow a user.")]
    [ProducesResponseType(typeof(FollowUserCommandResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> FollowUser([FromBody] FollowUserCommandRequest request)
    {
        return Response(await _mediatorHandler.Send(new FollowUserCommand(request)));
    }

    [Authorize(Roles = "Admin, User")]
    [HttpDelete]
    [SwaggerOperation(Summary = "Unfollow a user.")]
    [ProducesResponseType(typeof(UnfollowUserCommandResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UnfollowUser([FromBody] UnfollowUserCommandRequest request)
    {
        return Response(await _mediatorHandler.Send(new UnfollowUserCommand(request)));
    }

    [Authorize(Roles = "Admin, User")]
    [HttpGet("followers")]
    [SwaggerOperation(Summary = "Get followers of a user.")]
    [ProducesResponseType(typeof(GetFollowersQueryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFollowers(
        [FromQuery] Guid userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        return Response(await _mediatorHandler.Send(new GetFollowersQuery(userId, pageNumber, pageSize)));
    }

    [Authorize(Roles = "Admin, User")]
    [HttpGet("following")]
    [SwaggerOperation(Summary = "Get users followed by a user.")]
    [ProducesResponseType(typeof(GetFollowingQueryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFollowing(
        [FromQuery] Guid userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        return Response(await _mediatorHandler.Send(new GetFollowingQuery(userId, pageNumber, pageSize)));
    }

    [Authorize(Roles = "Admin, User")]
    [HttpGet("isFollowing")]
    [SwaggerOperation(Summary = "Check if the current user follows a given user.")]
    [ProducesResponseType(typeof(IsFollowingQueryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> IsFollowing([FromQuery] Guid followedUserId)
    {
        return Response(await _mediatorHandler.Send(new IsFollowingQuery(followedUserId)));
    }
}
