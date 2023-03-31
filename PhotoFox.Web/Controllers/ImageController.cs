using Microsoft.AspNetCore.Mvc;
using PhotoFox.Storage.Blob;
using PhotoFox.Storage.Table;

namespace PhotoFox.Web.Controllers
{
    public class ImageController : Controller
    {
        private readonly IPhotoFileStorage photoFileStorage;

        public ImageController(IPhotoFileStorage photoFileStorage)
        {
            this.photoFileStorage = photoFileStorage;
        }

        [HttpGet("/image/{id}")]
        public async Task<IActionResult> Image(string id)
        {
            var data = await this.photoFileStorage.GetPhotoAsync(id);

            return new FileStreamResult(data.ToStream(), "image/jpeg");
        }

        [HttpGet("/thumbnail/{id}")]
        public async Task<IActionResult> Thumbnail(string id)
        {
            var data = await this.photoFileStorage.GetThumbnailAsync(id);

            return new FileStreamResult(data.ToStream(), "image/jpeg");
        }
    }
}
