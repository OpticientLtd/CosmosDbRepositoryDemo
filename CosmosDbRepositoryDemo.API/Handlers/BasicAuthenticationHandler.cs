namespace CosmosDbRepositoryDemo.API.Handlers;

using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using CosmosDbRepositoryDemo.API.Configurations;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "BasicAuthenticationScheme";
    private readonly AuthenticationConfiguration _authenticationConfigurationSettings;

    public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder,
        ISystemClock clock, IOptions<AuthenticationConfiguration> authenticationConfigurationOptions) : base(options, logger, encoder, clock)
    {
        _authenticationConfigurationSettings = authenticationConfigurationOptions?.Value ?? throw new ArgumentNullException(nameof(authenticationConfigurationOptions));
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.Fail("Missing Authorization Header");
        }

        var header = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
        var credentialBytes = Convert.FromBase64String(header.Parameter);
        var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
        var username = credentials[0];
        var password = credentials[1];

        if (!isAuthorisedUser(username, password))
        {
            return await Task.FromResult(AuthenticateResult.Fail("Invalid UserName or Password"));
        }

        var claims = new Claim[] { new Claim(ClaimTypes.Name, username) };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }

    private bool isAuthorisedUser(string userName, string password)
        => (_authenticationConfigurationSettings?.Basic?.UserName?.Equals(userName, StringComparison.InvariantCultureIgnoreCase)).GetValueOrDefault(false)
            && (_authenticationConfigurationSettings?.Basic.Password.Equals(password, StringComparison.InvariantCulture)).GetValueOrDefault(false);
}
