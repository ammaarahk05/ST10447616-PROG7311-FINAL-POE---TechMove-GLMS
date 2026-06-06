namespace TechMoveLogisticSystem.DTOs
{
    public class LoginRequestDto
    {
        // These details are sent from MVC to the API login endpoint
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}