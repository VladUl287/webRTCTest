using System.Drawing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Files.Extensions;
using Web.Files.Options;

namespace Web.Files;

[Authorize]
[ApiController]
[Route("[controller]/[action]")]
public sealed class ImageController : ControllerBase
{
    private readonly IWebHostEnvironment webHostEnvironment;

    public ImageController(IWebHostEnvironment webHostEnvironment)
    {
        this.webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> UploadImage(IFormFile image, [FromServices] IOptions<ImageOptions> options)
    {
        if (!image.IsValid(options.Value))
        {
            return BadRequest("Некорректное изображение.");
        }
        try
        {
            var basePath = Path.Combine(webHostEnvironment.ContentRootPath, Configuration.StaticDirectory);

            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);

            using var img = Image.FromStream(memoryStream);
            img.Save($"{basePath}/{image.FileName}", img.RawFormat);
        }
        catch
        {
            return BadRequest("Возникла ошибка при добавлении изображения.");
        }

        return Ok();
    }
}
