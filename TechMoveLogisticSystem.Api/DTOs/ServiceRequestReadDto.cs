namespace TechMoveLogisticSystem.Api.DTOs
{
    public class ServiceRequestReadDto
    {
        public int Id { get; set; }

        public string Description { get; set; } = string.Empty;

        public decimal Cost { get; set; }

        public string Status { get; set; } = string.Empty;

        public int ContractId { get; set; }

        public string ContractStatus { get; set; } = string.Empty;

        public string ClientName { get; set; } = string.Empty;
    }
}