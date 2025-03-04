using Microsoft.AspNetCore.Mvc;
using Project.Domain.Notifications;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using Project.Application.Features.Commands.UploadTrack;
using Project.Domain.Common;
using Project.Application.Common.Localizers;
using Project.Application.Features.Commands.DeleteTrack;

namespace Project.WebApi.Controllers
{
    public class TrackController(
        INotificationHandler<DomainNotification> notifications,
        INotificationHandler<DomainSuccessNotification> successNotifications,
        IHttpContextAccessor httpContextAccessor,
        IMediator mediatorHandler,
        CultureLocalizer localizer) : BaseController(notifications, successNotifications, mediatorHandler, httpContextAccessor)
    {
        private readonly IMediator _mediatorHandler = mediatorHandler;
        private readonly CultureLocalizer _localizer = localizer;

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        [SwaggerOperation(Summary = "Upload a track with optional image.")]
        [ProducesResponseType(typeof(UploadTrackCommandResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadTrack([FromForm] UploadTrackCommandRequest request, IFormFile? trackFile = null, IFormFile? imageFile = null)
        {
            byte[] trackBytes;
            byte[] imageBytes = [];

            if (trackFile == null)
            {
                return BadRequest(ResponseBase<object>.Failure([_localizer.Text("TrackFileRequired").ToString()]));
            }

            using var trackStream = new MemoryStream();
            await trackFile.CopyToAsync(trackStream);
            trackBytes = trackStream.ToArray();

            if (!trackFile.ContentType.StartsWith("audio/"))
            {
                return BadRequest(ResponseBase<object>.Failure([_localizer.Text("TrackFileAudioRequired").ToString()]));
            }
            if (imageFile != null)
            {
                using var imageStream = new MemoryStream();
                await imageFile.CopyToAsync(imageStream);
                imageBytes = imageStream.ToArray();

                if (!imageFile.ContentType.StartsWith("image/"))
                {
                    return BadRequest(ResponseBase<object>.Failure([_localizer.Text("TrackFileImageRequired").ToString()]));
                }
            }

            var command = new UploadTrackCommand(request, trackBytes, imageBytes);

            var response = await _mediatorHandler.Send(command);

            return Ok(ResponseBase<UploadTrackCommandResponse>.Success(response));
        }

        [Authorize(Roles = "Admin, User")]
        [HttpDelete]
        [SwaggerOperation(Summary = "Delete a track by trackId.")]
        [ProducesResponseType(typeof(DeleteTrackCommandResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteTrack([FromBody] DeleteTrackCommandRequest request)
        {
            return Response(await _mediatorHandler.Send(new DeleteTrackCommand(request)));
        }
    }
}