using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Application.Common.Models;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.GetTrack;

public class GetTrackQueryHandler : IRequestHandler<GetTrackQuery, GetTrackQueryResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly IRepositoryBase<Track> _trackRepository;
    private readonly IRepositoryBase<TrackTag> _trackTagRepository;
    private readonly IRepositoryBase<Tag> _tagRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<UserProfile> _userProfileRepository;
    private readonly IRepositoryBase<TrackLike> _trackLikeRepository;
    private readonly CultureLocalizer _localizer;
    private readonly IUser _user;
    private readonly IRepositoryBase<TrackPlay> _trackPlayRepository;
    private readonly IRepositoryBase<TrackComment> _trackCommentRepository;

    public GetTrackQueryHandler(IUnitOfWork unitOfWork, IRepositoryBase<TrackComment> trackCommentRepository, IMediator mediator, IRepositoryBase<Track> trackRepository, IRepositoryBase<TrackTag> trackTagRepository, IRepositoryBase<Tag> tagRepository, IRepositoryBase<User> userRepository, CultureLocalizer localizer, IRepositoryBase<TrackLike> trackLikeRepository, IUser user, IRepositoryBase<TrackPlay> trackPlayRepository, IRepositoryBase<UserProfile> userProfileRepository)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _trackRepository = trackRepository;
        _trackTagRepository = trackTagRepository;
        _tagRepository = tagRepository;
        _userRepository = userRepository;
        _localizer = localizer;
        _trackLikeRepository = trackLikeRepository;
        _user = user;
        _trackPlayRepository = trackPlayRepository;
        _userProfileRepository = userProfileRepository;
        _trackCommentRepository = trackCommentRepository;
    }

    public async Task<GetTrackQueryResponse?> Handle(GetTrackQuery request, CancellationToken cancellationToken)
    {
        var track = _trackRepository.Get(x => x.Id == request.TrackId);
        if (track == null)
        {
            await _mediator.Publish(new DomainNotification("GetTrack", _localizer.Text("NotFound")), cancellationToken);
            return default;
        }

        var trackTags = _trackTagRepository.GetRanged(x => x.TrackId == track.Id).ToList();
        var tagIds = trackTags.Select(tt => tt.TagId).Distinct().ToList();
        var tags = _tagRepository.GetRanged(x => tagIds.Contains(x.Id)).ToDictionary(x => x.Id, x => x.Name);

        var trackLikeCount = _trackLikeRepository.GetRanged(x => x.TrackId == track.Id).Count();
        var trackPlayCount = _trackPlayRepository.GetRanged(x => x.TrackId == track.Id).Count();

        var userProfile = _userProfileRepository.Get(u => u.UserId == track.UserId);
        var user = _userRepository.Get(u => u.Id == track.UserId);
        var username = userProfile?.DisplayName ?? user?.Username ?? "Unknown";

        var trackComments = _trackCommentRepository.GetRanged(x => x.TrackId == track.Id).ToList();

        var commentUserIds = trackComments.Select(c => c.UserId).Distinct().ToList();
        var commentUsers = _userRepository
            .GetRanged(u => commentUserIds.Contains(u.Id))
            .ToDictionary(u => u.Id, u => u);

        var commentUserProfiles = _userProfileRepository
            .GetRanged(u => commentUserIds.Contains(u.UserId))
            .ToDictionary(u => u.UserId, u => u);

        var comments = trackComments.Select(comment =>
        {
            var user = commentUsers.GetValueOrDefault(comment.UserId);
            var userProfile = commentUserProfiles.GetValueOrDefault(comment.UserId);
            return new TrackViewModel.Comment
            {
                Id = comment.Id,
                Text = comment.Comment,
                UserId = comment.UserId,
                Username = user?.Username ?? "unknown",
                DisplayName = userProfile?.DisplayName ?? user?.Username ?? "unknown",
                UserProfilePictureUrl = userProfile?.ProfilePictureUrl ?? string.Empty,
                UserProfileUpdatedAt = userProfile?.UpdatedAt ?? userProfile?.CreatedAt ?? null,
                CreatedAt = comment.CreatedAt
            };
        }).ToList();

        var trackResponse = new TrackViewModel
        {
            Id = track.Id,
            Title = track.Title,
            Description = track.Description,
            CreatedAt = track.CreatedAt,
            ImageUrl = track.ImageUrl,
            ImageFileName = track.ImageFileName,
            AudioFileUrl = track.AudioFileUrl,
            AudioFileName = track.AudioFileName,
            Tags = trackTags.Select(tt => tags[tt.TagId]).ToArray(),
            UserId = track.UserId,
            Username = username,
            LikeCount = trackLikeCount,
            PlayCount = trackPlayCount,
            UserLikedTrack = _trackLikeRepository.Get(x => x.TrackId == track.Id && x.UserId == _user.Id) != null,
            Duration = track.Duration,
            Comments = comments,
            UpdatedAt = track.UpdatedAt ?? track.CreatedAt
        };

        _unitOfWork.Commit();
        await _mediator.Publish(new DomainSuccessNotification("GetTrack", _localizer.Text("Success")), cancellationToken);

        return new GetTrackQueryResponse
        {
            Track = trackResponse
        };
    }
}
