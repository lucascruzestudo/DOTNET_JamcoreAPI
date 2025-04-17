using System;
using System.Collections.Generic;
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

namespace Project.Application.Features.Commands.GetRecentCommentsByUser;

public class GetRecentCommentsByUserQueryHandler : IRequestHandler<GetRecentCommentsByUserQuery, GetRecentCommentsByUserQueryResponse?>
{
    private readonly IMediator _mediator;
    private readonly IRepositoryBase<TrackComment> _trackCommentRepository;
    private readonly IRepositoryBase<Track> _trackRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<UserProfile> _userProfileRepository;
    private readonly CultureLocalizer _localizer;

    public GetRecentCommentsByUserQueryHandler(
        IMediator mediator,
        IRepositoryBase<TrackComment> trackCommentRepository,
        IRepositoryBase<Track> trackRepository,
        IRepositoryBase<User> userRepository,
        IRepositoryBase<UserProfile> userProfileRepository,
        CultureLocalizer localizer)
    {
        _mediator = mediator;
        _trackCommentRepository = trackCommentRepository;
        _trackRepository = trackRepository;
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _localizer = localizer;
    }

    public async Task<GetRecentCommentsByUserQueryResponse?> Handle(GetRecentCommentsByUserQuery request, CancellationToken cancellationToken)
    {
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
                           select new
                           {
                               comment.Id,
                               comment.Comment,
                               comment.UserId,
                               comment.CreatedAt,
                               comment.TrackId,
                               TrackTitle = track.Title,
                               Username = user.Username,
                               DisplayName = userProfile != null ? userProfile.DisplayName : string.Empty
                           }).ToList();

        var commentsByTrack = commentData
            .GroupBy(c => c.TrackId)
            .Select(g => new CommentsViewModel
            {
                Comments = g.Select(c => new CommentsViewModel.CommentViewModel
                {
                    Id = c.Id,
                    Text = c.Comment,
                    UserId = c.UserId,
                    Username = c.Username,
                    DisplayName = c.DisplayName,
                    CreatedAt = c.CreatedAt,
                    TrackId = c.TrackId,
                    TrackName = c.TrackTitle
                }).ToList()
            }).ToList();

        var totalCount = _trackCommentRepository
            .GetRanged(x => x.UserId == request.UserId)
            .Count();

        var paginatedTracks = new PaginatedList<CommentsViewModel>(
            commentsByTrack,
            totalCount,
            request.PageNumber,
            request.PageSize
        );

        await _mediator.Publish(
            new DomainSuccessNotification("GetRecentCommentsByUser", _localizer.Text("Success")),
            cancellationToken
        );

        return new GetRecentCommentsByUserQueryResponse
        {
            Tracks = paginatedTracks
        };
    }
}