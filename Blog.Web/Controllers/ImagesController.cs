using System.Net;
using Blog.Web.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var imageUrl = await imageRepository.UploadAsync(file);

            if(imageUrl == null)
            {
                return Problem("Image upload failed", null, (int)HttpStatusCode.InternalServerError);
            }

            return new JsonResult(new { link = imageUrl });
        }
    }
}
