using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lead2Buy.API.Models
{
    public class NetworkContact
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } 

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(150)]
        public string? Email { get; set; }

        [Url]
        public string? FacebookUrl { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? WhatsAppNumber { get; set; }

        [Url]
        public string? YouTubeChannelUrl { get; set; }

        [Url]
        public string? LinkedInUrl { get; set; }

        [Url]
        public string? InstagramUrl { get; set; }

        [Url]
        public string? OtherSocialUrl { get; set; }

        public string? Notes { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}