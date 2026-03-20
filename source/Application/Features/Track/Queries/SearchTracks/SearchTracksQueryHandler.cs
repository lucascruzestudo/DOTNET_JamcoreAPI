using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Application.Common.Models;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Interfaces.Services;
using Project.Domain.Notifications;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.SearchTracks;

public class SearchTracksQueryHandler : IRequestHandler<SearchTracksQuery, SearchTracksQueryResponse?>
{
    private readonly IMediator _mediator;
    private readonly IRepositoryBase<Track> _trackRepository;
    private readonly IRepositoryBase<TrackTag> _trackTagRepository;
    private readonly IRepositoryBase<Tag> _tagRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<UserProfile> _userProfileRepository;
    private readonly IRepositoryBase<TrackLike> _trackLikeRepository;
    private readonly IRepositoryBase<TrackPlay> _trackPlayRepository;
    private readonly IRepositoryBase<TrackComment> _trackCommentRepository;
    private readonly CultureLocalizer _localizer;
    private readonly IUser _user;
    private readonly IRedisService _redis;

    public SearchTracksQueryHandler(
        IMediator mediator,
        IRepositoryBase<Track> trackRepository,
        IRepositoryBase<TrackTag> trackTagRepository,
        IRepositoryBase<Tag> tagRepository,
        IRepositoryBase<User> userRepository,
        IRepositoryBase<UserProfile> userProfileRepository,
        IRepositoryBase<TrackLike> trackLikeRepository,
        IRepositoryBase<TrackPlay> trackPlayRepository,
        IRepositoryBase<TrackComment> trackCommentRepository,
        CultureLocalizer localizer,
        IUser user,
        IRedisService redis)
    {
        _mediator = mediator;
        _trackRepository = trackRepository;
        _trackTagRepository = trackTagRepository;
        _tagRepository = tagRepository;
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _trackLikeRepository = trackLikeRepository;
        _trackPlayRepository = trackPlayRepository;
        _trackCommentRepository = trackCommentRepository;
        _localizer = localizer;
        _user = user;
        _redis = redis;
    }

    public async Task<SearchTracksQueryResponse?> Handle(SearchTracksQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _user.Id;
        var term = request.Search.Trim().ToLower();
        var cacheKey = $"tracks:search:{term}:{request.PageNumber}:{request.PageSize}";

        var cached = await _redis.GetAsync<SearchTracksQueryResponse>(cacheKey);
        if (cached != null)
        {
            var cachedResult = JsonSerializer.Deserialize<SearchTracksQueryResponse>(cached);
            if (cachedResult != null)
            {
                await _mediator.Publish(new DomainSuccessNotification("SearchTracks", _localizer.Text("Success")), cancellationToken);
                return cachedResult;
            }
        }

        // ── Tags that match the search term ─────────────────────────────────
        var matchingTagIds = _tagRepository
            .GetRanged(t => t.Name.ToLower().Contains(term))
            .Select(t => t.Id)
            .ToList();

        var trackIdsFromTags = matchingTagIds.Count > 0
            ? _trackTagRepository
                .GetRanged(tt => matchingTagIds.Contains(tt.TagId))
                .Select(tt => tt.TrackId)
                .Distinct()
                .ToList()
            : new List<Guid>();

        // ── User display names that match ─────────────────────────────────
        var matchingUserIds = _userRepository
            .GetRanged(u => u.Username != null && u.Username.ToLower().Contains(term))
            .Select(u => u.Id)
            .ToList();

        var matchingProfileUserIds = _userProfileRepository
            .GetRanged(p => p.DisplayName != null && p.DisplayName.ToLower().Contains(term))
            .Select(p => p.UserId)
            .ToList();

        var allMatchingUserIds = matchingUserIds.Union(matchingProfileUserIds).Distinct().ToList();

        // ── Total matching tracks ─────────────────────────────────────────
        var totalCount = _trackRepository
            .GetRanged(t => t.IsPublic && (
                t.Title.ToLower().Contains(term) ||
                t.Description.ToLower().Contains(term) ||
                allMatchingUserIds.Contains(t.UserId) ||
                trackIdsFromTags.Contains(t.Id)
            ))
            .Count();

        // ── Paginated tracks ──────────────────────────────────────────────
        var tracksQuery = _trackRepository
            .GetRanged(t => t.IsPublic && (
                t.Title.ToLower().Contains(term) ||
                t.Description.ToLower().Contains(term) ||
                allMatchingUserIds.Contains(t.UserId) ||
                trackIdsFromTags.Contains(t.Id)
            ))
            .OrderByDescending(t => t.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        // ── Aggregates ────────────────────────────────────────────────────
        var likeAggs = _trackLikeRepository
            .GetAll()
            .GroupBy(x => x.TrackId)
            .Select(g => new { TrackId = g.Key, Count = g.Count(), UserLiked = g.Any(l => l.UserId == currentUserId) })
            .ToList()
            .ToDictionary(g => g.TrackId, g => (LikeCount: g.Count, UserLiked: g.UserLiked));

        var playCounts = _trackPlayRepository
            .GetAll()
            .GroupBy(x => x.TrackId)
            .Select(g => new { TrackId = g.Key, Count = g.Count() })
            .ToDictionary(g => g.TrackId, g => g.Count);

        var commentCounts = _trackCommentRepository
            .GetAll()
            .GroupBy(x => x.TrackId)
            .Select(g => new { TrackId = g.Key, Count = g.Count() })
            .ToDictionary(g => g.TrackId, g => g.Count);

        // ── Join & project ────────────────────────────────────────────────
        var raw = (from track in tracksQuery
                   join user in _userRepository.GetAll() on track.UserId equals user.Id
                   join userProfile in _userProfileRepository.GetAll() on track.UserId equals userProfile.UserId into userProfiles
                   from userProfile in userProfiles.DefaultIfEmpty()
                   join trackTag in _trackTagRepository.GetAll() on track.Id equals trackTag.TrackId into trackTags
                   from trackTag in trackTags.DefaultIfEmpty()
                   join tag in _tagRepository.GetAll() on trackTag?.TagId equals tag.Id into tags
                   from tag in tags.DefaultIfEmpty()
                   select new { track, user, userProfile, tag } into x
                   group x by new
                   {
                       x.track.Id,
                       x.track.Title,
                       x.track.Description,
                       x.track.CreatedAt,
                       x.track.ImageUrl,
                       x.track.AudioFileUrl,
                       x.track.UserId,
                       Username = x.userProfile != null && !string.IsNullOrEmpty(x.userProfile.DisplayName)
                           ? x.userProfile.DisplayName!
                           : x.user.Username ?? "unknown",
                       x.track.Duration,
                       UpdatedAt = x.track.UpdatedAt ?? x.track.CreatedAt
                   } into g
                   select new TrackViewModel
                   {
                       Id = g.Key.Id,
                       Title = g.Key.Title,
                       Description = g.Key.Description,
                       CreatedAt = g.Key.CreatedAt,
                       ImageUrl = g.Key.ImageUrl,
                       AudioFileUrl = g.Key.AudioFileUrl,
                       Tags = g.Where(x => x.tag != null && !string.IsNullOrEmpty(x.tag.Name))
                                .Select(x => x.tag!.Name).Distinct().ToArray(),
                       UserId = g.Key.UserId,
                       Username = g.Key.Username,
                       Duration = g.Key.Duration,
                       UpdatedAt = g.Key.UpdatedAt,
                       LikeCount = likeAggs.TryGetValue(g.Key.Id, out var a) ? a.LikeCount : 0,
                       PlayCount = playCounts.TryGetValue(g.Key.Id, out var p) ? p : 0,
                       CommentCount = commentCounts.TryGetValue(g.Key.Id, out var c) ? c : 0,
                       UserLikedTrack = likeAggs.TryGetValue(g.Key.Id, out var ua) && ua.UserLiked,
                   }).ToList();

        var paginatedTracks = new PaginatedList<TrackViewModel>(raw, totalCount, request.PageNumber, request.PageSize);

        await _mediator.Publish(
            new DomainSuccessNotification("SearchTracks", _localizer.Text("Success")),
            cancellationToken
        );

        var response = new SearchTracksQueryResponse { Tracks = paginatedTracks };
        await _redis.SetAsync(cacheKey, response, TimeSpan.FromMinutes(2));
        return response;
    }
}
