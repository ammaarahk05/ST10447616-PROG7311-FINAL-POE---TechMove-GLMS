namespace TechMoveLogisticSystem.Api.DTOs
{
    public class ContractCreateDto
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string ServiceLevel { get; set; } = string.Empty;

        public int ClientId { get; set; }

        public string? SignedAgreementFileName { get; set; }
    }
}