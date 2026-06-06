namespace TechMoveLogisticSystem.Api.DTOs
{
    public class ClientReadDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string ContactDetails { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        public int ContractCount { get; set; }
    }
}