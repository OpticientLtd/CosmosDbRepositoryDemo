namespace CosmosDbRepositoryDemo.API.Controllers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using CosmosDbRepositoryDemo.API.Configurations;
using CosmosDbRepositoryDemo.API.Requests;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

[Route("[controller]")]
[ApiController]
public class TokensController : ControllerBase
{
    private readonly AuthenticationConfiguration _authenticationConfigurationSettings;
    public TokensController(IOptions<AuthenticationConfiguration> authenticationConfigurationOptions)
    {
        _authenticationConfigurationSettings = authenticationConfigurationOptions?.Value ?? throw new ArgumentNullException(nameof(authenticationConfigurationOptions));
    }

    /// <summary>
    /// Gives JWT token for given credentials.
    /// </summary>
    /// <param name="request">Email Address and Password</param>
    /// <returns>JWT Token</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> Post(TokenRequest request)
    {
        if (string.IsNullOrEmpty(request?.Email) || string.IsNullOrEmpty(request?.Password))
        {
            return BadRequest();
        }


        if (!isAuthorisedUser(request.Email, request.Password))
        {
            return BadRequest("Invalid credentials");
        }

        //create claims details based on the user information
        var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _authenticationConfigurationSettings.Jwt.Subject),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Email", request.Email)
                    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationConfigurationSettings.Jwt.Key));
        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _authenticationConfigurationSettings.Jwt.Issuer,
            audience: _authenticationConfigurationSettings.Jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: signIn);

        return await Task.FromResult(Ok(new JwtSecurityTokenHandler().WriteToken(token)));
    }

    private bool isAuthorisedUser(string email, string password)
        => email.Equals(_authenticationConfigurationSettings.Jwt.Email, StringComparison.OrdinalIgnoreCase)
            && password.Equals(_authenticationConfigurationSettings.Jwt.Password, StringComparison.InvariantCulture);
}
