using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;

    public FileService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folderName)
    {
        // ✅ 1. Validation: null
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty");

        // ✅ 2. Extension
        var extension = Path.GetExtension(file.FileName).ToLower();

        // 🖼️ صور
        var allowedImages = new[] { ".jpg", ".jpeg", ".png", ".webp" };

        // 📄 ملفات
        var allowedDocs = new[] { ".pdf", ".doc", ".docx" };

        // ✅ 3. تحديد النوع حسب الفولدر
        if (folderName == "logos" || folderName == "images")
        {
            if (!allowedImages.Contains(extension))
                throw new Exception("Invalid image format");
        }
        else if (folderName == "licenses" || folderName == "documents")
        {
            if (!allowedDocs.Contains(extension))
                throw new Exception("Invalid document format");
        }
        else
        {
            throw new Exception("Invalid folder type");
        }

        // ✅ 4. تحديد المسار
        var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", folderName);

        // ✅ 5. إنشاء الفولدر لو مش موجود
        if (!Directory.Exists(uploadsPath))
            Directory.CreateDirectory(uploadsPath);

        // ✅ 6. اسم file unique
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsPath, fileName);

        // ✅ 7. حفظ الملف
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // ✅ 8. رجوع URL
        return $"/uploads/{folderName}/{fileName}";
    }
}