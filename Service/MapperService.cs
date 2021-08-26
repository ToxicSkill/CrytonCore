using AutoMapper;

namespace CrytonCore.Mapper
{
    public class MapperService
    {
        // zmiennna konfiguracji mappera
        private static MapperConfiguration MapperConfig = new(cfg => cfg.AddProfile(new FileMapper()));
        private static MapperConfiguration MapperConfigPdfFiles = new(cfg => cfg.AddProfile(new PdfMapper()));
        private static MapperConfiguration MapperConfigSimplePdfFiles = new(cfg => cfg.AddProfile(new PdfMapper(true)));

        // zmienna mappera
        public readonly IMapper Mapper = MapperConfig.CreateMapper();
        public readonly IMapper MapperPdf = MapperConfigPdfFiles.CreateMapper();
        public readonly IMapper MapperSimplePdf = MapperConfigSimplePdfFiles.CreateMapper();
    }
}
