
﻿namespace CryptoStore.Infrastructure.Extensions
{
    using System.Text;
    using System.Linq;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.OpenApi.Models;

    using CryptoStore.Data;
    using CryptoStore.Data.Models;
    using CryptoStore.Infrastructure.Filters;
    using CryptoStore.Services;
    using CryptoStore.Services.EmailService;
    using CryptoStore.Services.ServicesType;

    using static Infrastructure.WebConstants;

    public static class ServiceCollectionExtensions
    {
        public static AppSettings GetApplicationSettings(
          this IServiceCollection services,
          IConfiguration configuration)
        {
            var applicationSettingsConfiguration = configuration.GetSection("ApplicationSettings");
            services.Configure<AppSettings>(applicationSettingsConfiguration);
            return applicationSettingsConfiguration.Get<AppSettings>();
        }

        public static IServiceCollection AddDatabase(
           this IServiceCollection services)
           => services
               .AddDbContext<CryptoStoreDb>(options => options
                   .UseSqlServer(ConfigurationData.ConnectionString));

        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services
                .AddIdentity<User, IdentityRole>(options =>
                {
                    options.Password.RequiredLength = 4;
                    options.Password.RequiredUniqueChars = 1; 
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;

                    options.SignIn.RequireConfirmedAccount = true;
                })
                .AddDefaultUI()
                .AddEntityFrameworkStores<CryptoStoreDb>()
                .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddAuthorizations(this IServiceCollection services)
            => services
                .AddAuthorization(options =>
                {
                   options.AddPolicy(ReadPolicy,
                       builder => builder.RequireRole(Admin));
                   options.AddPolicy(WritePolicy,
                       builder => builder.RequireRole(Admin));
                });

        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            var serviceInterfaceType = typeof(IService);
            var singletonServiceInterfaceType = typeof(ISingletonService);
            var scopedServiceInterfaceType = typeof(IScopedService);

            var types = serviceInterfaceType
                .Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .Select(t => new
                {
                    Service = t.GetInterface($"I{t.Name}"),
                    Implementation = t,
                })
                .Where(t => t.Service != null);

            foreach (var type in types)
            {
                if (serviceInterfaceType.IsAssignableFrom(type.Service))
                {
                    services.AddTransient(type.Service, type.Implementation);
                }
                else if (scopedServiceInterfaceType.IsAssignableFrom(type.Service))
                {
                    services.AddScoped(type.Service, type.Implementation);
                }
                else if (singletonServiceInterfaceType.IsAssignableFrom(type.Service))
                {
                    services.AddSingleton(type.Service, type.Implementation);
                }
            }

            return services;
        }

        public static SendGridOptions AddEmailSender(
            this IServiceCollection services,
           IConfiguration configuration)
        {
            services.AddSingleton<IEmailSender, SendGridEmailService>();

            var emailSettingsConfiguration = configuration.GetSection("EmailSettings"); 
            services.Configure<SendGridOptions>(emailSettingsConfiguration);
            return emailSettingsConfiguration.Get<SendGridOptions>();
        } 

        public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        AppSettings appSettings)
        {
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
          => services.AddSwaggerGen(c =>
          {
              c.SwaggerDoc(
                  "v1",
                  new OpenApiInfo
                  {
                      Title = "CryptoStore API",
                      Version = "v1"
                  });
          });

        public static void AddApiControllers(this IServiceCollection services)
            => services
                .AddControllers(options => options
                    .Filters
                    .Add<ModelOrNotFoundActionFilter>());
    }
}