using System.ComponentModel.DataAnnotations;

namespace TechMoveLogisticSystem.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Request Description")]
        public string Description { get; set; }

        [Display(Name = "Cost")]
        [Range(0, 1000000, ErrorMessage = "Cost must be a positive value")]
        public decimal Cost { get; set; }

        [Display(Name = "Request Status")]
        public string? Status { get; set; }

        // Foreign Key
        [Display(Name = "Contract")]
        public int ContractId { get; set; }

        // Navigation
        public Contract? Contract { get; set; }
    }
}