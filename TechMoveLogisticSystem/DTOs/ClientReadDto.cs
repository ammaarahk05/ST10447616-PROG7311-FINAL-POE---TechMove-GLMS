namespace TechMoveLogisticSystem.DTOs
{
    public class ClientReadDto
    {
        // This is the client data returned from the backend API
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string ContactDetails { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        public int ContractCount { get; set; }
    }
}