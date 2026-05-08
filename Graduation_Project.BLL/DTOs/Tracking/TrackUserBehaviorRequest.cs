namespace Graduation_Project.BLL.DTOs.Tracking
{
    public class TrackUserBehaviorRequest
    {
        public string SessionId { get; set; } = null!;

        public string ActionType { get; set; } = null!;

        public int? ProductId { get; set; }

        public int? CategoryId { get; set; }

        public int? BrandId { get; set; }

        public string? SearchQuery { get; set; }

        public string? SourcePage { get; set; }
    }
}