using AutoMapper;
using TaskManager.Application.Dtos.Other;
using TaskManager.Application.Dtos.Task;
using Task = TaskManager.Domain.Entities.Task;

namespace TaskManager.Application.AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Maps between generic PagedResponse types.
        CreateMap(typeof(PagedResponse<>), typeof(PagedResponse<>));

        // Maps between TaskPostDto and Task entities (for creating tasks).
        CreateMap<TaskPostDto, Task>()
            .ForMember(dest => dest.UserId, src => src.Ignore())
            .ForMember(dest => dest.CreatedAt, src => src.Ignore())
            .ForMember(dest => dest.UpdatedAt, src => src.Ignore());

        // Maps between Task and TaskGetDto (for retrieving task details).
        CreateMap<Task, TaskGetDto>();
    }
}
