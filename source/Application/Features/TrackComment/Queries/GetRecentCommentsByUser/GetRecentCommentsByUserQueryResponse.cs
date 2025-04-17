using Project.Application.Common.Models;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.GetRecentCommentsByUser;

public record GetRecentCommentsByUserQueryResponse
{
    public PaginatedList<CommentsViewModel> Tracks { get; set; } = default!;
}

