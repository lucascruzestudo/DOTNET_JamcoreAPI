using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Application.Common.Models;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Interfaces.Services;
using Project.Domain.Notifications;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.GetRecentCommentsByUser;

public class GetRecentCommentsByUserQueryHandler : IRequestHandler<GetRecentCommentsByUserQuery, GetRecentCommentsByUserQueryResponse?>
{
    private readonly IMediator _mediator;
    private readonly IRepositoryBase<TrackComment> _trackCommentRepository;
    private readonly IRepositoryBase<Track> _trackRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<UserProfile> _userProfileRepository;
    private readonly CultureLocalizer _localizer;
    private readonly IRedisService _redis;

    public GetRecentCommentsByUserQueryHandler(
        IMediator mediator,
        IRepositoryBase<TrackComment> trackCommentRepository,
        IRepositoryBase<Track> trackRepository,
        IRepositoryBase<User> userRepository,
        IRepositoryBase<UserProfile> userProfileRepository,
        CultureLocalizer localizer,
        IRedisService redis)
    {
        _mediator = mediator;
        _trackCommentRepository = trackCommentRepository;
        _trackRepository = trackRepository;
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _localizer = localizer;
        _redis = redis;
    }

    public async Task<GetRecentCommentsByUserQueryResponse?> Handle(GetRecentCommentsByUserQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"comments:user:{request.UserId}:{request.PageNumber}:{request.PageSize}";
        var cached = await _redis.GetAsync<GetRecentCommentsByUserQueryResponse>(cacheKey);
        if (cached != null)
        {
            var cachedResult = JsonSerializer.Deserialize<GetRecentCommentsByUserQueryResponse>(cached);
            if (cachedResult != null)
            {
                await _mediator.Publish(new DomainSuccessNotification("GetRecentCommentsByUser", _localizer.Text("Success")), cancellationToken);
                return cachedResult;
            }
        }

        var commentsQuery = _trackCommentRepository
            .GetRanged(x => x.UserId == request.UserId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        var commentData = (from comment in commentsQuery
                           join track in _trackRepository.GetAll()
                               on comment.TrackId equals track.Id
                           join user in _userRepository.GetAll()
                               on comment.UserId equals user.Id
                           join userProfile in _userProfileRepository.GetAll()
                               on comment.UserId equals userProfile.UserId into userProfiles
                           from userProfile in userProfiles.DefaultIfEmpty()
                           select new CommentViewModel
                           {
                               Id = comment.Id,
                               Text = comment.Comment,
                               UserId = comment.UserId,
                               Username = user.Username,
                               DisplayName = userProfile?.DisplayName ?? string.Empty,
                               CreatedAt = comment.CreatedAt,
                               TrackId = comment.TrackId,
                               TrackName = track.Title
                           }).ToList();

        var totalCount = _trackCommentRepository
            .GetRanged(x => x.UserId == request.UserId)
            .Count();

        var paginatedComments = new PaginatedList<CommentViewModel>(
            commentData,
            totalCount,
            request.PageNumber,
            request.PageSize
        );

        await _mediator.Publish(
            new DomainSuccessNotification("GetRecentCommentsByUser", _localizer.Text("Success")),
            cancellationToken
        );

        var response = new GetRecentCommentsByUserQueryResponse { Tracks = paginatedComments };
        await _redis.SetAsync(cacheKey, response, TimeSpan.FromMinutes(2));
        return response;
    }

}