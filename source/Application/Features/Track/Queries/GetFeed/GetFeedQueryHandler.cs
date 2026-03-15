using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Application.Common.Models;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Queries.GetFeed;

public class GetFeedQueryHandler : IRequestHandler<GetFeedQuery, GetFeedQueryResponse?>
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

    public GetFeedQueryHandler(
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
        IUser user)
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
    }

    public async Task<GetFeedQueryResponse?> Handle(GetFeedQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _user.Id;

        // ── Materialized aggregates ───────────────────────────────────────────
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

        // ── 1. Paginated feed tracks ──────────────────────────────────────────
        var tracksQuery = _trackRepository
            .GetRanged(x => x.IsPublic)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        var feedTracks = ProjectTracks(tracksQuery, likeAggs, playCounts, commentCounts);

        var totalCount = _trackRepository.GetRanged(x => x.IsPublic).Count();
        var paginatedTracks = new PaginatedList<TrackViewModel>(feedTracks, totalCount, request.PageNumber, request.PageSize);

        // ── 2. Recent plays for the authenticated user (top 3 distinct tracks) ─
        var recentPlayTrackIds = _trackPlayRepository
            .GetRanged(tp => tp.UserId == currentUserId)
            .GroupBy(tp => tp.TrackId)
            .Select(g => new { TrackId = g.Key, LastPlayedAt = g.Max(tp => tp.CreatedAt) })
            .OrderByDescending(g => g.LastPlayedAt)
            .Take(3)
            .Select(g => g.TrackId)
            .ToList();

        var recentPlays = recentPlayTrackIds.Count > 0
            ? ProjectSidebarTracks(recentPlayTrackIds, likeAggs, playCounts)
            : new List<TrackViewModel>();

        // ── 3. Recent likes for the authenticated user (top 3 distinct tracks) ─
        var recentLikeTrackIds = _trackLikeRepository
            .GetRanged(tl => tl.UserId == currentUserId)
            .GroupBy(tl => tl.TrackId)
            .Select(g => new { TrackId = g.Key, LastLikedAt = g.Max(tl => tl.CreatedAt) })
            .OrderByDescending(g => g.LastLikedAt)
            .Take(3)
            .Select(g => g.TrackId)
            .ToList();

        var recentLikes = recentLikeTrackIds.Count > 0
            ? ProjectSidebarTracks(recentLikeTrackIds, likeAggs, playCounts)
            : new List<TrackViewModel>();

        await _mediator.Publish(
            new DomainSuccessNotification("GetFeed", _localizer.Text("Success")),
            cancellationToken
        );

        return new GetFeedQueryResponse
        {
            Tracks = paginatedTracks,
            RecentPlays = recentPlays,
            RecentLikes = recentLikes,
        };
    }

    private List<TrackViewModel> ProjectTracks(
        IEnumerable<Track> tracksQuery,
        Dictionary<Guid, (int LikeCount, bool UserLiked)> likeAggs,
        Dictionary<Guid, int> playCounts,
        Dictionary<Guid, int> commentCounts)
    {
        var raw = JoinTrackData(tracksQuery);

        return raw.Select(r => new TrackViewModel
        {
            Id = r.Id,
            Title = r.Title,
            Description = r.Description,
            CreatedAt = r.CreatedAt,
            ImageUrl = r.ImageUrl,
            AudioFileUrl = r.AudioFileUrl,
            AudioFileName = r.AudioFileName,
            Tags = r.Tags,
            UserId = r.UserId,
            Username = r.Username,
            Duration = r.Duration,
            UpdatedAt = r.UpdatedAt,
            LikeCount = likeAggs.TryGetValue(r.Id, out var a) ? a.LikeCount : 0,
            PlayCount = playCounts.TryGetValue(r.Id, out var p) ? p : 0,
            CommentCount = commentCounts.TryGetValue(r.Id, out var c) ? c : 0,
            UserLikedTrack = likeAggs.TryGetValue(r.Id, out var ua) && ua.UserLiked,
        }).ToList();
    }

    private List<TrackViewModel> ProjectSidebarTracks(
        List<Guid> trackIds,
        Dictionary<Guid, (int LikeCount, bool UserLiked)> likeAggs,
        Dictionary<Guid, int> playCounts)
    {
        var tracksForIds = _trackRepository.GetRanged(t => t.IsPublic && trackIds.Contains(t.Id));
        var raw = JoinTrackData(tracksForIds);

        var result = raw.Select(r => new TrackViewModel
        {
            Id = r.Id,
            Title = r.Title,
            Description = r.Description,
            CreatedAt = r.CreatedAt,
            ImageUrl = r.ImageUrl,
            AudioFileUrl = r.AudioFileUrl,
            AudioFileName = r.AudioFileName,
            Tags = r.Tags,
            UserId = r.UserId,
            Username = r.Username,
            Duration = r.Duration,
            UpdatedAt = r.UpdatedAt,
            LikeCount = likeAggs.TryGetValue(r.Id, out var a) ? a.LikeCount : 0,
            PlayCount = playCounts.TryGetValue(r.Id, out var p) ? p : 0,
            UserLikedTrack = likeAggs.TryGetValue(r.Id, out var ua) && ua.UserLiked,
        }).ToList();

        // Preserve insertion order from trackIds
        return trackIds
            .Select(id => result.FirstOrDefault(t => t.Id == id))
            .Where(t => t is not null)
            .ToList()!;
    }

    private List<TrackRaw> JoinTrackData(IEnumerable<Track> tracksQuery)
    {
        return (from track in tracksQuery
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
                    x.track.AudioFileName,
                    x.track.UserId,
                    Username = x.userProfile != null && !string.IsNullOrEmpty(x.userProfile.DisplayName)
                        ? x.userProfile.DisplayName!
                        : x.user.Username ?? "unknown",
                    x.track.Duration,
                    UpdatedAt = x.track.UpdatedAt ?? x.track.CreatedAt
                } into g
                select new TrackRaw(
                    g.Key.Id,
                    g.Key.Title,
                    g.Key.Description,
                    g.Key.CreatedAt,
                    g.Key.ImageUrl,
                    g.Key.AudioFileUrl,
                    g.Key.AudioFileName,
                    g.Key.UserId,
                    g.Key.Username,
                    g.Key.Duration,
                    g.Key.UpdatedAt,
                    g.Where(x => x.tag != null && !string.IsNullOrEmpty(x.tag.Name))
                     .Select(x => x.tag!.Name).Distinct().ToArray()
                )).ToList();
    }

    private record TrackRaw(
        Guid Id, string Title, string Description,
        DateTime CreatedAt, string ImageUrl,
        string AudioFileUrl, string AudioFileName,
        Guid UserId, string Username, string Duration,
        DateTime UpdatedAt, string[] Tags);
}
