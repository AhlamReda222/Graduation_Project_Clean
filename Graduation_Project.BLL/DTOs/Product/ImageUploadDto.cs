using Microsoft.AspNetCore.Http;

public class ImageUploadDto
{
    public IFormFile Image { get; set; }
}