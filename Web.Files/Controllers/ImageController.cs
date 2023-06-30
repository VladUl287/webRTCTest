using Web.Files.Options;
using Web.Files.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Web.Files.Controllers;

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
            var imagesBasePath = webHostEnvironment.WebRootPath;

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

        var pathToImage = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + "/" + image.FileName;

        return Ok(pathToImage);
    }
}