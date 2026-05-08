using Graduation_Project.BLL.Common;
using Graduation_Project.BLL.DTOs.Product;
using Graduation_Project.BLL.Services.Interfaces;
using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Models.Enums;
using Graduation_Project.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
namespace Graduation_Project.BLL.Services.Implementations
{
    public class ProductService : IProductService
    {
            private readonly IFileService _fileService;
        private readonly IUnitOfWork _unitOfWork;
            private readonly IProductAIService _productAIService;
          private readonly IAiModerationService  _aiModerationService;


        public ProductService(IUnitOfWork unitOfWork,
    IFileService fileService ,IProductAIService productAIService,IAiModerationService aiModerationService)
{
    _unitOfWork = unitOfWork;
    _fileService = fileService;
    _productAIService = productAIService;
    _aiModerationService = aiModerationService;
}
public async Task<ServiceResult<ProductDto>> CreateProductAsync(int userId, int brandId, CreateProductDto dto)
{
    try
    {
        // ================= VALIDATE INPUT =================
        if (dto == null)
            return ServiceResult<ProductDto>.Failure("Invalid request");

        if (string.IsNullOrWhiteSpace(dto.ProductName))
            return ServiceResult<ProductDto>.Failure("Product name is required");

        if (string.IsNullOrWhiteSpace(dto.Description))
            return ServiceResult<ProductDto>.Failure("Description is required");

        // ================= BRAND =================
        var brand = await _unitOfWork.Brands
            .GetQueryable()
            .FirstOrDefaultAsync(b => b.BrandId == brandId && b.UserId == userId);

        if (brand == null)
            return ServiceResult<ProductDto>.Failure("Brand not found or not owned by user");

        if (!brand.IsActive)
            return ServiceResult<ProductDto>.Failure("Brand is not active");

        // ================= USER =================
        var user = await _unitOfWork.ApplicationUsers.GetByIdAsync(userId);
        if (user == null)
            return ServiceResult<ProductDto>.Failure("User not found");

        if (!user.HasAcceptedContract)
            return ServiceResult<ProductDto>.Failure("You must accept the contract first");

        // ================= CATEGORY =================
        var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
        if (category == null)
            return ServiceResult<ProductDto>.Failure("Invalid category");

        // ================= VARIANTS =================
        List<ProductVariantDto>? variants = null;
        try
        {
            if (!string.IsNullOrWhiteSpace(dto.VariantsJson))
            {
                variants = JsonSerializer.Deserialize<List<ProductVariantDto>>(
                    dto.VariantsJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
        }
        catch
        {
            return ServiceResult<ProductDto>.Failure("Invalid variants format");
        }

        bool hasVariants = variants != null && variants.Any();

        // ================= PRICE + STOCK VALIDATION =================
        if (!hasVariants)
        {
            if (dto.BasePrice is null || dto.BasePrice <= 0)
                return ServiceResult<ProductDto>.Failure("Base price must be greater than 0");

            if (dto.StockQuantity is null || dto.StockQuantity <= 0)
                return ServiceResult<ProductDto>.Failure("Stock must be greater than 0");
        }
        else
        {
            foreach (var v in variants!)
            {
                if (v.Price <= 0)
                    return ServiceResult<ProductDto>.Failure("Variant price must be greater than 0");

                if (v.StockQuantity <= 0)
                    return ServiceResult<ProductDto>.Failure("Variant stock must be greater than 0");

                if (string.IsNullOrWhiteSpace(v.Size))
                    return ServiceResult<ProductDto>.Failure("Variant size is required");

                if (string.IsNullOrWhiteSpace(v.Color))
                    return ServiceResult<ProductDto>.Failure("Variant color is required");
            }
        }

        if (hasVariants && dto.StockQuantity != null)
            return ServiceResult<ProductDto>.Failure("Do not send stock when using variants");

        // ================= BASE PRICE =================
        decimal basePrice = hasVariants
            ? variants!.Min(v => v.Price)
            : dto.BasePrice!.Value;

        // ✅ السعر النهائي:
        // لو الأونر اختار AI suggestion وبعت قيمة valid → استخدمها
        // لو لا → استخدم السعر اللي هو حطه
        decimal finalPrice;

        if (dto.UseAiSuggestion
            && dto.AiSuggestedPrice.HasValue
            && dto.AiSuggestedPrice.Value > 0)
        {
            finalPrice = dto.AiSuggestedPrice.Value;
        }
        else
        {
            finalPrice = basePrice;
        }

        // ================= IMAGE UPLOAD =================
        // ✅ الـ Image Validation بالفعل اتعملت Real-time من الفرونت
        // هنا بس بنعمل Upload للصور
        List<string> uploadedUrls = new();

        if (dto.Images == null || !dto.Images.Any())
            return ServiceResult<ProductDto>.Failure("At least one image is required");

        foreach (var image in dto.Images)
        {
            var url = await _fileService.UploadFileAsync(image, "images");
            uploadedUrls.Add(url);
        }

        // ================= FINAL MODERATION (نص فقط) =================
        // ✅ الـ Text Moderation بتتعمل هنا عند الـ Submit النهائي
        var aiRequest = new AiModerationRequestDto
        {
            ProductName = dto.ProductName,
            Description = dto.Description,
            Category    = category.CategoryName,
            Price       = finalPrice,
            Images      = uploadedUrls
        };

        var aiResult = await _aiModerationService.ModerateProductAsync(aiRequest);

        var approvalStatus = aiResult.Status switch
        {
            "auto_approved" => ApprovalStatus.Approved,
            "rejected"      => ApprovalStatus.Rejected,
            _               => ApprovalStatus.Pending
        };

        // ================= PRODUCT =================
        var product = new Product
        {
            BrandId    = brandId,
            CategoryId = dto.CategoryId,
            ProductName = dto.ProductName,
            Description = dto.Description,
            ImageUrls   = string.Join(",", uploadedUrls),

            BasePrice     = finalPrice,
            StockQuantity = hasVariants ? 0 : dto.StockQuantity!.Value,

            AllowsCustomization = dto.Customization != null &&
                (dto.Customization.AllowsPrinting || dto.Customization.AllowsText),
            AllowsPrinting = dto.Customization?.AllowsPrinting ?? false,
            AllowsText     = dto.Customization?.AllowsText     ?? false,

            ApprovalStatus = approvalStatus,
            IsActive       = approvalStatus == ApprovalStatus.Approved,
            ApprovalDate   = approvalStatus == ApprovalStatus.Approved ? DateTime.UtcNow : null,
            RejectionReason = approvalStatus == ApprovalStatus.Rejected ? aiResult.Reason : null,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveAsync();

        // ================= VARIANTS =================
        if (hasVariants)
        {
            foreach (var v in variants!)
            {
                await _unitOfWork.ProductVariants.AddAsync(new ProductVariant
                {
                    ProductId     = product.ProductId,
                    Size          = v.Size,
                    Color         = v.Color,
                    Price         = v.Price,
                    StockQuantity = v.StockQuantity,
                    SKU = string.IsNullOrWhiteSpace(v.SKU)
                        ? $"{product.ProductName}-{Guid.NewGuid()}"
                        : v.SKU
                });
            }
            await _unitOfWork.SaveAsync();
        }

        // ================= CUSTOMIZATION ZONES =================
        if (product.AllowsCustomization && dto.Customization?.Zones != null)
        {
            foreach (var zone in dto.Customization.Zones)
            {
                await _unitOfWork.ProductCustomizationZones.AddAsync(new ProductCustomizationZone
                {
                    ProductId   = product.ProductId,
                    Zone        = (CustomizationZone)zone,
                    IsAvailable = true
                });
            }
            await _unitOfWork.SaveAsync();
        }

        // ================= FETCH + RETURN =================
        var productDto = await GetProductByIdAsync(product.ProductId);

        if (!productDto.Succeeded || productDto.Data == null)
            return ServiceResult<ProductDto>.Failure("Product created but failed to fetch data");

        var message = approvalStatus switch
        {
            ApprovalStatus.Approved => "Product created and approved automatically",
            ApprovalStatus.Rejected => $"Product rejected: {aiResult.Reason}",
            _                       => "Product created and pending manual approval"
        };

        return ServiceResult<ProductDto>.Success(productDto.Data, message);
    }
    catch (Exception ex)
    {
        return ServiceResult<ProductDto>.Failure($"Unexpected error: {ex.Message}");
    }
}

// يجيب Product بالـ ID
        public async Task<ServiceResult<ProductDto>> GetProductByIdAsync(int productId)
        {
            try
            {
                var product = await _unitOfWork.Products
                    .GetQueryable()
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.Variants)
                    .Include(p => p.CustomizationZones)
                    .FirstOrDefaultAsync(p => p.ProductId == productId);

                if (product == null)
                    return ServiceResult<ProductDto>.Failure("Product not found");

                return ServiceResult<ProductDto>.Success(MapToDto(product));
            }
            catch (Exception ex)
            {
                return ServiceResult<ProductDto>.Failure($"Error fetching product: {ex.Message}");
            }
        }

        // كل الـ Products الـ Approved - للكاستمر
        public async Task<ServiceResult<List<ProductDto>>> GetAllApprovedProductsAsync()
        {
            try
            {
                var products = await _unitOfWork.Products
                    .GetQueryable()
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.Variants)
                    .Include(p => p.CustomizationZones)
                    .Where(p => p.ApprovalStatus == ApprovalStatus.Approved && p.IsActive)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                return ServiceResult<List<ProductDto>>.Success(products.Select(MapToDto).ToList());
            }
            catch (Exception ex)
            {
                return ServiceResult<List<ProductDto>>.Failure($"Error fetching products: {ex.Message}");
            }
        }

        // Products بتاعت Brand معين
        public async Task<ServiceResult<List<ProductDto>>> GetProductsByBrandAsync(int brandId)
        {
            try
            {
                var products = await _unitOfWork.Products
                    .GetQueryable()
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.Variants)
                    .Include(p => p.CustomizationZones)
                    .Where(p => p.BrandId == brandId && p.ApprovalStatus == ApprovalStatus.Approved && p.IsActive)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                return ServiceResult<List<ProductDto>>.Success(products.Select(MapToDto).ToList());
            }
            catch (Exception ex)
            {
                return ServiceResult<List<ProductDto>>.Failure($"Error fetching products: {ex.Message}");
            }
        }

        // Owner يشوف Products بتاعته كلها
        public async Task<ServiceResult<List<ProductDto>>> GetOwnerProductsAsync(int userId)
        {
            try
            {
                var products = await _unitOfWork.Products
                    .GetQueryable()
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.Variants)
                    .Include(p => p.CustomizationZones)
                    .Where(p => p.Brand.UserId == userId)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                return ServiceResult<List<ProductDto>>.Success(products.Select(MapToDto).ToList());
            }
            catch (Exception ex)
            {
                return ServiceResult<List<ProductDto>>.Failure($"Error fetching products: {ex.Message}");
            }
        }

        // Admin يشوف الـ Pending Products
        public async Task<ServiceResult<List<ProductDto>>> GetPendingProductsAsync()
        {
            try
            {
                var products = await _unitOfWork.Products
                    .GetQueryable()
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.Variants)
                    .Where(p => p.ApprovalStatus == ApprovalStatus.Pending)
                    .OrderBy(p => p.CreatedAt)
                    .ToListAsync();

                return ServiceResult<List<ProductDto>>.Success(products.Select(MapToDto).ToList());
            }
            catch (Exception ex)
            {
                return ServiceResult<List<ProductDto>>.Failure($"Error fetching pending products: {ex.Message}");
            }
        }public async Task<ServiceResult<ProductDto>> UpdateProductAsync(int productId, int userId, UpdateProductDto dto)
{
    try
    {
        var product = await _unitOfWork.Products
            .GetQueryable()
            .Include(p => p.Brand)
            .Include(p => p.Variants)
            .Include(p => p.CustomizationZones)
            .FirstOrDefaultAsync(p => p.ProductId == productId);

        if (product == null)
            return ServiceResult<ProductDto>.Failure("Product not found");

        if (product.Brand.UserId != userId)
            return ServiceResult<ProductDto>.Failure("Unauthorized");

        bool hasVariants = dto.Variants != null && dto.Variants.Any();

        // ================= VALIDATION =================
        if (!hasVariants)
        {
            if (dto.BasePrice <= 0)
                return ServiceResult<ProductDto>.Failure("Base price is required");

            if (dto.StockQuantity == null || dto.StockQuantity <= 0)
                return ServiceResult<ProductDto>.Failure("Stock is required");
        }

        if (hasVariants && dto.StockQuantity != null)
            return ServiceResult<ProductDto>.Failure("Do not send stock with variants");

        var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId);
        if (category == null)
            return ServiceResult<ProductDto>.Failure("Invalid category");

        // ================= IMAGES =================
        List<string> uploadedUrls = new();

        if (dto.Images != null && dto.Images.Any())
        {
            foreach (var img in dto.Images)
            {
                // ✅ validate image زي create
                var validation = await _productAIService.ValidateImageAsync(img);

                if (!validation.IsValid)
                    return ServiceResult<ProductDto>.Failure(validation.Message ?? "Image rejected");

                var url = await _fileService.UploadFileAsync(img, "products");
                uploadedUrls.Add(url);
            }
        }
        else
        {
            // احتفظ بالقديم
            uploadedUrls = product.ImageUrls?.Split(',').ToList() ?? new List<string>();
        }

        var imageUrl = uploadedUrls.FirstOrDefault();

        // ================= BASE PRICE =================
        decimal basePrice = hasVariants
            ? dto.Variants.Min(v => v.Price)
            : dto.BasePrice;

        // ================= AI MODERATION =================
        var aiResult = await _productAIService.ModerateTextAsync(
            dto.ProductName,
            dto.Description,
            category.CategoryName,
            basePrice
        );

        if (aiResult.Status == "rejected")
            return ServiceResult<ProductDto>.Failure(aiResult.Reason ?? "Rejected by AI");

        var approvalStatus = aiResult.Status == "auto_approved"
            ? ApprovalStatus.Approved
            : ApprovalStatus.Pending;

        // ================= UPDATE =================
        product.ProductName = dto.ProductName;
        product.Description = dto.Description;
        product.CategoryId = dto.CategoryId;
        product.ImageUrls = string.Join(",", uploadedUrls);

        product.BasePrice = basePrice;
        product.StockQuantity = hasVariants ? 0 : dto.StockQuantity!.Value;

        product.UpdatedAt = DateTime.UtcNow;

        product.ApprovalStatus = approvalStatus;
        product.IsActive = approvalStatus == ApprovalStatus.Approved;
        product.RejectionReason = aiResult.Status == "rejected" ? aiResult.Reason : null;

        // ================= VARIANTS =================
        var oldVariants = await _unitOfWork.ProductVariants.FindAsync(v => v.ProductId == productId);
        foreach (var v in oldVariants)
            _unitOfWork.ProductVariants.Delete(v);

        if (hasVariants)
        {
            foreach (var v in dto.Variants)
            {
                await _unitOfWork.ProductVariants.AddAsync(new ProductVariant
                {
                    ProductId = productId,
                    Size = v.Size,
                    Color = v.Color,
                    Price = v.Price,
                    StockQuantity = v.StockQuantity,
                    SKU = v.SKU ?? $"{productId}-{Guid.NewGuid()}"
                });
            }
        }

        await _unitOfWork.SaveAsync();

        var result = await GetProductByIdAsync(productId);

        return ServiceResult<ProductDto>.Success(
            result.Data,
            approvalStatus == ApprovalStatus.Approved
                ? "Updated successfully"
                : "Updated and pending approval"
        );
    }
    catch (Exception ex)
    {
        return ServiceResult<ProductDto>.Failure(ex.Message);
    }
}
        // Owner يحذف Product بتاعته
        public async Task<ServiceResult<bool>> DeleteProductAsync(int productId, int userId)
        {
            try
            {
                var product = await _unitOfWork.Products
                    .GetQueryable()
                    .Include(p => p.Brand)
                    .FirstOrDefaultAsync(p => p.ProductId == productId);

                if (product == null)
                    return ServiceResult<bool>.Failure("Product not found");

                if (product.Brand.UserId != userId)
                    return ServiceResult<bool>.Failure("You can only delete your own products");

                _unitOfWork.Products.Delete(product);
                await _unitOfWork.SaveAsync();

                return ServiceResult<bool>.Success(true, "Product deleted successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Failure($"Error deleting product: {ex.Message}");
            }
        }

        // AI يوافق أو يرفض
        public async Task<ServiceResult<ProductDto>> ApproveProductAsync(int productId, string aiResponse)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null)
                    return ServiceResult<ProductDto>.Failure("Product not found");

                if (aiResponse == "approved")
                {
                    product.ApprovalStatus = ApprovalStatus.Approved;
                    product.ApprovalDate = DateTime.UtcNow;
                    product.IsActive = true;
                    product.RejectionReason = null;
                }
                else
                {
                    product.ApprovalStatus = ApprovalStatus.Rejected;
                    product.RejectionReason = aiResponse;
                    product.IsActive = false;
                }

                product.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Products.Update(product);
                await _unitOfWork.SaveAsync();

                return await GetProductByIdAsync(productId);
            }
            catch (Exception ex)
            {
                return ServiceResult<ProductDto>.Failure($"Error approving product: {ex.Message}");
            }
        }

        // يجيب كل تقنيات الطباعة
        public async Task<ServiceResult<List<PrintingTechniqueDto>>> GetPrintingTechniquesAsync()
        {
            try
            {
                var techniques = await _unitOfWork.PrintingTechniques
                    .GetQueryable()
                    .Where(t => t.IsActive)
                    .ToListAsync();

                var result = techniques.Select(t => new PrintingTechniqueDto
                {
                    TechniqueId = t.TechniqueId,
                    Name = t.Name,
                    Dimensions = t.Dimensions,
                    Price = t.Price
                }).ToList();

                return ServiceResult<List<PrintingTechniqueDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<PrintingTechniqueDto>>.Failure($"Error fetching techniques: {ex.Message}");
            }
        }

     private ProductDto MapToDto(Product product) => new ProductDto
{
    ProductId = product.ProductId,
OwnerId = product.Brand.UserId,
    BrandId = product.BrandId,
    BrandName = product.Brand?.BrandName,
    CategoryId = product.CategoryId,
    CategoryName = product.Category?.CategoryName,
    ProductName = product.ProductName,
    Description = product.Description,
    ImageUrls = product.ImageUrls,

    AllowsCustomization = product.AllowsCustomization,

    CustomizationOptions = product.AllowsCustomization ? new ProductCustomizationOptionsDto
    {
        AllowsPrinting = product.AllowsPrinting,
        AllowsText = product.AllowsText,
        AvailableZones = product.CustomizationZones?
            .Where(z => z.IsAvailable)
            .Select(z => ((CustomizationZone)z.Zone).ToString())
            .ToList() ?? new()
    } : null,

    ApprovalStatus = product.ApprovalStatus,
    ApprovalStatusText = product.ApprovalStatus.ToString(),
    RejectionReason = product.RejectionReason,
    CreatedAt = product.CreatedAt,
    IsActive = product.IsActive,

    AverageRating = product.AverageRating,
    ReviewCount = product.ReviewCount,

    BasePrice = product.BasePrice,
 AiSuggestedPrice = product.AiSuggestedPrice,
MinPrice = product.MinPrice,
MaxPrice = product.MaxPrice,
PriceReasoning = product.PriceReasoning,
    Variants = product.Variants?
        .Select(v => new ProductVariantDto
        {
            VariantId = v.VariantId,
            Size = v.Size,
            Color = v.Color,
            Price = v.Price,
            StockQuantity = v.StockQuantity,
            SKU = v.SKU
        }).ToList() ?? new()

        
};

}}