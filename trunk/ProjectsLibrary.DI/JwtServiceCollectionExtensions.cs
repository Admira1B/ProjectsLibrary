using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Models.Options;
using ProjectsLibrary.Domain.Models.Enums;
using ProjectsLibrary.Domain.Exceptions;
using System.Text;

namespace ProjectsLibrary.CompositionRoot
{
    public static class JwtServiceCollectionExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration) 
        {
            var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();

            if (jwtOptions == null || string.IsNullOrWhiteSpace(jwtOptions.SecretKey))
                throw new JwtOptionsNotConfiguratedException(
                    message: "Jwt options not configured or secret key is missing",
                    details: "Cannot add jwt authentication, because jwt options or secret key are missing");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, (options) =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateActor = false,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                        };

                        options.Events = new JwtBearerEvents 
                        {
                            OnMessageReceived = context => 
                            {
                                context.Token = context.Request.Cookies["auth-t"];

                                return Task.CompletedTask;
                            }
                        };
                    });

            return services;
        }

        public static IServiceCollection AddJwtAuthorization(this IServiceCollection services) 
        {
            services.AddSingleton<IAuthorizationHandler, RoleAuthorizationHandler>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyLevelName.AdminPolicy, policy =>
                    policy.Requirements.Add(new RoleAuthorizationRequirement(EmployeeRole.Admin)));

                options.AddPolicy(PolicyLevelName.SupervisorLevel, policy =>
                    policy.Requirements.Add(new RoleAuthorizationRequirement(EmployeeRole.Supervisor)));

                options.AddPolicy(PolicyLevelName.ManagmentLevel, policy =>
                    policy.Requirements.Add(new RoleAuthorizationRequirement(EmployeeRole.Manager)));

                options.AddPolicy(PolicyLevelName.BaseLevel, policy =>
                    policy.Requirements.Add(new RoleAuthorizationRequirement(EmployeeRole.Employee)));
            });

            return services;
        }
    }
}
