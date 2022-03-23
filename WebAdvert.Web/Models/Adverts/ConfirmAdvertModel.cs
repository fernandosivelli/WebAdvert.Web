using WebAdvert.Api.Domain.Enums;

namespace WebAdvert.Web.Models.Adverts
{
    public class ConfirmAdvertModel
    {
        public string Id { get; set; }
        public AdvertStatusEnum Status { get; set; }
    }
}
