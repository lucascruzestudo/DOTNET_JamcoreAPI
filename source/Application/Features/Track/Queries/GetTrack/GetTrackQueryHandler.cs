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

namespace Project.Application.Features.Commands.GetTrack;

public class GetTrackQueryHandler : IRequestHandler<GetTrackQuery, GetTrackQueryResponse?>
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
    private readonly IRepositoryBase<TrackComment> _trackCommentRepository;
    private readonly CultureLocalizer _localizer;
    private readonly IUser _user;

    public GetTrackQueryHandler(
        IUnitOfWork unitOfWork,
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
        _unitOfWork = unitOfWork;
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

    public async Task<GetTrackQueryResponse?> Handle(GetTrackQuery request, CancellationToken cancellationToken)
    {
        var track = _trackRepository.Get(x => x.Id == request.TrackId);
        if (track == null)
        {
            await _mediator.Publish(
                new DomainNotification("GetTrack", _localizer.Text("NotFound")),
                cancellationToken);
            return default;
        }

        var likesByTrack = _trackLikeRepository
            .GetAll()
            .GroupBy(x => x.TrackId)
            .Select(g => new { TrackId = g.Key, Likes = g.ToList() })
            .FirstOrDefault(x => x.TrackId == track.Id);

        var playsByTrack = _trackPlayRepository
            .GetAll()
            .GroupBy(x => x.TrackId)
            .Select(g => new { TrackId = g.Key, Count = g.Count() })
            .FirstOrDefault(x => x.TrackId == track.Id);

        var trackTags = _trackTagRepository.GetRanged(x => x.TrackId == track.Id);
        var tagIds = trackTags.Select(tt => tt.TagId).Distinct();
        var tags = _tagRepository
            .GetRanged(x => tagIds.Contains(x.Id))
            .Select(t => t.Name)
            .ToArray();

        var author = _userRepository.Get(u => u.Id == track.UserId);
        var authorProfile = _userProfileRepository.Get(u => u.UserId == track.UserId);
        var username = authorProfile?.DisplayName ?? author?.Username ?? "Unknown";

        var commentsQuery = from comment in _trackCommentRepository.GetRanged(x => x.TrackId == track.Id)
                           join user in _userRepository.GetAll() on comment.UserId equals user.Id
                           join userProfile in _userProfileRepository.GetAll() on comment.UserId equals userProfile.UserId into profiles
                           from userProfile in profiles.DefaultIfEmpty()
                           select new TrackViewModel.Comment
                           {
                               Id = comment.Id,
                               Text = comment.Comment,
                               UserId = comment.UserId,
                               Username = user.Username ?? "unknown",
                               DisplayName = userProfile?.DisplayName ?? user.Username ?? "unknown",
                               UserProfilePictureUrl = userProfile?.ProfilePictureUrl ?? string.Empty,
                               UserProfileUpdatedAt = userProfile?.UpdatedAt ?? userProfile?.CreatedAt,
                               CreatedAt = comment.CreatedAt
                           };

        var comments = commentsQuery.ToList();

        var trackResponse = new TrackViewModel
        {
            Id = track.Id,
            Title = track.Title,
            Description = track.Description,
            CreatedAt = track.CreatedAt,
            ImageUrl = track.ImageUrl,
            ImageFileName = track.ImageFileName,
            AudioFileUrl = track.AudioFileUrl,
            AudioFileName = track.AudioFileName,
            Tags = tags,
            UserId = track.UserId,
            Username = username,
            LikeCount = likesByTrack?.Likes.Count ?? 0,
            PlayCount = playsByTrack?.Count ?? 0,
            UserLikedTrack = likesByTrack?.Likes.Any(like => like.UserId == _user.Id) ?? false,
            Duration = track.Duration,
            Comments = comments,
            UpdatedAt = track.UpdatedAt ?? track.CreatedAt
        };

        _unitOfWork.Commit();

        await _mediator.Publish(
            new DomainSuccessNotification("GetTrack", _localizer.Text("Success")),
            cancellationToken);

        return new GetTrackQueryResponse
        {
            Track = trackResponse
        };
    }
}