using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechMoveLogisticSystem.Models
{
    public class Contract
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Contract Status")]
        public string Status { get; set; } // Draft / Active / Expired / On Hold

        [Display(Name = "Service Level")]
        public string? ServiceLevel { get; set; }

        // Foreign Key
        [Display(Name = "Client")]
        public int ClientId { get; set; }

        // Navigation
        [ValidateNever] // I added this so MVC doesn't try to validate the full Client object
        public Client Client { get; set; }

        public List<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();

        // File path to use this later for my uploads
        public string? SignedAgreementPath { get; set; }
        // users see this in the ui
        public string? SignedAgreementFileName { get; set; }

        [NotMapped] // it tells the EF not to store this in database
        [Display(Name = "Upload Signed Agreement (PDF)")]
        public IFormFile? SignedAgreementFile { get; set; }
    }
}