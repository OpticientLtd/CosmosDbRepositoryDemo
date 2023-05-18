
namespace CosmosDbRepositoryDemo.API;

using System.Reflection;
using System.Text;

using CosmosDbRepositoryDemo.API.Configurations;
using CosmosDbRepositoryDemo.API.Extensions;
using CosmosDbRepositoryDemo.API.Handlers;
using CosmosDbRepositoryDemo.Infrastructure;
using CosmosDbRepositoryDemo.Infrastructure.Persistence.Interfaces;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        Configure(app);

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        services.Configure<AuthenticationConfiguration>(configuration.GetSection(AuthenticationConfiguration.SectionKey));
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.AddApiKeyAuthSchemaSecurityDefinitions()
                .AddBasicAuthSchemaSecurityDefinitions()
                .AddJwtAuthSchemaSecurityDefinitions()
                .IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
        });

        var authenticationConfigurationOptions = services.BuildServiceProvider().GetRequiredService<IOptions<AuthenticationConfiguration>>();
        var authenticationConfiguration = authenticationConfigurationOptions.Value;

        services.AddAuthentication()
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationHandler.SchemeName, null)
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationHandler.SchemeName, null)
        .AddJwtBearer("JwtAuthenticationScheme", options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = authenticationConfiguration.Jwt.Audience,
                ValidIssuer = authenticationConfiguration.Jwt.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationConfiguration.Jwt.Key))
            };
        });
        services.AddInfrastructures(configuration);
    }

    private static void Configure(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        EnsureCosmosDbIscreated(app);

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();
    }
    public static void EnsureCosmosDbIscreated(WebApplication webApplication)
    {
        using var scope = webApplication.Services.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<ICosmosDbContainerFactory>();
        factory.EnsureDbSetupAsync().Wait();
    }
}
