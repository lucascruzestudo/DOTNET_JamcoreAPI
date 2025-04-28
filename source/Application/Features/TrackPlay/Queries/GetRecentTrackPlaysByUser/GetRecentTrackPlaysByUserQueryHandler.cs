
using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Application.Common.Models;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Queries.GetRecentTrackPlaysByUser;

public class GetRecentTrackPlaysByUserQueryHandler : IRequestHandler<GetRecentTrackPlaysByUserQuery, GetRecentTrackPlaysByUserQueryResponse?>
{
    private readonly IMediator _mediator;
    private readonly IRepositoryBase<Track> _trackRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<UserProfile> _userProfileRepository;
    private readonly IRepositoryBase<TrackTag> _trackTagRepository;
    private readonly IRepositoryBase<Tag> _tagRepository;
    private readonly IRepositoryBase<TrackLike> _trackLikeRepository;
    private readonly IRepositoryBase<TrackPlay> _trackPlayRepository;
    private readonly CultureLocalizer _localizer;
    private readonly IUser _user;

    public GetRecentTrackPlaysByUserQueryHandler(
        IMediator mediator,
        IRepositoryBase<Track> trackRepository,
        IRepositoryBase<User> userRepository,
        IRepositoryBase<UserProfile> userProfileRepository,
        IRepositoryBase<TrackTag> trackTagRepository,
        IRepositoryBase<Tag> tagRepository,
        IRepositoryBase<TrackLike> trackLikeRepository,
        IRepositoryBase<TrackPlay> trackPlayRepository,
        CultureLocalizer localizer,
        IUser user)
    {
        _mediator = mediator;
        _trackRepository = trackRepository;
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _trackTagRepository = trackTagRepository;
        _tagRepository = tagRepository;
        _trackLikeRepository = trackLikeRepository;
        _trackPlayRepository = trackPlayRepository;
        _localizer = localizer;
        _user = user;
    }

    public async Task<GetRecentTrackPlaysByUserQueryResponse?> Handle(GetRecentTrackPlaysByUserQuery request, CancellationToken cancellationToken)
    {
        var userValidation = _userRepository.Get(u => u.Id == request.UserId);
        if (userValidation == null)
        {
            await _mediator.Publish(
                new DomainNotification("GetRecentTrackPlaysByUser", _localizer.Text("NotFound")),
                cancellationToken
            );
            return default;
        }

        var trackPlaysQuery = _trackPlayRepository
            .GetRanged(tp => tp.UserId == request.UserId)
            .OrderByDescending(tp => tp.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        var totalCount = _trackPlayRepository
            .GetRanged(tp => tp.UserId == request.UserId)
            .Count();

        var likesByTrack = _trackLikeRepository
            .GetAll()
            .GroupBy(x => x.TrackId)
            .Select(g => new { TrackId = g.Key, Likes = g.ToList() });

        var playsByTrack = _trackPlayRepository
            .GetAll()
            .GroupBy(x => x.TrackId)
            .Select(g => new { TrackId = g.Key, Count = g.Count() });

        var query = from trackPlay in trackPlaysQuery
                    join track in _trackRepository.GetAll() on trackPlay.TrackId equals track.Id
                    join user in _userRepository.GetAll() on track.UserId equals user.Id
                    join userProfile in _userProfileRepository.GetAll() on track.UserId equals userProfile.UserId into userProfiles
                    from userProfile in userProfiles.DefaultIfEmpty()
                    join trackTag in _trackTagRepository.GetAll() on track.Id equals trackTag.TrackId into trackTags
                    from trackTag in trackTags.DefaultIfEmpty()
                    join tag in _tagRepository.GetAll() on trackTag?.TagId equals tag.Id into tags
                    from tag in tags.DefaultIfEmpty()
                    select new { track, user, userProfile, tag, trackPlay } into x
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
                        UpdatedAt = x.track.UpdatedAt ?? x.track.CreatedAt,
                        PlayedAt = x.trackPlay.CreatedAt
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
                        Duration = g.Key.Duration,
                        UpdatedAt = g.Key.UpdatedAt
                    };

        var tracks = query.ToList();
        var paginatedTracks = new PaginatedList<TrackViewModel>(
            tracks,
            totalCount,
            request.PageNumber,
            request.PageSize
        );

        await _mediator.Publish(
            new DomainSuccessNotification("GetRecentTrackPlaysByUser", _localizer.Text("Success")),
            cancellationToken
        );

        return new GetRecentTrackPlaysByUserQueryResponse
        {
            Tracks = paginatedTracks
        };
    }
}