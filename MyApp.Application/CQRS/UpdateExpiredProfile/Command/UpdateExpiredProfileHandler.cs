using MediatR;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Common.Message;
using MyApp.Application.Interfaces.UpdateExpiredProfile;

namespace MyApp.Application.CQRS.UpdateExpiredProfile.Command
{
    public class UpdateExpiredProfileHandler
        : IRequestHandler<UpdateExpiredProfileRequest, UpdateExpiredProfileResponse>
    {
        private readonly IUpdateExpiredProfileRepository _updateExpiredProfileRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateExpiredProfileHandler(
            IUpdateExpiredProfileRepository updateExpiredProfileRepository,
            ICurrentUserService currentUserService
        )
        {
            _updateExpiredProfileRepository = updateExpiredProfileRepository;
            _currentUserService = currentUserService;
        }

        public async Task<UpdateExpiredProfileResponse> Handle(
            UpdateExpiredProfileRequest request,
            CancellationToken cancellationToken
        )
        {
            var userId = _currentUserService.GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                return new UpdateExpiredProfileResponse
                {
                    Code = 401,
                    Message = Message.LOGIN_INFO_NOT_FOUND,
                };
            }

            var user = await _updateExpiredProfileRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return new UpdateExpiredProfileResponse
                {
                    Code = 404,
                    Message = Message.USER_DOES_NOT_EXSIT,
                };
            }

            if (request.CitizenIdentification != user.CitizenIdentification)
            {
                return new UpdateExpiredProfileResponse
                {
                    Code = 400,
                    Message = Message.CITIZEN_IDENTIFICATION_NOT_MATCH,
                };
            }

            try
            {
                user.Id = Guid.Parse(userId);
                user.Name = request.Name;
                user.BirthDay = request.BirthDay;
                user.Nationality = request.Nationality;
                user.Gender = request.Gender;
                user.ValidDate = request.ValidDate;
                user.OriginLocation = request.OriginLocation;
                user.RecentLocation = request.RecentLocation;
                user.IssueDate = request.IssueDate;
                user.IssueBy = request.IssueBy;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = Guid.Parse(userId);

                await _updateExpiredProfileRepository.UpdateUserAsync(user);

                return new UpdateExpiredProfileResponse
                {
                    Code = 200,
                    Message = Message.UPDATE_PROFILE_SUCCESS,
                };
            }
            catch (Exception)
            {
                return new UpdateExpiredProfileResponse
                {
                    Code = 500,
                    Message = Message.SYSTEM_ERROR,
                };
            }
        }
    }
}
