using AutoMapper;

namespace MessageService.Core.AutoMapper
{
    public class ExampleProfile : Profile
    {
        public ExampleProfile()
        {
            // Пример создания автомаппера
            /*
            CreateMap<ModelFoo, ModelBar>()
                .ForPath(m => m.AddrId, opt => opt.Ignore());
            */
        }
    }
}
