using Microsoft.AspNetCore.Mvc;
using Project.Domain.Notifications;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using Project.Application.Features.Commands.UploadTrack;

namespace Project.WebApi.Controllers
{
    public class TrackController(
        INotificationHandler<DomainNotification> notifications,
        INotificationHandler<DomainSuccessNotification> successNotifications,
        IHttpContextAccessor httpContextAccessor,
        IMediator mediatorHandler) 
        : BaseController(notifications, successNotifications, mediatorHandler, httpContextAccessor)
    {
        private readonly IMediator _mediatorHandler = mediatorHandler;

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        [SwaggerOperation(Summary = "Upload a track with optional image.")]
        [ProducesResponseType(typeof(UploadTrackCommandResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadTrack([FromForm] UploadTrackCommandRequest request, IFormFile? trackFile = null, IFormFile? imageFile = null)
        {
            byte[] trackBytes = [];
            byte[] imageBytes = [];

            if (trackFile != null)
            {
                using var trackStream = new MemoryStream();
                await trackFile.CopyToAsync(trackStream);
                trackBytes = trackStream.ToArray();

                if (!trackFile.ContentType.StartsWith("audio/"))
                {
                    return BadRequest("Track file must be an audio file.");
                }
            } else {
                return BadRequest("Track file is required.");
            }

            if (imageFile != null)
            {
                using var imageStream = new MemoryStream();
                await imageFile.CopyToAsync(imageStream);
                imageBytes = imageStream.ToArray();

                if (!imageFile.ContentType.StartsWith("image/"))
                {
                    return BadRequest("Image file must be an image.");
                }
            }

            var command = new UploadTrackCommand(request, trackBytes, imageBytes);

            var response = await _mediatorHandler.Send(command);

            return Response(response);
        }
    }
}
