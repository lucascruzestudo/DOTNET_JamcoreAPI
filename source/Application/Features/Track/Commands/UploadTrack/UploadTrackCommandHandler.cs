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
using TagLib;
using Tag = Project.Domain.Entities.Tag;

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
        private readonly IUnitOfWork _unitOfWork;

        public UploadTrackCommandHandler(
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
                Title = command.Request.Title!.Trim(),
                Description = command.Request.Description ?? string.Empty,
                IsPublic = command.Request.IsPublic,
                UserId = user.Id
            };

            var createdTrack = _trackRepository.Add(track);
            _unitOfWork.Commit();

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
                            _unitOfWork.Commit();
                        }
                        else
                        {
                            var newTag = new Tag
                            {
                                Name = normalizedTag
                            };
                            var addedTag = _tagRepository.Add(newTag);

                            _unitOfWork.Commit();

                            trackTag = new TrackTag
                            {
                                TrackId = trackId,
                                TagId = addedTag.Id
                            };
                            _trackTagRepository.Add(trackTag);

                            _unitOfWork.Commit();
                        }
                    }
                }

                if (command.Image != null)
                {
                    string fileName = $"track_{trackId}_image.jpg";
                    uploadedImageUrl = await _supabaseService.UploadFileAsync(command.Image, fileName, "jamcore-tracks");

                    if (string.IsNullOrEmpty(uploadedImageUrl))
                    {
                        throw new Exception("Upload failed for image.");
                    }

                    uploadedImageFileName = fileName;
                    track.ImageUrl = uploadedImageUrl;
                    track.ImageFileName = uploadedImageFileName;
                    _trackRepository.Update(track);
                    _unitOfWork.Commit();
                }

                if (command.Track != null)
                {
                    string audioFileName = $"track_{trackId}_audio.mp3";
                    uploadedAudioFileUrl = await _supabaseService.UploadFileAsync(command.Track, audioFileName, "jamcore-tracks");

                    if (string.IsNullOrEmpty(uploadedAudioFileUrl))
                    {
                        throw new Exception("Upload failed for audio.");
                    }

                    uploadedAudioFileName = audioFileName;
                    track.AudioFileUrl = uploadedAudioFileUrl;
                    track.AudioFileName = uploadedAudioFileName;
                    
                    using (var stream = new MemoryStream(command.Track))
                    {
                        var audioFile = TagLib.File.Create(new StreamFileAbstraction(audioFileName, stream, stream));
                        var duration = audioFile.Properties.Duration;
                        var durationText = duration.TotalSeconds >= 3600
                            ? $"{duration.Hours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}"
                            : $"{duration.Minutes:D2}:{duration.Seconds:D2}";
                        track.Duration = durationText;
                    }

                    _trackRepository.Update(track);
                    _unitOfWork.Commit();
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
                        _unitOfWork.Commit();
                    }
                }

                var trackTags = _trackTagRepository.GetRanged(x => x.TrackId == trackId);
                _trackTagRepository.DeleteRange([.. trackTags]);
                _unitOfWork.Commit();

                var tags = _tagRepository.GetRanged(x => x.Name == track.Title);
                _tagRepository.DeleteRange([.. tags]);
                _unitOfWork.Commit();

                if (!string.IsNullOrEmpty(uploadedImageFileName))
                {
                    await _supabaseService.DeleteFileAsync(uploadedImageFileName, "jamcore-tracks");
                }

                if (!string.IsNullOrEmpty(uploadedAudioFileName))
                {
                    await _supabaseService.DeleteFileAsync(uploadedAudioFileName, "jamcore-tracks");
                }

                await _mediator.Publish(new DomainNotification("UploadTrack", _localizer.Text("UploadFailed")), cancellationToken);
                return default;
            }
        }
    }
}
