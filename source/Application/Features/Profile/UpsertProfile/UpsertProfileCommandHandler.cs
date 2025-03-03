using System;
using System.Threading;
using System.Threading.Tasks;
using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Application.Features.Commands.UpsertProfile;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Interfaces.Services;
using Project.Domain.Notifications;
using MediatR;

namespace Project.Application.Features.Email.Commands.UpsertProfile
{
    public class UpsertProfileCommandHandler(
        IUnitOfWork unitOfWork,
        IMediator mediator,
        IEmailService emailService,
        IUser user,
        ISupabaseService supabaseService,
        CultureLocalizer localizer,
        IRepositoryBase<User> userRepository,
        IRepositoryBase<UserProfile> userProfileRepository
    ) : IRequestHandler<UpsertProfileCommand, UpsertProfileCommandResponse?>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMediator _mediator = mediator;
        private readonly IEmailService _emailService = emailService;
        private readonly IUser _user = user;
        private readonly ISupabaseService _supabaseService = supabaseService;
        private readonly CultureLocalizer _localizer = localizer;
        private readonly IRepositoryBase<User> _userRepository = userRepository;
        private readonly IRepositoryBase<UserProfile> _userProfileRepository = userProfileRepository;

        public async Task<UpsertProfileCommandResponse?> Handle(UpsertProfileCommand command, CancellationToken cancellationToken)
        {
            
            var user = _userRepository.Get(x => x.Id == _user.Id);

            if (user == null)
            {
                await _mediator.Publish(new DomainNotification("UpsertProfile", _localizer.Text("InvalidUser")), cancellationToken);
                return default;
            }

            var userProfile = _userProfileRepository.Get(x => x.UserId == user.Id);
            if (userProfile == null)
            {
                userProfile = new UserProfile
                {
                    UserId = user.Id,
                    DisplayName = command.Request.DisplayName,
                    Bio = command.Request.Bio,
                    Location = command.Request.Location,
                };
                _userProfileRepository.Add(userProfile);
            }
            else
            {
                userProfile.DisplayName = command.Request.DisplayName;
                userProfile.Bio = command.Request.Bio;
                userProfile.Location = command.Request.Location;
            }

            if (command.Image != null)
            {
                string fileName = $"profile_{user.Id}.jpg";
                if (!string.IsNullOrEmpty(userProfile.ProfilePictureUrl))
                {
                    string publicUrl = await _supabaseService.GetPublicUrlAsync(fileName);
                    if (!string.IsNullOrEmpty(publicUrl))
                    {
                        await _supabaseService.DeleteFileAsync(fileName);
                    }
                }

                string fileUrl = await _supabaseService.UploadFileAsync(command.Image, fileName);

                if (string.IsNullOrEmpty(fileUrl))
                {
                    await _mediator.Publish(new DomainNotification("UpsertProfile", _localizer.Text("UploadFailed")), cancellationToken);
                    return null;
                }

                userProfile.ProfilePictureUrl = fileUrl;
            }

            _userProfileRepository.Update(userProfile);
            _unitOfWork.Commit();

            await _mediator.Publish(new DomainSuccessNotification("UpsertProfile", _localizer.Text("Success")), cancellationToken);

            return new UpsertProfileCommandResponse
            {
                DisplayName = userProfile.DisplayName ?? string.Empty,
                Bio = userProfile.Bio ?? string.Empty,
                Location = userProfile.Location ?? string.Empty,
                ProfilePictureUrl = userProfile.ProfilePictureUrl ?? string.Empty
            };
        }
    }
}
