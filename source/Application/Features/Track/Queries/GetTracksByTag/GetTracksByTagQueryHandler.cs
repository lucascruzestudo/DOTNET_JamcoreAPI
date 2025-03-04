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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly IRepositoryBase<Track> _trackRepository;
    private readonly IRepositoryBase<TrackTag> _trackTagRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<Tag> _tagRepository;
    private readonly CultureLocalizer _localizer;

    public GetTracksByTagQueryHandler(IUnitOfWork unitOfWork, IMediator mediator, IRepositoryBase<Track> trackRepository, IRepositoryBase<TrackTag> trackTagRepository, IRepositoryBase<Tag> tagRepository, CultureLocalizer localizer, IRepositoryBase<User> userRepository)
    {
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _trackRepository = trackRepository;
        _trackTagRepository = trackTagRepository;
        _tagRepository = tagRepository;
        _localizer = localizer;
        _userRepository = userRepository;
    }

    public async Task<GetTracksByTagQueryResponse?> Handle(GetTracksByTagQuery request, CancellationToken cancellationToken)
    {
        var matchingTag = _tagRepository.Get(x => x.Name == request.Tag);

        if (matchingTag == null)
        {
            await _mediator.Publish(new DomainNotification("GetTracksByTag", _localizer.Text("NotFound")), cancellationToken);
            return default;
        }

        var matchingTracks = _trackTagRepository
            .GetRanged(x => x.TagId == matchingTag.Id)
            .Select(x => x.TrackId)
            .ToList();

        var query = _trackRepository
            .GetRanged(x => matchingTracks.Contains(x.Id) && x.IsPublic)
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
            }).AsQueryable();
            
        var paginatedTracks = PaginatedList<TrackViewModel>.Create(query, request.PageNumber, request.PageSize);

        _unitOfWork.Commit();
        await _mediator.Publish(new DomainSuccessNotification("GetTracksByTag", _localizer.Text("Success")), cancellationToken);

        return new GetTracksByTagQueryResponse
        {
            Tracks = paginatedTracks
        };
    }
}
