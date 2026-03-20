using Microsoft.Extensions.Configuration;
using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Interfaces.Services;
using Project.Domain.Notifications;

namespace Project.Application.Features.Queries.GetTrackStream;

public class GetTrackStreamQueryHandler : IRequestHandler<GetTrackStreamQuery, GetTrackStreamQueryResponse?>
{
    private readonly IRepositoryBase<Track> _trackRepository;
    private readonly ISupabaseService _supabaseService;
    private readonly IMediator _mediator;
    private readonly IUser _user;
    private readonly CultureLocalizer _localizer;
    private readonly string _bucketName;

    public GetTrackStreamQueryHandler(
        IRepositoryBase<Track> trackRepository,
        ISupabaseService supabaseService,
        IMediator mediator,
        IUser user,
        CultureLocalizer localizer,
        IConfiguration configuration)
    {
        _trackRepository = trackRepository;
        _supabaseService = supabaseService;
        _mediator = mediator;
        _user = user;
        _localizer = localizer;
        _bucketName = configuration["Supabase:Bucket"]!;
    }

    public async Task<GetTrackStreamQueryResponse?> Handle(GetTrackStreamQuery request, CancellationToken cancellationToken)
    {
        var track = _trackRepository.Get(t => t.Id == request.TrackId);

        if (track == null)
        {
            await _mediator.Publish(
                new DomainNotification("GetTrackStream", _localizer.Text("NotFound")),
                cancellationToken);
            return null;
        }

        if (!track.IsPublic && track.UserId != _user.Id)
        {
            await _mediator.Publish(
                new DomainNotification("GetTrackStream", _localizer.Text("Forbidden")),
                cancellationToken);
            return null;
        }

        if (string.IsNullOrEmpty(track.AudioFileName))
        {
            await _mediator.Publish(
                new DomainNotification("GetTrackStream", _localizer.Text("NotFound")),
                cancellationToken);
            return null;
        }

        var signedUrl = await _supabaseService.GetSignedUrlAsync(track.AudioFileName, _bucketName, expiresIn: 3600);

        return new GetTrackStreamQueryResponse(signedUrl);
    }
}
