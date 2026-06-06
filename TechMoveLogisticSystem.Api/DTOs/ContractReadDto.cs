namespace TechMoveLogisticSystem.Api.DTOs
{
    public class ContractReadDto
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string ServiceLevel { get; set; } = string.Empty;

        public int ClientId { get; set; }

        public string ClientName { get; set; } = string.Empty;

        public string ClientRegion { get; set; } = string.Empty;

        public string? SignedAgreementFileName { get; set; }

        public int ServiceRequestCount { get; set; }
    }
}