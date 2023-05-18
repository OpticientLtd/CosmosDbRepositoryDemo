namespace CosmosDbRepositoryDemo.API.Extensions;

using CosmosDbRepositoryDemo.API.Handlers;

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

public static class SwaggerExtensions
{
    public static SwaggerGenOptions AddBasicAuthSchemaSecurityDefinitions(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("basic", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "basic",
            In = ParameterLocation.Header,
            Description = "Basic Authorization header using the Bearer scheme."
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "basic"
                                }
                            },
                            new string[] {}
                    }
                });

        return options;
    }

    public static SwaggerGenOptions AddApiKeyAuthSchemaSecurityDefinitions(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("API Key", new OpenApiSecurityScheme
        {
            Name = ApiKeyAuthenticationHandler.API_KEY_HEADER_NAME,
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Description = "Api key from header",
        });


        options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "API Key"
                                }
                            },
                            new string[] {}
                        }
                    });

        return options;
    }

    public static SwaggerGenOptions AddJwtAuthSchemaSecurityDefinitions(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            In = ParameterLocation.Header,
            Description = "Bearer Token from Header",
            Scheme = "Bearer"
        });


        options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme="oauth2",
                                Name="Bearer",
                                In = ParameterLocation.Header
                            },
                            new string[] {}
                        }
                    });

        return options;
    }
}

