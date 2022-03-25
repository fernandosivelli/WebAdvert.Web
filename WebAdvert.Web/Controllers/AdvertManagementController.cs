using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Runtime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebAdvert.Api.Domain.Enums;
using WebAdvert.Api.Domain.Models;
using WebAdvert.Web.Models;
using WebAdvert.Web.Models.Adverts;
using WebAdvert.Web.Service.FileManager;
using WebAdvert.Web.ServiceClients;
using ConfirmAdvertModel = WebAdvert.Web.Models.Adverts.ConfirmAdvertModel;

namespace WebAdvert.Web.Controllers
{
    public class AdvertManagementController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IAdvertApiClient _advertApiClient;


        public AdvertManagementController(IFileUploader fileUploader, IAdvertApiClient advertApiClient)
        {
            _fileUploader = fileUploader;
            _advertApiClient = advertApiClient;
        }

        public IActionResult Create(CreateAdvertViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdvertViewModel model, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            var advertModel = new CreateAdvertModel()
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price
            };

            var createAdvertResponseJson = await _advertApiClient.CreateAsync(advertModel);

            var createAdvertResponseModel = JsonConvert.DeserializeObject<CreateAdvertResponseModel>(createAdvertResponseJson);

            if (imageFile == null)
                return View(model);

            var fileName = !string.IsNullOrEmpty(imageFile.FileName)
                ? Path.GetFileName(imageFile.FileName)
                : createAdvertResponseModel.Id;

            var filePath = $"{createAdvertResponseModel.Id}/{fileName}";

            try
            {
                var readStream = imageFile.OpenReadStream();
                var result = await _fileUploader.UploadFileAsync(filePath, readStream);

                if (!result)
                    throw new Exception("Could not upload the image to the file repository.");

                var confirmAdvertModel = new ConfirmAdvertModel()
                {
                    Id = createAdvertResponseModel.Id,
                    Status = AdvertStatusEnum.Active
                };

                var canConfirm = await _advertApiClient.ConfirmAsync(confirmAdvertModel);
                if (!canConfirm)
                    throw new Exception($"Cannot confirm advert of id = {createAdvertResponseModel.Id}");

            }
            catch (Exception ex)
            {
                var confirmAdvertModel = new ConfirmAdvertModel()
                {
                    Id = createAdvertResponseModel.Id,
                    Status = AdvertStatusEnum.Pending
                };
                await _advertApiClient.ConfirmAsync(confirmAdvertModel).ConfigureAwait(false);

                Console.WriteLine(ex);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
