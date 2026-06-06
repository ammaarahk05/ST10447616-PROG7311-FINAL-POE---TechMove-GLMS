namespace TechMoveLogisticSystem.Api.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}