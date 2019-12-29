using System.ComponentModel.DataAnnotations;

namespace BPT_Service.Model.Entities
{
    public class EmailModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }
    }
}