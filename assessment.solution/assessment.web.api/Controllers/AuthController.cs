using assessment.web.api.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace assessment.web.api.Controllers
{
    [Route("[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private IConfiguration _config;
    public AuthController(IConfiguration config)
    {
      _config = config;
    }
    [HttpPost]
    public IActionResult Post([FromBody] AuthRequest loginRequest)
    {
      //your logic for login process
      //If login usrename and password are correct then proceed to generate token

      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

      var Sectoken = new JwtSecurityToken(_config["Jwt:Issuer"],
        _config["Jwt:Issuer"],
        [new System.Security.Claims.Claim("username", loginRequest.username)],
        expires: DateTime.Now.AddMinutes(15),
        signingCredentials: credentials);

      var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

      return Ok(token);
    }
  }
}
