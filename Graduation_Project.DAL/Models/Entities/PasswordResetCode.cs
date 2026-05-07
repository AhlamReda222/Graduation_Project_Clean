namespace Graduation_Project.DAL.Models.Entities
{
    public class PasswordResetCode
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Code { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;

        // Navigation
        public virtual ApplicationUser User { get; set; }
    }
}