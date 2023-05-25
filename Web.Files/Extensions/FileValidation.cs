using Microsoft.AspNetCore.StaticFiles;
using Web.Files.Options;

namespace Web.Files.Extensions;

internal static class FileValidation
{
    private static readonly char[] InvalidNameChars = Path.GetInvalidFileNameChars();
    private static readonly FileExtensionContentTypeProvider ExtensionContentType = new();

    public static bool IsValid(this IFormFile formFile, FileBaseOptions config)
    {
        if (formFile is null || config is { Extensions: null or { Length: 0 } })
        {
            return false;
        }

        if (formFile.FileName.Intersect(InvalidNameChars).Any())
        {
            return false;
        }

        var extension = Path.GetExtension(formFile.FileName);
        if (!config.Extensions.Any(x => x.Equals(extension, System.StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        if (!ExtensionContentType.TryGetContentType(formFile.FileName, out string? contentType))
        {
            return false;
        }

        if (!formFile.ContentType.Equals(contentType, System.StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (formFile.Length > config.MaxBytes || formFile.Length < config.MinBytes)
        {
            return false;
        }

        try
        {
            if (!formFile.OpenReadStream().CanRead)
            {
                return false;
            }
        }
        catch
        {
            return false;
        }

        return true;
    }
}
