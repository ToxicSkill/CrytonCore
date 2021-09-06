using AutoMapper;
using CrytonCore.Model;

namespace CrytonCore.Mapper
{
    public class PdfMapper : Profile
    {
        public PdfMapper()
        {
            _ = CreateMap<Model.Image, SimpleImageManager>()
                .ForMember(c => c.Url, opt => opt.MapFrom(src => src.Url))
                .ForMember(c => c.OutputUrl, opt => opt.MapFrom(src => src.OutputUrl))
                .ForMember(c => c.Extension, opt => opt.MapFrom(src => src.Extension))
                .ForMember(c => c.Ratio, opt => opt.MapFrom(src => src.Ratio))
                .ForMember(c => c.Rotation, opt => opt.MapFrom(src => src.Rotation))
                .ForMember(c => c.MaxNumberOfPages, opt => opt.MapFrom(src => src.MaxNumberOfPages))
                .ForMember(c => c.SwitchPixels, opt => opt.MapFrom(src => src.SwitchPixels));
        }
        public PdfMapper(bool SimpleToImage)
        {
            _ = CreateMap<SimpleImageManager, Model.Image>()
                .ForMember(c => c.Url, opt => opt.MapFrom(src => src.Url))
                .ForMember(c => c.OutputUrl, opt => opt.MapFrom(src => src.OutputUrl))
                .ForMember(c => c.Extension, opt => opt.MapFrom(src => src.Extension))
                .ForMember(c => c.Ratio, opt => opt.MapFrom(src => src.Ratio))
                .ForMember(c => c.Rotation, opt => opt.MapFrom(src => src.Rotation))
                .ForMember(c => c.MaxNumberOfPages, opt => opt.MapFrom(src => src.MaxNumberOfPages))
                .ForMember(c => c.SwitchPixels, opt => opt.MapFrom(src => src.SwitchPixels));
        }
    }
}
