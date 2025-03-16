using Project.Application.Common.Models;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.GetUserProfileById;

public record GetUserProfileByIdQueryResponse
{
    public UserProfileViewModel UserProfile { get; set; } = default!;
}

