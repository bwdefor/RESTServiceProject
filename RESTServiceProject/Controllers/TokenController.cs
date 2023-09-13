using Microsoft.AspNetCore.Mvc;

namespace RESTServiceProject.Controllers
{
    public class TokenRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    [Route("api/TokenController")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        [HttpPost]
        public dynamic Post([FromBody] TokenRequest tokenRequest)
        {
            var token = TokenHelper.GetToken(tokenRequest.Email, tokenRequest.Password);
            return new { Token = token };
        }

        [HttpGet("{email}/{password}")]
        public dynamic Get(string email, string password)
        {
            var token = TokenHelper.GetToken(email, password);
            return new { Token = token };
        }
    }
}