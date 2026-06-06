namespace TechMoveLogisticSystem.DTOs
{
    public class LoginResponseDto
    {
        // The API will return this JWT token after successful login
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}