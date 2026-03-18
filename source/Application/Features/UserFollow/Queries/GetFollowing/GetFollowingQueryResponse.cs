using Project.Application.Common.Models;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.GetFollowing;

public record GetFollowingQueryResponse
{
    public PaginatedList<UserFollowViewModel> Following { get; set; } = default!;
}
