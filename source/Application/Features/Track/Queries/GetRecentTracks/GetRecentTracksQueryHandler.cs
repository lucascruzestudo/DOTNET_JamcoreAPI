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

namespace Project.Application.Features.Commands.GetRecentTracks;

public class GetRecentTracksQueryHandler : IRequestHandler<GetRecentTracksQuery, GetRecentTracksQueryResponse?>
{
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

    public GetRecentTracksQueryHandler(
        IMediator mediator,
        IRepositoryBase<Track> trackRepository,
        IRepositoryBase<TrackTag> trackTagRepository,
        IRepositoryBase<Tag> tagRepository,
        IRepositoryBase<User> userRepository,
        IRepositoryBase<UserProfile> userProfileRepository,
        IRepositoryBase<TrackLike> trackLikeRepository,
        IRepositoryBase<TrackPlay> trackPlayRepository,
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
        _localizer = localizer;
        _user = user;
    }

    public async Task<GetRecentTracksQueryResponse?> Handle(GetRecentTracksQuery request, CancellationToken cancellationToken)
    {
        // Base query for public tracks with pagination
        var tracksQuery = _trackRepository
            .GetRanged(x => x.IsPublic)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        // Join tracks with related data
        var query = from track in tracksQuery
                    join user in _userRepository.GetAll() on track.UserId equals user.Id
                    join userProfile in _userProfileRepository.GetAll() on track.UserId equals userProfile.UserId into userProfiles
                    from userProfile in userProfiles.DefaultIfEmpty()
                    join trackTag in _trackTagRepository.GetAll() on track.Id equals trackTag.TrackId into trackTags
                    from trackTag in trackTags.DefaultIfEmpty()
                    join tag in _tagRepository.GetAll() on trackTag.TagId equals tag.Id into tags
                    from tag in tags.DefaultIfEmpty()
                    join trackLike in _trackLikeRepository.GetAll() on track.Id equals trackLike.TrackId into trackLikes
                    from trackLike in trackLikes.DefaultIfEmpty()
                    join trackPlay in _trackPlayRepository.GetAll() on track.Id equals trackPlay.TrackId into trackPlays
                    from trackPlay in trackPlays.DefaultIfEmpty()
                    group new { track, user, userProfile, tag, trackLike, trackPlay } by new
                    {
                        track.Id,
                        track.Title,
                        track.Description,
                        track.CreatedAt,
                        track.ImageUrl,
                        track.ImageFileName,
                        track.AudioFileUrl,
                        track.AudioFileName,
                        track.UserId,
                        Username = userProfile != null && !string.IsNullOrEmpty(userProfile.DisplayName)
                            ? userProfile.DisplayName
                            : user.Username ?? "null",
                        track.Duration,
                        UpdatedAt = track.UpdatedAt ?? track.CreatedAt
                    } into g
                    select new TrackViewModel
                    {
                        Id = g.Key.Id,
                        Title = g.Key.Title,
                        Description = g.Key.Description,
                        CreatedAt = g.Key.CreatedAt,
                        ImageUrl = g.Key.ImageUrl,
                        AudioFileUrl = g.Key.AudioFileUrl,
                        AudioFileName = g.Key.AudioFileName,
                        Tags = g.Where(x => x.tag != null).Select(x => x.tag.Name).Distinct().ToArray(),
                        UserId = g.Key.UserId,
                        Username = g.Key.Username,
                        LikeCount = g.Count(x => x.trackLike != null),
                        PlayCount = g.Count(x => x.trackPlay != null),
                        UserLikedTrack = g.Any(x => x.trackLike != null && x.trackLike.UserId == _user.Id),
                        Duration = g.Key.Duration,
                        UpdatedAt = g.Key.UpdatedAt
                    };

        // Execute query and get total count for pagination
        var tracks = query.ToList();
        var totalCount = _trackRepository.GetRanged(x => x.IsPublic).Count();

        // Create paginated list
        var paginatedTracks = new PaginatedList<TrackViewModel>(
            tracks,
            totalCount,
            request.PageNumber,
            request.PageSize
        );

        // Publish success notification
        await _mediator.Publish(
            new DomainSuccessNotification("GetRecentTracks", _localizer.Text("Success")),
            cancellationToken
        );

        return new GetRecentTracksQueryResponse
        {
            Tracks = paginatedTracks
        };
    }
}