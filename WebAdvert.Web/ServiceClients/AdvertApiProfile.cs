using AutoMapper;
using WebAdvert.Api.Domain.Models;
using WebAdvert.Web.Models.Adverts;
using ConfirmAdvertModel = WebAdvert.Api.Domain.Models.ConfirmAdvertModel;

namespace WebAdvert.Web.ServiceClients
{
    public class AdvertApiProfile : Profile
    {
        public AdvertApiProfile()
        {
            CreateMap<AdvertModel, CreateAdvertModel>().ReverseMap();
            CreateMap<ConfirmAdvertModel, Web.Models.Adverts.ConfirmAdvertModel>().ReverseMap();
        }
    }
}
