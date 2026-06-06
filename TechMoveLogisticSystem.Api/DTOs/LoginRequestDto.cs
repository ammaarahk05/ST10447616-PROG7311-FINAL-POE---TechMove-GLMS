namespace TechMoveLogisticSystem.Api.DTOs
{
    public class LoginRequestDto
    {
        // these login details are sent to request a JWT token
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}