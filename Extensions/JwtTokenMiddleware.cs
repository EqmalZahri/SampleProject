using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Rbac_IctJohor.common;
using System.IdentityModel.Tokens.Jwt;

namespace Rbac_IctJohor.Extensions
{
    public static class JwtTokenMiddleware
    {
        public static IServiceCollection AddJWTSwaggerAuth(this IServiceCollection services)
        {
            return services.AddSwaggerGen(setup =>
            {
                // Include 'SecurityScheme' to use JWT Authentication
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
                //setup.DescribeAllEnumsAsStrings();
                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
                setup.CustomSchemaIds(type => type.ToString());
            });
        }
        public static AuthenticationBuilder AddJWTAuthentication(this IServiceCollection services, AppSettings appSettings)
        {
            return services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            })
               .AddJwtBearer(options =>
               {
                   options.Audience = appSettings.Authentication.Audience;
                   options.SaveToken = true;
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       SaveSigninToken = true,
                       // The signing key must match!
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.
                           GetBytes(appSettings.Authentication.SecurityKey)),

                       // Validate the JWT Issuer (iss) claim
                       //ValidateIssuer = true,
                       ValidIssuer = appSettings.Authentication.Issuer,

                       // Validate the JWT Audience (aud) claim
                       //ValidateAudience = true,
                       ValidAudience = appSettings.Authentication.Audience,

                       // Validate the token expiry
                       ValidateLifetime = true,

                       // If you want to allow a certain amount of clock drift, set that here
                       ClockSkew = TimeSpan.Zero
                   };
               });
        }
        public static IApplicationBuilder UseJwtTokenMiddleware(this IApplicationBuilder app)
        {
            return app.Use(async (ctx, next) =>
            {
                if (ctx.User.Identity?.IsAuthenticated != true)
                {

                    var result = await ctx.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
                    if (result.Succeeded && result.Principal != null)
                    {
                        ctx.User = result.Principal;
                    }
                }

                await next();
            });
        }
        public static void RegisterTokenConfigToDI(this IServiceCollection services, AppSettings appSettings)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.Authentication.SecurityKey));
            services.AddTransient(c => new TokenAuthConfiguration
            {
                Audience = appSettings.Authentication.Audience,
                Issuer = appSettings.Authentication.Issuer,
                SecurityKey = securityKey,
                Expiration = TimeSpan.FromHours(appSettings.Authentication.AccessTokenLifeTime),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            });
        }
    }
    public class TokenAuthConfiguration
    {
        public SymmetricSecurityKey SecurityKey { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public SigningCredentials SigningCredentials { get; set; }

        public TimeSpan Expiration { get; set; }
    }

}
  