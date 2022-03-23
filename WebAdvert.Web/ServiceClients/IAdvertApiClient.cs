using System.Threading.Tasks;
using WebAdvert.Web.Models.Adverts;

namespace WebAdvert.Web.ServiceClients
{
    public interface IAdvertApiClient
    {
        Task<string> CreateAsync(CreateAdvertModel model);
        Task<bool> ConfirmAsync(ConfirmAdvertModel model);
    }
}
