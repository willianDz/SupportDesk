﻿using AutoMapper;
using SupportDesk.Application.Models.Dtos;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Application.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Gender, GenderDto>();
        CreateMap<Request, RequestDto>();
        CreateMap<RequestDocument, RequestDocumentDto>();
        CreateMap<RequestStatus, RequestStatusDto>();
        CreateMap<RequestType, RequestTypeDto>();
        CreateMap<User, UserDto>();
        CreateMap<UserRequestType, UserRequestTypeDto>();
        CreateMap<UserZone, UserZoneDto>();
        CreateMap<Zone, ZoneDto>();
    }
}
