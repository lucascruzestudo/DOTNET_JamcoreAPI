namespace Project.Application.Features.Queries.GetTrackStream;

public record GetTrackStreamQuery(Guid TrackId) : IRequest<GetTrackStreamQueryResponse?>;

public record GetTrackStreamQueryResponse(string SignedUrl);
