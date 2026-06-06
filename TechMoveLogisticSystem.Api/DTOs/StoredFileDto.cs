namespace TechMoveLogisticSystem.Api.DTOs
{
    public class StoredFileDto
    {
        public string FilePath { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = "application/pdf";
    }
}