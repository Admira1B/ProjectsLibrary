using AutoMapper;
using ProjectsLibrary.DTOs.Project;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.DTOs.Task;
using ProjectsLibrary.DTOs.Company;
using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectsLibrary.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<ProjectAddDto, Project>().ReverseMap();
            CreateMap<ProjectUpdateDto, Project>().ReverseMap();
            CreateMap<ProjectShortDto, Project>().ReverseMap();
            CreateMap<Project, ProjectReadDto>()
                .ForMember(dest => dest.Employees, opt => opt.MapFrom(src => src.Employees))
                .ForMember(dest => dest.ProjectManager, opt => opt.MapFrom(src => src.ProjectManager))
                .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Company))
                .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => src.Tasks));
            CreateMap<ProjectReadDto, ProjectUpdateDto>().ReverseMap();
            CreateMap<Project, ProjectTasksInfoDto>();

            CreateMap<EmployeeAddDto, Employee>().ReverseMap();
            CreateMap<EmployeeUpdateDto, Employee>().ReverseMap();
            CreateMap<EmployeeShortDto, Employee>().ReverseMap();
            CreateMap<Employee, EmployeeReadDto>()
                .ForMember(dest => dest.CreatedTasks, opt => opt.MapFrom(src => src.CreatedTasks))
                .ForMember(dest => dest.ExecutingTasks, opt => opt.MapFrom(src => src.ExecutingTasks))
                .ForMember(dest => dest.WorkingProjects, opt => opt.MapFrom(src => src.WorkingProjects))
                .ForMember(dest => dest.ManagedProjects, opt => opt.MapFrom(src => src.ManagedProjects));
            CreateMap<EmployeeReadDto, EmployeeUpdateDto>().ReverseMap();

            CreateMap<TaskAddDto, TaskPL>().ReverseMap();
            CreateMap<TaskUpdateDto, TaskPL>().ReverseMap();
            CreateMap<TaskShortDto, TaskPL>().ReverseMap();
            CreateMap<TaskPL, TaskReadDto>()
                .ForMember(dest => dest.Executor, opt => opt.MapFrom(src => src.Executor))
                .ForMember(dest => dest.Project, opt => opt.MapFrom(src => src.Project))
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator));
            CreateMap<TaskReadDto, TaskUpdateDto>().ReverseMap();

            CreateMap<CompanyAddDto, Company>().ReverseMap();
            CreateMap<CompanyUpdateDto, Company>().ReverseMap();
            CreateMap<CompanyShortDto, Company>().ReverseMap();
            CreateMap<Company, CompanyReadDto>();
            CreateMap<CompanyReadDto, CompanyUpdateDto>().ReverseMap();
        }
    }
}
