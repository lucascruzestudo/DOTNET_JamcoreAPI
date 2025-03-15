using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Application.Common.Models;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.GetTracksByTag;

public class GetTracksByTagQueryHandler : IRequestHandler<GetTracksByTagQuery, GetTracksByTagQueryResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly IRepositoryBase<Track> _trackRepository;
    private readonly IRepositoryBase<TrackTag> _trackTagRepository;
    private readonly IRepositoryBase<Tag> _tagRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<UserProfile> _userProfileRepository;
    private readonly IRepositoryBase<TrackLike> _trackLikeRepository;
    private readonly IRepositoryBase<TrackPlay> _trackPlayRepository;
    private readonly CultureLocalizer _localizer;
    private readonly IUser _user;

    public GetTracksByTagQueryHandler(IUnitOfWork unitOfWork, IMediator mediator, IRepositoryBase<Track> trackRepository,
        IRepositoryBase<TrackTag> trackTagRepository, IRepositoryBase<Tag> tagRepository, CultureLocalizer localizer,
        IRepositoryBase<User> userRepository, IRepositoryBase<TrackLike> trackLikeRepository, IUser user, IRepositoryBase<TrackPlay> trackPlayRepository,
        IRepositoryBase<UserProfile> userProfileRepository)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _trackRepository = trackRepository;
        _trackTagRepository = trackTagRepository;
        _tagRepository = tagRepository;
        _localizer = localizer;
        _userRepository = userRepository;
        _trackLikeRepository = trackLikeRepository;
        _user = user;
        _trackPlayRepository = trackPlayRepository;
        _userProfileRepository = userProfileRepository;
    }

    public async Task<GetTracksByTagQueryResponse?> Handle(GetTracksByTagQuery request, CancellationToken cancellationToken)
    {
        var matchingTag = _tagRepository.Get(x => x.Name == request.Tag);
        if (matchingTag == null)
        {
            await _mediator.Publish(new DomainNotification("GetTracksByTag", _localizer.Text("NotFound")), cancellationToken);
            return default;
        }

        var trackIds = _trackTagRepository
            .GetRanged(x => x.TagId == matchingTag.Id)
            .Select(x => x.TrackId)
            .ToList();

        var tracks = _trackRepository
            .GetRanged(x => trackIds.Contains(x.Id) && x.IsPublic)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new
            {
                x.Id,
                x.Title,
                x.Description,
                x.CreatedAt,
                x.ImageUrl,
                x.ImageFileName,
                x.AudioFileUrl,
                x.AudioFileName,
                x.Duration,
                x.UserId
            })
            .ToList();

        var trackLikeCounts = _trackLikeRepository
            .GetRanged(x => trackIds.Contains(x.TrackId))
            .GroupBy(x => x.TrackId)
            .ToDictionary(g => g.Key, g => g.Count());

        var trackPlayCounts = _trackPlayRepository
            .GetRanged(x => trackIds.Contains(x.TrackId))
            .GroupBy(x => x.TrackId)
            .ToDictionary(g => g.Key, g => g.Count());

        var userIds = tracks.Select(x => x.UserId).Distinct().ToList();
        var userProfiles = _userProfileRepository
            .GetRanged(x => userIds.Contains(x.UserId))
            .ToDictionary(x => x.UserId, x => x.DisplayName);

        var users = _userRepository
            .GetRanged(x => userIds.Contains(x.Id))
            .ToDictionary(x => x.Id, x => x.Username);

        var trackViewModels = tracks.Select(x => new TrackViewModel
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            CreatedAt = x.CreatedAt,
            ImageUrl = x.ImageUrl,
            ImageFileName = x.ImageFileName,
            AudioFileUrl = x.AudioFileUrl,
            AudioFileName = x.AudioFileName,
            Tags = _trackTagRepository
                .GetRanged(tt => tt.TrackId == x.Id)
                .Join(_tagRepository.GetAll(), tt => tt.TagId, t => t.Id, (tt, t) => t.Name)
                .ToArray(),
            UserId = x.UserId,
            Username = userProfiles.TryGetValue(x.UserId, out var displayName) && !string.IsNullOrEmpty(displayName)
                ? displayName
                : users.TryGetValue(x.UserId, out var username) && !string.IsNullOrEmpty(username)
                    ? username
                    : "Unknown",
            LikeCount = trackLikeCounts.TryGetValue(x.Id, out var likeCount) ? likeCount : 0,
            PlayCount = trackPlayCounts.TryGetValue(x.Id, out var playCount) ? playCount : 0,
            UserLikedTrack = _trackLikeRepository.Get(like => like.TrackId == x.Id && like.UserId == _user.Id) != null,
            Duration = x.Duration
        }).ToList();

        var paginatedTracks = PaginatedList<TrackViewModel>.Create(trackViewModels.AsQueryable(), request.PageNumber, request.PageSize);

        _unitOfWork.Commit();
        await _mediator.Publish(new DomainSuccessNotification("GetTracksByTag", _localizer.Text("Success")), cancellationToken);

        return new GetTracksByTagQueryResponse
        {
            Tracks = paginatedTracks
        };
    }
}
