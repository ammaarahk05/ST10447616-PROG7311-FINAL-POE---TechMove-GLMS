namespace TechMoveLogisticSystem.DTOs
{
    public class ContractStatusUpdateDto
    {
        // when changing contract status, this is sent to the API PATCH endpoint 
        public string Status { get; set; } = string.Empty;
    }
}