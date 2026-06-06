namespace TechMoveLogisticSystem.DTOs
{
    public class ClientUpdateDto
    {
        // Used by the MVC edit page when updating a client through the API
        public string Name { get; set; } = string.Empty;

        public string ContactDetails { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;
    }
}