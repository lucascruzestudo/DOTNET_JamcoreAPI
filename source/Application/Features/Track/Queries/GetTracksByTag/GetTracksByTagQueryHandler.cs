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

namespace Project.Application.Features.Commands.GetTracksByTag;

public class GetTracksByTagQueryHandler : IRequestHandler<GetTracksByTagQuery, GetTracksByTagQueryResponse?>
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

    public GetTracksByTagQueryHandler(
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

    public async Task<GetTracksByTagQueryResponse?> Handle(GetTracksByTagQuery request, CancellationToken cancellationToken)
    {
        var matchingTag = _tagRepository.Get(x => x.Name == request.Tag);
        if (matchingTag == null)
        {
            await _mediator.Publish(
                new DomainNotification("GetTracksByTag", _localizer.Text("NotFound")),
                cancellationToken
            );
            return default;
        }

        var tracksQuery = from trackTag in _trackTagRepository.GetRanged(tt => tt.TagId == matchingTag.Id)
                          join track in _trackRepository.GetRanged(t => t.IsPublic)
                              on trackTag.TrackId equals track.Id
                          orderby track.CreatedAt descending
                          select track;

        tracksQuery = tracksQuery
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
                        Duration = g.Key.Duration,
                        UpdatedAt = g.Key.UpdatedAt
                    };

        var tracks = query.ToList();
        var totalCount = (from trackTag in _trackTagRepository.GetRanged(tt => tt.TagId == matchingTag.Id)
                         join track in _trackRepository.GetRanged(t => t.IsPublic)
                             on trackTag.TrackId equals track.Id
                         select track.Id).Count();

        var paginatedTracks = new PaginatedList<TrackViewModel>(
            tracks,
            totalCount,
            request.PageNumber,
            request.PageSize
        );

        await _mediator.Publish(
            new DomainSuccessNotification("GetTracksByTag", _localizer.Text("Success")),
            cancellationToken
        );

        return new GetTracksByTagQueryResponse
        {
            Tracks = paginatedTracks
        };
    }
}