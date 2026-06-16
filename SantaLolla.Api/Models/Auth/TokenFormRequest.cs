using System.ComponentModel.DataAnnotations;

namespace SantaLolla.Api.Models.Auth
{
    public class TokenFormRequest
    {
        [Required]
        public string ClientId { get; set; } = string.Empty;

        [Required]
        public string ClientSecret { get; set; } = string.Empty;
    }
}
