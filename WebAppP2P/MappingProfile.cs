using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppP2P.Core.Messages;
using WebAppP2P.Dto;

namespace WebAppP2P
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ClientInternalMessage, InternalMessage>();
            CreateMap<InternalMessage, ClientInternalMessage>();

            CreateMap<ClientInternalMessage,InternalMessageConfirmation>();

            CreateMap<InternalMessage, DecryptedMessageDto>();
            CreateMap<DecryptedMessageDto, InternalMessage>();
        }
    }
}
