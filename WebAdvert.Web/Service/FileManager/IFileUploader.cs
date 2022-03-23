using System.IO;
using System.Threading.Tasks;

namespace WebAdvert.Web.Service.FileManager
{
    public interface IFileUploader
    {
        Task<bool> UploadFileAsync(string fileName, Stream storageStream);
    }
}
