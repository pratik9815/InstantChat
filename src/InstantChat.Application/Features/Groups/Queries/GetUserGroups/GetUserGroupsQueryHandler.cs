using AutoMapper;
using InstantChat.Domain.Entities;
using InstantChat.Domain.Interfaces;
using MediatR;

namespace InstantChat.Application.Features.Groups.Queries.GetUserGroups;

public class GetUserGroupsQueryHandler : IRequestHandler<GetUserGroupsQuery, GetUserGroupsResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUserGroupsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GetUserGroupsResponse> Handle(GetUserGroupsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get active participations for the user
            var userGroupParticipations = await _unitOfWork.GroupParticipants.FindAsync(p =>
                p.UserId == request.UserId);

            var groups = new List<GroupChat>();
            foreach (var participation in userGroupParticipations)
            {
                // Get the group if it's not soft-deleted
                var group = await _unitOfWork.Groups.FirstOrDefaultAsync(g =>
                    g.Id == participation.GroupChatId);
                if (group != null)
                {
                    groups.Add(group);
                }
            }
            var groupDtos = _mapper.Map<List<GroupChatDto>>(groups);
            return new GetUserGroupsResponse
            {
                Groups = groupDtos,
                Success = true,
                Message = "User groups retrieved successfully"
            };
        } 
        catch (Exception ex)
        {
            return new GetUserGroupsResponse
            {
                Success = false,
                Message = $"Error retrieving user groups: {ex.Message}"
            };
        }
    }
}