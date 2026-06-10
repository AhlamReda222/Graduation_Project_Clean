// FILE: BLL/DTOs/Tracking/TrackUserBehaviorRequest.cs
namespace Graduation_Project.BLL.DTOs.Tracking
{
    public class TrackUserBehaviorRequest
    {
        // ✅ الفرونت مش محتاج يبعت SessionId - الباك بياخده من الـ Cookie
        // بس لو بعته مش هيتجاهله
        public string? SessionId { get; set; }

        public string ActionType { get; set; } = null!;

        public int? ProductId  { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId    { get; set; }

        public string? SearchQuery { get; set; }
        public string? SourcePage  { get; set; }
    }
}