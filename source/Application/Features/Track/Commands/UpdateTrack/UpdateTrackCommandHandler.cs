using System.Threading;
using System.Threading.Tasks;
using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Application.Features.Commands.UpdateTrack;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Interfaces.Services;
using Project.Domain.Notifications;
using MediatR;

namespace Project.Application.Features.Commands.UpdateTrack
{
    public class UpdateTrackCommandHandler : IRequestHandler<UpdateTrackCommand, UpdateTrackCommandResponse?>
    {
        private readonly IMediator _mediator;
        private readonly ISupabaseService _supabaseService;
        private readonly IRepositoryBase<Track> _trackRepository;
        private readonly IRepositoryBase<Tag> _tagRepository;
        private readonly IRepositoryBase<TrackTag> _trackTagRepository;
        private readonly CultureLocalizer _localizer;
        private readonly IUser _user;
        private readonly IRepositoryBase<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTrackCommandHandler(
            IMediator mediator,
            ISupabaseService supabaseService,
            IRepositoryBase<Track> trackRepository,
            IRepositoryBase<TrackTag> trackTagRepository,
            IRepositoryBase<Tag> tagRepository,
            CultureLocalizer localizer,
            IUser user,
            IRepositoryBase<User> userRepository,
            IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _supabaseService = supabaseService;
            _trackRepository = trackRepository;
            _localizer = localizer;
            _trackTagRepository = trackTagRepository;
            _tagRepository = tagRepository;
            _user = user;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateTrackCommandResponse?> Handle(UpdateTrackCommand command, CancellationToken cancellationToken)
        {
            var user = _userRepository.Get(x => x.Id == _user.Id);

            if (user == null)
            {
                await _mediator.Publish(new DomainNotification("UpdateTrack", _localizer.Text("InvalidUser")), cancellationToken);
                return default;
            }

            var track = _trackRepository.Get(x => x.Id == command.Request.TrackId);

            if (track == null)
            {
                await _mediator.Publish(new DomainNotification("UpdateTrack", _localizer.Text("NotFound")), cancellationToken);
                return default;
            }

            if (track.UserId != user.Id)
            {
                await _mediator.Publish(new DomainNotification("UpdateTrack", _localizer.Text("InvalidUser")), cancellationToken);
                return default;
            }

            track.Title = command.Request.Title?.Trim() ?? track.Title;
            track.Description = command.Request.Description ?? track.Description;
            track.IsPublic = command.Request.IsPublic ?? track.IsPublic;
            _trackRepository.Update(track);
            _unitOfWork.Commit();

            string? uploadedImageUrl = null;
            string? uploadedImageFileName = null;
            string? uploadedAudioFileUrl = null;
            string? uploadedAudioFileName = null;

            if (command.Request.Tags != null)
            {
                var existingTags = _trackTagRepository.GetRanged(x => x.TrackId == track.Id);
                _trackTagRepository.DeleteRange([.. existingTags]);
                _unitOfWork.Commit();

                foreach (var tag in command.Request.Tags)
                {
                    var normalizedTag = tag.Trim().ToLowerInvariant();
                    var existingTag = _tagRepository.Get(x => x.Name == normalizedTag);

                    if (existingTag == null)
                    {
                        var newTag = new Tag { Name = normalizedTag };
                        existingTag = _tagRepository.Add(newTag);
                        _unitOfWork.Commit();
                    }

                    var trackTag = new TrackTag { TrackId = track.Id, TagId = existingTag.Id };
                    _trackTagRepository.Add(trackTag);
                    _unitOfWork.Commit();
                }
            }

            if (command.Image != null && command.Image.Length > 0)
            {
                if (track.ImageFileName != null)
                {
                    await _supabaseService.DeleteFileAsync(track.ImageFileName, "jamcore-tracks");
                }
                string fileName = $"track_{track.Id}_image.jpg";
                uploadedImageUrl = await _supabaseService.UploadFileAsync(command.Image, fileName, "jamcore-tracks");
                uploadedImageFileName = fileName;
                track.ImageUrl = uploadedImageUrl;
                track.ImageFileName = uploadedImageFileName;
                _trackRepository.Update(track);
                _unitOfWork.Commit();
            }

            if (command.Track != null && command.Track.Length > 0)
            {
                if (track.AudioFileName != null)
                {
                    await _supabaseService.DeleteFileAsync(track.AudioFileName, "jamcore-tracks");
                }
                string audioFileName = $"track_{track.Id}_audio.mp3";
                uploadedAudioFileUrl = await _supabaseService.UploadFileAsync(command.Track, audioFileName, "jamcore-tracks");
                uploadedAudioFileName = audioFileName;
                track.AudioFileUrl = uploadedAudioFileUrl;
                track.AudioFileName = uploadedAudioFileName;
                _trackRepository.Update(track);
                _unitOfWork.Commit();
            }

            await _mediator.Publish(new DomainSuccessNotification("UpdateTrack", _localizer.Text("Success")), cancellationToken);

            return new UpdateTrackCommandResponse
            {
                Id = track.Id,
                Title = track.Title,
                Description = track.Description,
                IsPublic = track.IsPublic,
                ImageUrl = track.ImageUrl,
                AudioFileUrl = track.AudioFileUrl,
            };
        }
    }
}
