using Project.Application.Common.Models;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.GetFollowers;

public record GetFollowersQueryResponse
{
    public PaginatedList<UserFollowViewModel> Followers { get; set; } = default!;
}
