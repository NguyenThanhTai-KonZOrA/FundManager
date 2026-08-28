using System.ComponentModel.DataAnnotations;

namespace FundManager.Implement.ViewModels.Request
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}