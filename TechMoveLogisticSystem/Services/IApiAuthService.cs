namespace TechMoveLogisticSystem.Services
{
    public interface IApiAuthService
    {
        // this'll give MVC a valid JWT token for protected API calls
        Task<string> GetTokenAsync();
    }
}