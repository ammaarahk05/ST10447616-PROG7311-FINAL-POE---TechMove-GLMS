namespace TechMoveLogisticSystem.Api.DTOs
{
    public class ServiceRequestCreateDto
    {
        public string Description { get; set; } = string.Empty;

        public decimal Cost { get; set; }

        public string Status { get; set; } = "Pending";

        public int ContractId { get; set; }
    }
}