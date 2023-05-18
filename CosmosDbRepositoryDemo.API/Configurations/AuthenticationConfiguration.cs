namespace CosmosDbRepositoryDemo.API.Configurations;

public class AuthenticationConfiguration
{
    public const string SectionKey = "AuthenticationSettings";
    public BasicSettings Basic { get; set; }
    public APISettings API { get; set; }
    public JwtSettings Jwt { get; set; }
}
public class BasicSettings
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class JwtSettings
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Subject { get; set; }
}
public class APISettings
{
    public string Key { get; set; }
}