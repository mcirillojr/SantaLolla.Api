using System.ComponentModel.DataAnnotations;

namespace AlterVision.Api.Models.Auth
{
    public class TokenFormRequest
    {
        [Required]
        public string ClientId { get; set; } = string.Empty;

        [Required]
        public string ClientSecret { get; set; } = string.Empty;
    }
}