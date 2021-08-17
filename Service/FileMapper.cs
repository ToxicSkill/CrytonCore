using AutoMapper;
using CrytonCore.File;

namespace CrytonCore.Mapper
{
    public class FileMapper : Profile
    {
        public FileMapper()
        {
            CreateMap<CrytonFile, SimpleFile>()
                    .ForMember(c => c.Name, opt => opt.MapFrom(src => src.Name))
                    .ForMember(c => c.Exist, opt => opt.MapFrom(src => src.Exist))
                    .ForMember(c => c.Extension, opt => opt.MapFrom(src => src.Extension))
                    .ForMember(c => c.Method, opt => opt.MapFrom(src => src.Method))
                    .ForMember(c => c.MethodId, opt => opt.MapFrom(src => src.MethodId))
                    .ForMember(c => c.ChunkSize, opt => opt.MapFrom(src => src.ChunkSize))
                    .ForMember(c => c.Status, opt => opt.MapFrom(src => src.Status))
                    .ForMember(c => c.SizeString, opt => opt.MapFrom(src => src.SizeString))
                    .ForMember(c => c.ClipboardString, opt => opt.MapFrom(src => src.ClipboardString));
        }
    }
}