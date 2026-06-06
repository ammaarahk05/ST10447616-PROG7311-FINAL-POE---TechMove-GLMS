namespace TechMoveLogisticSystem.Services
{
    public class ApiSettings
    {
        // This will be the base address of my backend Web API
        public string BaseUrl { get; set; } = string.Empty;

        // These demo credentials are used by the MVC frontend to request a JWT token
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}