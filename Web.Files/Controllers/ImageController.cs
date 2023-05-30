using Web.Files.Options;
using Web.Files.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Web.Files.Controllers;

// [Authorize]
[ApiController]
[Route("[controller]/[action]")]
public sealed class ImageController : ControllerBase
{
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly ImageOptions options;

    public ImageController(IWebHostEnvironment webHostEnvironment, IOptions<ImageOptions> options)
    {
        this.webHostEnvironment = webHostEnvironment;
        this.options = options.Value;
    }

    public async Task<IActionResult> Upload(IFormFile image)
    {
        if (!image.IsValid(options))
        {
            return BadRequest("Некорректное изображение.");
        }

        try
        {
            var imagesBasePath = Path.Combine(webHostEnvironment.ContentRootPath, ImagesConfiguration.Directory);

            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            using var img = await Image.LoadAsync(memoryStream);
            await img.SaveAsync($"{imagesBasePath}/{image.FileName}");
        }
        catch
        {
            return BadRequest("Возникла ошибка при добавлении изображения.");
        }

        return Ok();
    }
}
