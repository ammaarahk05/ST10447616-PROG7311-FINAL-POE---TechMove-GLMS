namespace TechMoveLogisticSystem.DTOs
{
    public class AgreementUploadResultDto
    {
        // after a PDF agreement is uploaded, this is returned by the API 
        public int ContractId { get; set; }

        public string OriginalFileName { get; set; } = string.Empty;

        public string SavedFileName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}