using System.ComponentModel.DataAnnotations;

namespace TechMoveLogisticSystem.DTOs
{
    public class ClientCreateDto
    {
        [Required(ErrorMessage = "Client name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact details are required.")]
        public string ContactDetails { get; set; } = string.Empty;

        [Required(ErrorMessage = "Region is required.")]
        public string Region { get; set; } = string.Empty;
    }
}