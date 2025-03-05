using Microsoft.AspNetCore.Mvc;
using Project.Domain.Notifications;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using Project.Application.Features.Commands.UploadTrack;
using Project.Domain.Common;
using Project.Application.Common.Localizers;
using Project.Application.Features.Commands.DeleteTrack;
using Project.Application.Features.Commands.UpdateTrack;
using Project.Application.Features.Commands.GetTracksByTag;
using Project.Application.Features.Commands.GetTracksByUser;
using Project.Application.Features.Commands.GetRecentTracks;

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

        [Authorize(Roles = "Admin, User")]
        [HttpPut]
        [SwaggerOperation(Summary = "Update a track with optional image and audio changes.")]
        [ProducesResponseType(typeof(UpdateTrackCommandResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateTrack([FromForm] UpdateTrackCommandRequest request, IFormFile? trackFile = null, IFormFile? imageFile = null)
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
                    return BadRequest(ResponseBase<object>.Failure([_localizer.Text("TrackFileAudioRequired").ToString()]));
                }
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

            var command = new UpdateTrackCommand(request, trackBytes, imageBytes);

            var response = await _mediatorHandler.Send(command);

            return Ok(ResponseBase<UpdateTrackCommandResponse>.Success(response));
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet()]
        [SwaggerOperation(Summary = "Get recent tracks.")]
        [ProducesResponseType(typeof(GetRecentTracksQueryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTracks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Response(await _mediatorHandler.Send(new GetRecentTracksQuery(pageNumber, pageSize)));
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("byTag")]
        [SwaggerOperation(Summary = "Get public tracks for a tag.")]
        [ProducesResponseType(typeof(GetTracksByTagQueryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTracksByTag([FromQuery] string tag, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Response(await _mediatorHandler.Send(new GetTracksByTagQuery(tag, pageNumber, pageSize)));
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("byUser")]
        [SwaggerOperation(Summary = "Get public tracks of an user.")]
        [ProducesResponseType(typeof(GetTracksByUserQueryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTracksByUser([FromQuery] string username, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Response(await _mediatorHandler.Send(new GetTracksByUserQuery(username, pageNumber, pageSize)));
        }
    }
}