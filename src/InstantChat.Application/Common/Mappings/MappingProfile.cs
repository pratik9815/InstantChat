using AutoMapper;
using InstantChat.Application.Features.Chat.Queries.GetChatHistory;
using InstantChat.Application.Features.Groups.Queries.GetOnlineUsers;
using InstantChat.Application.Features.Groups.Queries.GetUserGroups;
using InstantChat.Domain.Entities;
namespace InstantChat.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
            .ForMember(dest => dest.IsOnline, opt => opt.MapFrom(src => src.IsOnline));

        CreateMap<ChatMessage, ChatMessageDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId))
            .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.DisplayName))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImagePath))
            .ForMember(dest => dest.OriginalFileName, opt => opt.MapFrom(src => src.OriginalFileName))
            .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.FileSize))
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()); // Set manually in handler

        CreateMap<GroupChat, GroupChatDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ParticipantCount, opt => opt.MapFrom(src => src.Participants.Count(p => !p.IsDeleted)));
    }
}