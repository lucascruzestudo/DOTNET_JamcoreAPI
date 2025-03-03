using Microsoft.AspNetCore.Mvc;
using Project.Domain.Notifications;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using Project.Application.Features.Commands.UpsertProfile;

namespace Project.WebApi.Controllers;

public class ProfileController(INotificationHandler<DomainNotification> notifications,
                      INotificationHandler<DomainSuccessNotification> successNotifications,
                      IHttpContextAccessor httpContextAccessor,
                      IMediator mediatorHandler) : BaseController(notifications, successNotifications, mediatorHandler, httpContextAccessor)
{
    private readonly IMediator _mediatorHandler = mediatorHandler;

    [Authorize(Roles = "Admin, User")]
    [HttpPut]
    [SwaggerOperation(Summary = "Upsert a user profile.")]
    [ProducesResponseType(typeof(UpsertProfileCommandResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpsertProfile([FromForm] UpsertProfileCommandRequest request, IFormFile? image = null)
    {
        byte[] imageBytes = [];

        if (image != null)
        {
            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);
            imageBytes = memoryStream.ToArray();
        }

        var command = new UpsertProfileCommand(request, imageBytes);
        var response = await _mediatorHandler.Send(command);

        return Response(response);
    }

}