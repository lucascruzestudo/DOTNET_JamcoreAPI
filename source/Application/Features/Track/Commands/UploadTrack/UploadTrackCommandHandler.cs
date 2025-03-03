using System.Threading;
using System.Threading.Tasks;
using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Application.Features.Commands.UploadTrack;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Interfaces.Services;
using Project.Domain.Notifications;
using MediatR;

namespace Project.Application.Features.Commands.UploadTrack
{
    public class UploadTrackCommandHandler : IRequestHandler<UploadTrackCommand, UploadTrackCommandResponse?>
    {
        private readonly IMediator _mediator;
        private readonly ISupabaseService _supabaseService;
        private readonly IRepositoryBase<Track> _trackRepository;
        private readonly IRepositoryBase<Tag> _tagRepository;
        private readonly IRepositoryBase<TrackTag> _trackTagRepository;
        private readonly CultureLocalizer _localizer;
        private readonly IUser _user;
        private readonly IRepositoryBase<User> _userRepository;

        public UploadTrackCommandHandler(
            IMediator mediator,
            ISupabaseService supabaseService,
            IRepositoryBase<Track> trackRepository,
            IRepositoryBase<TrackTag> trackTagRepository,
            IRepositoryBase<Tag> tagRepository,
            CultureLocalizer localizer,
            IUser user,
            IRepositoryBase<User> userRepository)
        {
            _mediator = mediator;
            _supabaseService = supabaseService;
            _trackRepository = trackRepository;
            _localizer = localizer;
            _trackTagRepository = trackTagRepository;
            _tagRepository = tagRepository;
            _user = user;
            _userRepository = userRepository;
        }

        public async Task<UploadTrackCommandResponse?> Handle(UploadTrackCommand command, CancellationToken cancellationToken)
        {
            var user = _userRepository.Get(x => x.Id == _user.Id);

            if (user == null)
            {
                await _mediator.Publish(new DomainNotification("UploadTrack", _localizer.Text("InvalidUser")), cancellationToken);
                return default;
            }

            var track = new Track
            {
                Title = command.Request.Title!,
                Description = command.Request.Description ?? string.Empty,
                IsPublic = command.Request.IsPublic,
                UserId = user.Id
            };

            var createdTrack = _trackRepository.Add(track);
            var trackId = createdTrack.Id;
            string? uploadedImageUrl = null;
            string? uploadedImageFileName = null;
            string? uploadedAudioFileUrl = null;
            string? uploadedAudioFileName = null;

            try
            {
                if (command.Request.Tags != null)
                {
                    foreach (var tag in command.Request.Tags)
                    {
                        var normalizedTag = tag.Trim().ToLowerInvariant();

                        var existingTag = _tagRepository.Get(x => x.Name == normalizedTag);

                        TrackTag trackTag;
                        if (existingTag != null)
                        {
                            trackTag = new TrackTag
                            {
                                TrackId = trackId,
                                TagId = existingTag.Id
                            };
                            _trackTagRepository.Add(trackTag);
                        }
                        else
                        {
                            var newTag = new Tag
                            {
                                Name = normalizedTag
                            };
                            var addedTag = _tagRepository.Add(newTag);

                            trackTag = new TrackTag
                            {
                                TrackId = trackId,
                                TagId = addedTag.Id
                            };
                            _trackTagRepository.Add(trackTag);
                        }
                    }
                }

                if (command.Image != null)
                {
                    string fileName = $"track_{trackId}_{track.Title.ToLower().Replace(" ", "_")}_image.jpg";
                    uploadedImageUrl = await _supabaseService.UploadFileAsync(command.Image, fileName, "jamcore-tracks");

                    if (string.IsNullOrEmpty(uploadedImageUrl))
                    {
                        throw new Exception("Upload failed for image.");
                    }

                    uploadedImageFileName = fileName;
                    track.ImageUrl = uploadedImageUrl;
                    _trackRepository.Update(track);
                }

                if (command.Track != null)
                {
                    string audioFileName = $"track_{trackId}_{track.Title.ToLower().Replace(" ", "_")}_audio.mp3";
                    uploadedAudioFileUrl = await _supabaseService.UploadFileAsync(command.Track, audioFileName, "jamcore-tracks");

                    if (string.IsNullOrEmpty(uploadedAudioFileUrl))
                    {
                        throw new Exception("Upload failed for audio.");
                    }

                    uploadedAudioFileName = audioFileName;
                    track.AudioFileUrl = uploadedAudioFileUrl;
                    _trackRepository.Update(track);
                }

                await _mediator.Publish(new DomainSuccessNotification("UploadTrack", _localizer.Text("Success")), cancellationToken);

                return new UploadTrackCommandResponse
                {
                    Id = track.Id,
                    Title = track.Title,
                    Description = track.Description,
                    IsPublic = track.IsPublic,
                    ImageUrl = track.ImageUrl,
                    AudioFileUrl = track.AudioFileUrl,
                };
            }
            catch (Exception)
            {
                if (trackId != Guid.Empty)
                {
                    var trackToDelete = _trackRepository.Get(x => x.Id == trackId);
                    if (trackToDelete != null)
                    {
                        _trackRepository.Delete(trackToDelete);
                    }
                }

                var trackTags = _trackTagRepository.GetRanged(x => x.TrackId == trackId);
                foreach (var trackTag in trackTags)
                {
                    _trackTagRepository.Delete(trackTag);
                }

                var tags = _tagRepository.GetRanged(x => x.Name == track.Title);
                foreach (var tag in tags)
                {
                    _tagRepository.Delete(tag);
                }

                if (!string.IsNullOrEmpty(uploadedImageFileName))
                {
                    await _supabaseService.DeleteFileAsync(uploadedImageFileName);
                }

                if (!string.IsNullOrEmpty(uploadedAudioFileName))
                {
                    await _supabaseService.DeleteFileAsync(uploadedAudioFileName);
                }

                await _mediator.Publish(new DomainNotification("UploadTrack", _localizer.Text("UploadFailed")), cancellationToken);
                return null;
            }
        }
    }
}
