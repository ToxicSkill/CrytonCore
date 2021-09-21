using AutoMapper;

namespace CrytonCore.Mapper
{
    public class MapperService
    {
        private static readonly MapperConfiguration MapperConfig = new(cfg => cfg.AddProfile(new FileMapper()));

        public readonly IMapper Mapper = MapperConfig.CreateMapper();
    }
}
