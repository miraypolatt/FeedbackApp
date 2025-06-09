using System.ComponentModel.DataAnnotations;

namespace FeedbackApp.Application.DTOs
{
    public class FeedbackDto
    {
        [Required]
        [StringLength(30)]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = null!;

    }
}
