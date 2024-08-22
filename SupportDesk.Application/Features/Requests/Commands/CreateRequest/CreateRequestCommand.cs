﻿using MediatR;

namespace SupportDesk.Application.Features.Requests.Commands.CreateRequest;

public class CreateRequestCommand : IRequest<CreateRequestCommandResponse>
{
    public Guid UserId { get; set; }
    public int RequestTypeId { get; set; }
    public int ZoneId { get; set; }
    public string Comments { get; set; } = string.Empty!;
}

