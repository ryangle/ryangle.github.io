using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GrpcServer.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public TokenController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// 获取Token
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <returns></returns>
    [HttpGet]
    public string GetToken([FromHeader] string clientId, [FromHeader] string clientSecret)
    {
        if (clientId == "grpc" && clientSecret == "123456")
        {
            string key = _configuration.GetValue<string>("Authentication:SymmetricSecurityKey") ?? "";
            var securityKey = new SymmetricSecurityKey(Guid.Parse(key).ToByteArray());
            var claims = new[] {
                new Claim(ClaimTypes.Name, clientId),
                new Claim(ClaimTypes.NameIdentifier,clientId)
            };
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken("Greeter", "Greeter", claims, expires: DateTime.Now.AddSeconds(60), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        else
        {
            return "非法请求，不能获取token";
        }
    }
}
