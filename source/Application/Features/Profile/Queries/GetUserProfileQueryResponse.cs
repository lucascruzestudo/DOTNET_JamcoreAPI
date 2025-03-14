using Project.Application.Common.Models;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.GetUserProfile;

public record GetUserProfileQueryResponse
{
    public UserProfileViewModel UserProfile { get; set; } = default!;
}

