using System.ComponentModel.DataAnnotations;

namespace TechMoveLogisticSystem.Models
{
    public class Client
    {
        // Primary Key
        public int Id { get; set; }

        [Required(ErrorMessage = "Client name is required")]
        [StringLength(100)]
        [Display(Name = "Client Name")] 
        public string Name { get; set; }

        [Display(Name = "Contact Details")]
        public string? ContactDetails { get; set; }

        [Display(Name = "Region")]
        public string? Region { get; set; }

        // navigation property
        // One Client can have many Contracts
        public List<Contract> Contracts { get; set; } = new List<Contract>();
    }
}