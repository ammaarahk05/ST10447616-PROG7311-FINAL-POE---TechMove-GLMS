namespace TechMoveLogisticSystem.Api.DTOs
{
    public class ServiceRequestUpdateDto
    {
        // These are the fields the user can update from the MVC frontend
        public string Description { get; set; } = string.Empty;

        public decimal Cost { get; set; }

        public string Status { get; set; } = string.Empty;

        public int ContractId { get; set; }
    }
}