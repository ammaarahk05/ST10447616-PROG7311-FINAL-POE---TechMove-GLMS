namespace TechMoveLogisticSystem.Api.DTOs
{
    public class AgreementUploadResultDto
    {
        public int ContractId { get; set; }

        public string OriginalFileName { get; set; } = string.Empty;

        public string SavedFileName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}