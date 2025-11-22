using AutoMapper;
using ToDo.Api.Dtos;
using ToDo.Api.Models;

namespace ToDo.Api.Mapping;

public class TaskItemProfile : Profile
{
    public TaskItemProfile()
    {
        // Domain -> DTO
        CreateMap<TaskItem, TaskItemDto>();

        // Create request -> Domain
        CreateMap<CreateTaskItemRequest, TaskItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.IsComplete, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // Update request -> existing Domain entity
        CreateMap<UpdateTaskItemRequest, TaskItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}
