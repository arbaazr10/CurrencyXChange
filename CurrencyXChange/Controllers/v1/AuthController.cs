using CurrencyXChange.Authentication;
using CurrencyXChange.Domain.v1.Request;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyXChange.Controllers.v1
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenGenerator _tokenGenerator;

        public AuthController(JwtTokenGenerator tokenGenerator)
        {
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Username == "admin" && request.Password == "password")
            {
                var token = _tokenGenerator.GenerateToken(request.Username, "Admin");
                return Ok(new { token });
            }else if (request.Username == "user" && request.Password == "password")
            {
                var token = _tokenGenerator.GenerateToken(request.Username, "User");
                return Ok(new { token });
            }

            return Unauthorized();
        }
    }
}
