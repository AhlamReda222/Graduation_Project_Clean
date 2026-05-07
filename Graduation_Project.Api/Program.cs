using Graduation_Project.BLL.Services.Implementations;
using Graduation_Project.BLL.Services.Interfaces;
using Graduation_Project.BLL.Seeders;
using Graduation_Project.DAL.DataBase;
using Graduation_Project.DAL.Models.Entities;
using Graduation_Project.DAL.Repositories.Implementations;
using Graduation_Project.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Graduation_Project.Api.Services;
using Graduation_Project.Api.Hubs;
 
 using Graduation_Project.BLL.Services;
using Graduation_Project.BLL.Common;
var builder = WebApplication.CreateBuilder(args);
 
// ================= DATABASE =================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
 
// ================= IDENTITY =================
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
 
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
 
// ================= JWT =================
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme             = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer              = jwt["Issuer"],
        ValidAudience            = jwt["Audience"],
        IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
        ClockSkew                = TimeSpan.Zero
    };
});
 
// ================= AUTHORIZATION =================
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly",      policy => policy.RequireClaim("UserType", "Admin"));
    options.AddPolicy("BrandOwnerOnly", policy => policy.RequireClaim("UserType", "BrandOwner"));
    options.AddPolicy("CustomerOnly",   policy => policy.RequireClaim("UserType", "Customer"));
});
 
// ================= SWAGGER =================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name        = "Authorization",
        Type        = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme      = "bearer",
        BearerFormat = "JWT",
        In          = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });
 
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
 
// ================= HTTP CLIENTS =================
// ✅ الترتيب مهم - HTTP Clients الأول
builder.Services.AddHttpClient<IAiModerationService, AiModerationService>();
builder.Services.AddHttpClient<IProductDescriptionService, ProductDescriptionService>();
 
// ================= SCOPED SERVICES =================
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBrandOwnerRequestService, BrandOwnerRequestService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationHub, NotificationHubService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
 
// ✅ ProductAIService بيعتمد على IAiModerationService (HttpClient)
builder.Services.AddScoped<IProductAIService, ProductAIService>();
 
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IEmailSender, SmartEmailSender>();
// ✅ ProductAIService بيعتمد على IAiModerationService (HttpClient)
builder.Services.AddScoped<IProductAIService, ProductAIService>();
 builder.Services.AddScoped<NotificationHelper>();

// ✅ ProductService بيعتمد على IProductAIService و IAiModerationService
// لازم نسجله بـ factory عشان يأخذ IAiModerationService من الـ HttpClient
builder.Services.AddScoped<IProductService>(sp =>
{
    var unitOfWork          = sp.GetRequiredService<IUnitOfWork>();
    var fileService         = sp.GetRequiredService<IFileService>();
    var productAIService    = sp.GetRequiredService<IProductAIService>();
    var aiModerationService = sp.GetRequiredService<IAiModerationService>();
 
    return new ProductService(unitOfWork, fileService, productAIService, aiModerationService);
});
 
// ================= EMAIL =================
builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddScoped<IInAppEmailService, InAppEmailService>();
 
// ================= SIGNALR =================
builder.Services.AddSignalR();
 
// ================= CORS =================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
 
builder.Services.AddControllers();
 
// ================= BUILD =================
var app = builder.Build();
 
// ================= SEED DATA =================
using (var scope = app.Services.CreateScope())
{
    await AdminSeeder.SeedAdminAsync(scope.ServiceProvider);
    await PrintingTechniqueSeeder.SeedAsync(scope.ServiceProvider);
}
 
// ================= MIDDLEWARE PIPELINE =================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    c.RoutePrefix = "swagger";
});
 
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
 
app.MapHub<NotificationHub>("/hubs/notifications");
app.MapHub<ChatHub>("/hubs/chat");
app.MapControllers();
app.MapGet("/test", () => "API is working");
 
// ================= PORT =================
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");
 
app.Run();