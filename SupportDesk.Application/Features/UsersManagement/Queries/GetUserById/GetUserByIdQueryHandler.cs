using AutoMapper;
using MediatR;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Models.Dtos;

namespace SupportDesk.Application.Features.UsersManagement.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdQueryResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<GetUserByIdQueryResponse> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var response = new GetUserByIdQueryResponse();

        var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);

        // Validaciones
        if (user == null)
        {
            response.Success = false;
            response.Message = UsersMessages.UserNotFound;
            return response;
        }

        if (!user.IsActive)
        {
            response.Success = false;
            response.Message = UsersMessages.UserIsInactive;
            return response;
        }

        response.User = _mapper.Map<UserDto>(user);
        response.Success = true;
        return response;
    }
}
