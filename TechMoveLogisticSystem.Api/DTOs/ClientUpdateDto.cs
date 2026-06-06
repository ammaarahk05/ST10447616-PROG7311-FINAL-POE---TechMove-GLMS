namespace TechMoveLogisticSystem.Api.DTOs
{
    public class ClientUpdateDto
    {
        // Used when editing an existing client
        public string Name { get; set; } = string.Empty;

        public string ContactDetails { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;
    }
}