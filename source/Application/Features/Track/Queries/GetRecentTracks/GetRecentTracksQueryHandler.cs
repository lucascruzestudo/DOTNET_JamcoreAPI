using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Application.Common.Models;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.GetRecentTracks;

public class GetRecentTracksQueryHandler : IRequestHandler<GetRecentTracksQuery, GetRecentTracksQueryResponse?>
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

    public GetRecentTracksQueryHandler(IUnitOfWork unitOfWork, IMediator mediator, IRepositoryBase<Track> trackRepository, IRepositoryBase<TrackTag> trackTagRepository, IRepositoryBase<Tag> tagRepository, IRepositoryBase<User> userRepository, CultureLocalizer localizer, IRepositoryBase<TrackLike> trackLikeRepository, IUser user, IRepositoryBase<TrackPlay> trackPlayRepository, IRepositoryBase<UserProfile> userProfileRepository)
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
    }

    public async Task<GetRecentTracksQueryResponse?> Handle(GetRecentTracksQuery request, CancellationToken cancellationToken)
    {
        var trackIdsQuery = _trackRepository
            .GetRanged(x => x.IsPublic)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.Id);

        var trackTags = _trackTagRepository
            .GetRanged(x => trackIdsQuery.Contains(x.TrackId))
            .ToList();

        var tagIds = trackTags.Select(tt => tt.TagId).Distinct().ToList();
        var tags = _tagRepository
            .GetRanged(x => tagIds.Contains(x.Id))
            .ToDictionary(x => x.Id, x => x.Name);

        var trackLikeCounts = _trackLikeRepository
            .GetRanged(x => trackIdsQuery.Contains(x.TrackId))
            .GroupBy(x => x.TrackId)
            .ToDictionary(g => g.Key, g => g.Count());

        var trackPlayCounts = _trackPlayRepository
            .GetRanged(x => trackIdsQuery.Contains(x.TrackId))
            .GroupBy(x => x.TrackId)
            .ToDictionary(g => g.Key, g => g.Count());

        var userIds = _trackRepository
            .GetRanged(x => trackIdsQuery.Contains(x.Id))
            .Select(x => x.UserId)
            .Distinct()
            .ToList();

        var userProfiles = _userProfileRepository
            .GetRanged(x => userIds.Contains(x.UserId))
            .ToDictionary(x => x.UserId, x => x.DisplayName);

        var users = _userRepository
            .GetRanged(x => userIds.Contains(x.Id))
            .ToDictionary(x => x.Id, x => x.Username);

        var query = _trackRepository
            .GetRanged(x => trackIdsQuery.Contains(x.Id))
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new TrackViewModel
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                CreatedAt = x.CreatedAt,
                ImageUrl = x.ImageUrl,
                ImageFileName = x.ImageFileName,
                AudioFileUrl = x.AudioFileUrl,
                AudioFileName = x.AudioFileName,
                Tags = trackTags
                    .Where(tt => tt.TrackId == x.Id)
                    .Select(tt => tags[tt.TagId])
                    .ToArray(),
                UserId = x.UserId,
                Username = userProfiles.TryGetValue(x.UserId, out var displayName) && !string.IsNullOrEmpty(displayName)
                    ? displayName
                    : users.TryGetValue(x.UserId, out var username) && !string.IsNullOrEmpty(username)
                        ? username
                        : "Unknown",
                LikeCount = trackLikeCounts.TryGetValue(x.Id, out int value) ? value : 0,
                PlayCount = trackPlayCounts.TryGetValue(x.Id, out int value2) ? value2 : 0,
                UserLikedTrack = _trackLikeRepository.Get(like => like.TrackId == x.Id && like.UserId == _user.Id) != null,
                Duration = x.Duration
            }).AsQueryable();

        var paginatedTracks = PaginatedList<TrackViewModel>.Create(query, request.PageNumber, request.PageSize);

        _unitOfWork.Commit();
        await _mediator.Publish(new DomainSuccessNotification("GetRecentTracks", _localizer.Text("Success")), cancellationToken);

        return new GetRecentTracksQueryResponse
        {
            Tracks = paginatedTracks
        };
    }

}