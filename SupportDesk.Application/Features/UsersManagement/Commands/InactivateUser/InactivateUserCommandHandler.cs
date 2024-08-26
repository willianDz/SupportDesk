using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Models.Dtos;

namespace SupportDesk.Application.Features.UsersManagement.Commands.InactivateUser;

public class InactivateUserCommandHandler : IRequestHandler<InactivateUserCommand, InactivateUserCommandResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<InactivateUserCommandHandler> _logger;

    public InactivateUserCommandHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<InactivateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<InactivateUserCommandResponse> Handle(InactivateUserCommand request, CancellationToken cancellationToken)
    {
        var response = new InactivateUserCommandResponse();

        // Validar existencia del usuario
        var userToInactivate = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (userToInactivate == null)
        {
            response.Success = false;
            response.Message = UsersMessages.UserNotFound;
            return response;
        }

        // Inactivar el usuario
        userToInactivate.IsActive = false;
        userToInactivate.LastModifiedDate = DateTime.UtcNow;

        await _userRepository.UpdateAsync(userToInactivate, cancellationToken);

        // Mapear usuario inactivado a DTO
        response.InactivatedUser = _mapper.Map<UserDto>(userToInactivate);
        response.Success = true;

        _logger.LogInformation($"Usuario con ID {userToInactivate.Id} ha sido inactivado.");

        return response;
    }
}
