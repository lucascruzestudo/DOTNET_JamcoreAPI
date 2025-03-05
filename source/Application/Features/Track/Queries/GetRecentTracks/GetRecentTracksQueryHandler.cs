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
    private readonly IRepositoryBase<TrackLike> _trackLikeRepository;
    private readonly CultureLocalizer _localizer;

    public GetRecentTracksQueryHandler(IUnitOfWork unitOfWork, IMediator mediator, IRepositoryBase<Track> trackRepository, IRepositoryBase<TrackTag> trackTagRepository, IRepositoryBase<Tag> tagRepository, IRepositoryBase<User> userRepository, CultureLocalizer localizer, IRepositoryBase<TrackLike> trackLikeRepository)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _trackRepository = trackRepository;
        _trackTagRepository = trackTagRepository;
        _tagRepository = tagRepository;
        _userRepository = userRepository;
        _localizer = localizer;
        _trackLikeRepository = trackLikeRepository;
    }

    public async Task<GetRecentTracksQueryResponse?> Handle(GetRecentTracksQuery request, CancellationToken cancellationToken)
    {
        var query = _trackRepository
            .GetRanged(x => x.IsPublic)
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
                Username = _userRepository.Get(u => u.Id == x.UserId)?.Username!,
                LikeCount = _trackLikeRepository.GetRanged(like => like.TrackId == x.Id).Count()
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