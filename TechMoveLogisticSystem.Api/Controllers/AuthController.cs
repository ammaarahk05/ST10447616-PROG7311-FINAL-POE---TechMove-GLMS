using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TechMoveLogisticSystem.Api.DTOs;


namespace TechMoveLogisticSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<LoginResponseDto> Login(LoginRequestDto request)
        {
            //used a simple demo admin login for the assignment API authentication requirement
            if (request.Username != "admin" || request.Password != "Admin@123")
            {
                return Unauthorized("Invalid username or password.");
            }

            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];
            var expiryMinutes = Convert.ToInt32(_configuration["Jwt:ExpiryMinutes"]);

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "JWT key is missing.");
            }

            var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            var tokenText = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new LoginResponseDto
            {
                Token = tokenText,
                ExpiresAt = expiresAt,
                Message = "Login successful."
            });
        }
    }
}