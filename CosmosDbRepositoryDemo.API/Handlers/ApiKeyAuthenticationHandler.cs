namespace CosmosDbRepositoryDemo.API.Handlers;

using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using CosmosDbRepositoryDemo.API.Configurations;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "ApiKeyAuthenticationScheme";
    public const string API_KEY_HEADER_NAME = "X-API-Key";
    private readonly AuthenticationConfiguration _authenticationConfigurationSettings;
    public ApiKeyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder,
        ISystemClock clock, IOptions<AuthenticationConfiguration> authenticationConfigurationOptions) : base(options, logger, encoder, clock)
    {
        _authenticationConfigurationSettings = authenticationConfigurationOptions?.Value ?? throw new ArgumentNullException(nameof(authenticationConfigurationOptions));
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(API_KEY_HEADER_NAME))
        {
            return AuthenticateResult.Fail("Missing Authorization Header");
        }

        if (!Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var providedApiKey))
        {
            await Task.FromResult(AuthenticateResult.Fail("API Key is not provided."));
        }

        if (!providedApiKey.Equals(_authenticationConfigurationSettings.API.Key))
        {
            await Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
        }

        var claims = new Claim[] { new Claim(ClaimTypes.Name, "API User") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}
