using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Application.Common.Models;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.GetTracksByUser;

public class GetTracksByUserQueryHandler : IRequestHandler<GetTracksByUserQuery, GetTracksByUserQueryResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
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

    public GetTracksByUserQueryHandler(
        IUnitOfWork unitOfWork, IMediator mediator,
        IRepositoryBase<Track> trackRepository, IRepositoryBase<User> userRepository,
        IRepositoryBase<TrackTag> trackTagRepository, IRepositoryBase<Tag> tagRepository,
        CultureLocalizer localizer, IRepositoryBase<TrackLike> trackLikeRepository,
        IUser user, IRepositoryBase<TrackPlay> trackPlayRepository,
        IRepositoryBase<UserProfile> userProfileRepository)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _trackRepository = trackRepository;
        _userRepository = userRepository;
        _trackTagRepository = trackTagRepository;
        _tagRepository = tagRepository;
        _localizer = localizer;
        _trackLikeRepository = trackLikeRepository;
        _user = user;
        _trackPlayRepository = trackPlayRepository;
        _userProfileRepository = userProfileRepository;
    }

    public async Task<GetTracksByUserQueryResponse?> Handle(GetTracksByUserQuery request, CancellationToken cancellationToken)
    {
        var user = _userRepository.Get(u => u.Id == request.Id);

        if (user == null)
        {
            await _mediator.Publish(new DomainNotification("GetTracksByUser", _localizer.Text("NotFound")), cancellationToken);
            return default;
        }

        var trackIds = _trackRepository
            .GetRanged(x => x.UserId == user.Id && x.IsPublic)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.Id)
            .ToList();

        var trackTags = _trackTagRepository.GetRanged(tt => trackIds.Contains(tt.TrackId)).ToList();
        var tagIds = trackTags.Select(tt => tt.TagId).Distinct().ToList();
        var tags = _tagRepository.GetRanged(x => tagIds.Contains(x.Id)).ToDictionary(x => x.Id, x => x.Name);

        var trackLikeCounts = _trackLikeRepository.GetRanged(like => trackIds.Contains(like.TrackId))
            .GroupBy(like => like.TrackId)
            .ToDictionary(g => g.Key, g => g.Count());

        var trackPlayCounts = _trackPlayRepository.GetRanged(play => trackIds.Contains(play.TrackId))
            .GroupBy(play => play.TrackId)
            .ToDictionary(g => g.Key, g => g.Count());

        var userProfile = _userProfileRepository.Get(up => up.UserId == user.Id);
        var displayName = userProfile?.DisplayName ?? user.Username;

        var query = _trackRepository
            .GetRanged(x => trackIds.Contains(x.Id))
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
                Tags = trackTags.Where(tt => tt.TrackId == x.Id)
                    .Select(tt => tags.GetValueOrDefault(tt.TagId, "Unknown"))
                    .ToArray(),
                UserId = x.UserId,
                Username = displayName,
                LikeCount = trackLikeCounts.GetValueOrDefault(x.Id, 0),
                PlayCount = trackPlayCounts.GetValueOrDefault(x.Id, 0),
                UserLikedTrack = _trackLikeRepository.Get(like => like.TrackId == x.Id && like.UserId == _user.Id) != null,
                Duration = x.Duration,
                UpdatedAt = x.UpdatedAt ?? x.CreatedAt
            }).AsQueryable();

        var paginatedTracks = PaginatedList<TrackViewModel>.Create(query, request.PageNumber, request.PageSize);

        _unitOfWork.Commit();
        await _mediator.Publish(new DomainSuccessNotification("GetTracksByUser", _localizer.Text("Success")), cancellationToken);

        return new GetTracksByUserQueryResponse
        {
            Tracks = paginatedTracks
        };
    }

}
