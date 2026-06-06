namespace TechMoveLogisticSystem.DTOs
{
    public class ServiceRequestUpdateDto
    {
        public string Description { get; set; } = string.Empty;

        public decimal Cost { get; set; }

        public string Status { get; set; } = string.Empty;

        public int ContractId { get; set; }
    }
}