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

namespace Project.Application.Features.Commands.GetTracksByUser;

public class GetTracksByUserQueryHandler : IRequestHandler<GetTracksByUserQuery, GetTracksByUserQueryResponse?>
{
    private readonly IMediator _mediator;
    private readonly IRepositoryBase<Track> _trackRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<UserProfile> _userProfileRepository;
    private readonly IRepositoryBase<TrackTag> _trackTagRepository;
    private readonly IRepositoryBase<Tag> _tagRepository;
    private readonly IRepositoryBase<TrackLike> _trackLikeRepository;
    private readonly IRepositoryBase<TrackPlay> _trackPlayRepository;
    private readonly IRepositoryBase<TrackComment> _trackCommentRepository;
    private readonly CultureLocalizer _localizer;
    private readonly IUser _user;
    private readonly IRedisService _redis;

    public GetTracksByUserQueryHandler(
        IMediator mediator,
        IRepositoryBase<Track> trackRepository,
        IRepositoryBase<User> userRepository,
        IRepositoryBase<UserProfile> userProfileRepository,
        IRepositoryBase<TrackTag> trackTagRepository,
        IRepositoryBase<Tag> tagRepository,
        IRepositoryBase<TrackLike> trackLikeRepository,
        IRepositoryBase<TrackPlay> trackPlayRepository,
        IRepositoryBase<TrackComment> trackCommentRepository,
        CultureLocalizer localizer,
        IUser user,
        IRedisService redis)
    {
        _mediator = mediator;
        _trackRepository = trackRepository;
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _trackTagRepository = trackTagRepository;
        _tagRepository = tagRepository;
        _trackLikeRepository = trackLikeRepository;
        _trackPlayRepository = trackPlayRepository;
        _trackCommentRepository = trackCommentRepository;
        _localizer = localizer;
        _user = user;
        _redis = redis;
    }

    public async Task<GetTracksByUserQueryResponse?> Handle(GetTracksByUserQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"tracks:user:{request.Id}:{request.PageNumber}:{request.PageSize}";
        var cached = await _redis.GetAsync<GetTracksByUserQueryResponse>(cacheKey);
        if (cached != null)
        {
            var cachedResult = JsonSerializer.Deserialize<GetTracksByUserQueryResponse>(cached);
            if (cachedResult != null)
            {
                await _mediator.Publish(new DomainSuccessNotification("GetTracksByUser", _localizer.Text("Success")), cancellationToken);
                return cachedResult;
            }
        }

        var userValidation = _userRepository.Get(u => u.Id == request.Id);
        if (userValidation == null)
        {
            await _mediator.Publish(
                new DomainNotification("GetTracksByUser", _localizer.Text("NotFound")),
                cancellationToken
            );
            return default;
        }

        var tracksQuery = _trackRepository
            .GetRanged(x => x.UserId == request.Id && x.IsPublic)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        var likesByTrack = _trackLikeRepository
            .GetAll()
            .GroupBy(x => x.TrackId)
            .Select(g => new { TrackId = g.Key, Likes = g.ToList() });

        var playsByTrack = _trackPlayRepository
            .GetAll()
            .GroupBy(x => x.TrackId)
            .Select(g => new { TrackId = g.Key, Count = g.Count() });

        var commentsByTrack = _trackCommentRepository
            .GetAll()
            .GroupBy(x => x.TrackId)
            .Select(g => new { TrackId = g.Key, Count = g.Count() });

        var query = from track in tracksQuery
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
                        x.track.ImageFileName,
                        x.track.AudioFileUrl,
                        x.track.AudioFileName,
                        x.track.UserId,
                        Username = x.userProfile != null && !string.IsNullOrEmpty(x.userProfile.DisplayName)
                            ? x.userProfile.DisplayName
                            : x.user.Username ?? "Unknown",
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
                        ImageFileName = g.Key.ImageFileName,
                        AudioFileUrl = g.Key.AudioFileUrl,
                        AudioFileName = g.Key.AudioFileName,
                        Tags = g.Where(x => x.tag != null).Select(x => x.tag.Name).Distinct().ToArray(),
                        UserId = g.Key.UserId,
                        Username = g.Key.Username,
                        LikeCount = likesByTrack.FirstOrDefault(l => l.TrackId == g.Key.Id)?.Likes.Count ?? 0,
                        PlayCount = playsByTrack.FirstOrDefault(l => l.TrackId == g.Key.Id)?.Count ?? 0,
                        UserLikedTrack = likesByTrack.Any(l => l.TrackId == g.Key.Id && l.Likes.Any(like => like.UserId == _user.Id)),
                        CommentCount = commentsByTrack.FirstOrDefault(c => c.TrackId == g.Key.Id)?.Count ?? 0,
                        Duration = g.Key.Duration,
                        UpdatedAt = g.Key.UpdatedAt
                    };

        var tracks = query.ToList();
        var totalCount = _trackRepository
            .GetRanged(x => x.UserId == request.Id && x.IsPublic)
            .Count();

        var paginatedTracks = new PaginatedList<TrackViewModel>(
            tracks,
            totalCount,
            request.PageNumber,
            request.PageSize
        );

        await _mediator.Publish(
            new DomainSuccessNotification("GetTracksByUser", _localizer.Text("Success")),
            cancellationToken
        );

        var response = new GetTracksByUserQueryResponse { Tracks = paginatedTracks };
        await _redis.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5));
        return response;
    }
}