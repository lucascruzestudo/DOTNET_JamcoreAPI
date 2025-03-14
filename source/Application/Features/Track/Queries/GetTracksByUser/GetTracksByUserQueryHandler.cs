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

    public GetTracksByUserQueryHandler(IUnitOfWork unitOfWork, IMediator mediator, IRepositoryBase<Track> trackRepository, IRepositoryBase<User> userRepository, IRepositoryBase<TrackTag> trackTagRepository, IRepositoryBase<Tag> tagRepository, CultureLocalizer localizer, IRepositoryBase<TrackLike> trackLikeRepository, IUser user, IRepositoryBase<TrackPlay> trackPlayRepository, IRepositoryBase<UserProfile> userProfileRepository)
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
        var user = _userRepository.Get(u => u.Username == request.Username);
        
        if (user == null)
        {
            await _mediator.Publish(new DomainNotification("GetTracksByUser", _localizer.Text("NotFound")), cancellationToken);
            return default;
        }

        var query = _trackRepository
            .GetRanged(x => x.UserId == user.Id && x.IsPublic)
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
                Tags = [.. _trackTagRepository
                    .GetRanged(tt => tt.TrackId == x.Id)
                    .Join(_tagRepository.GetAll(), tt => tt.TagId, t => t.Id, (tt, t) => t.Name)],
                UserId = x.UserId,
                Username = _userProfileRepository.Get(u => u.UserId == x.UserId)?.DisplayName != null && _userProfileRepository.Get(u => u.UserId == x.UserId)?.DisplayName != ""
                    ? $"{_userProfileRepository.Get(u => u.UserId == x.UserId)?.DisplayName} · @{_userRepository.Get(u => u.Id == x.UserId)?.Username!}"
                    : $"@{_userRepository.Get(u => u.Id == x.UserId)?.Username!}",
                LikeCount = _trackLikeRepository.GetRanged(like => like.TrackId == x.Id).Count(),
                PlayCount = _trackPlayRepository.GetRanged(play => play.TrackId == x.Id).Count(),
                UserLikedTrack = _trackLikeRepository.Get(like => like.TrackId == x.Id && like.UserId == _user.Id) != null,
                Duration = x.Duration
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
