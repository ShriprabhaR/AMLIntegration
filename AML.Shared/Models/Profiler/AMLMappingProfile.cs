using AutoMapper;
using AML.Shared.Models;

namespace AML.Shared.Models.Profiler
{
    public class AMLMappingProfile : Profile
    {
        public AMLMappingProfile()
        {
            CreateMap<ScreeningRequest, Customer>();
            //.ForMember(dest => dest.dateOfBirth,
            //    opt => opt.MapFrom(src => DateTime.Parse(src.dateOfBirth).ToString("yyyy-MM-dd")))
            //.ForMember(dest => dest.createdDate,
            //    opt => opt.MapFrom(src => DateTime.Parse(src.createdDate).ToString("yyyy-MM-dd")))
            //.ForMember(dest => dest.modifiedDate,
            //    opt => opt.MapFrom(src => DateTime.Parse(src.modifiedDate).ToString("yyyy-MM-dd")));
        }
    }
}