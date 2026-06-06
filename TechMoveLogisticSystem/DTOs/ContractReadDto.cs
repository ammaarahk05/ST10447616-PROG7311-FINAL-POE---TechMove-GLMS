namespace TechMoveLogisticSystem.DTOs
{
    public class ContractReadDto
    {
        // This is the contract data thats returned from the backend API
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string ServiceLevel { get; set; } = string.Empty;

        public int ClientId { get; set; }

        public string ClientName { get; set; } = string.Empty;

        public string ClientRegion { get; set; } = string.Empty;

        public string SignedAgreementFileName { get; set; } = string.Empty;

        public int ServiceRequestCount { get; set; }
    }
}