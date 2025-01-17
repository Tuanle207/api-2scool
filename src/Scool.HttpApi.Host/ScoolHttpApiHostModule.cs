using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scool.EntityFrameworkCore;
using Scool.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Scool.Middlewares;
using Volo.Abp.AspNetCore.SignalR;
using Scool.Notification;
using Volo.Abp.MailKit;
using MailKit.Security;
using Volo.Abp.BackgroundJobs;
using Scool.Email;

namespace Scool
{
    [DependsOn(
        typeof(ScoolHttpApiModule),
        typeof(AbpAutofacModule),
        typeof(AbpAspNetCoreMultiTenancyModule),
        typeof(ScoolApplicationModule),
        typeof(ScoolEntityFrameworkCoreDbMigrationsModule),
        typeof(AbpAspNetCoreMvcUiBasicThemeModule),
        typeof(AbpAspNetCoreAuthenticationJwtBearerModule),
        typeof(AbpAccountWebIdentityServerModule),
        typeof(AbpAspNetCoreSerilogModule),
        typeof(AbpSwashbuckleModule),
        typeof(AbpAspNetCoreSignalRModule),
        typeof(AbpBackgroundJobsModule)
    )]
    public class ScoolHttpApiHostModule : AbpModule
    {
        private const string CorsPolicyName = "AllowCORS";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var hostingEnvironment = context.Services.GetHostingEnvironment();

            ConfigureBundles();
            ConfigureUrls(configuration);
            ConfigureConventionalControllers();
            ConfigureAuthentication(context, configuration, hostingEnvironment);
            ConfigureLocalization();
            ConfigureVirtualFileSystem(context);
            ConfigureCors(context, configuration);
            ConfigureSwaggerServices(context, configuration);
            ConfigureAutoAntiForgery();
            ConfigureMailKit();
            ConfigBackgroundJobWorker();
            ConfigureEnvConfiguration(context, configuration);
            ConfigureOtherSettings(context);
        }

        private void ConfigureOtherSettings(ServiceConfigurationContext context)
        {
            context.Services.AddHttpContextAccessor();
            context.Services.AddTransient<NotificationHub>();
            context.Services.AddHttpClient();
        }

        private void ConfigureEnvConfiguration(ServiceConfigurationContext context, IConfiguration configuration)
        {
            var emailOptions = configuration.GetSection("Email");
            context.Services.Configure<EmailOptions>(emailOptions);

        }

        private void ConfigBackgroundJobWorker()
        {
            Configure<AbpBackgroundJobWorkerOptions>(options =>
            {
                options.DefaultTimeout = 10 * 60; // 10 minutes
            });
        }

        private void ConfigureMailKit()
        {
            Configure<AbpMailKitOptions>(options =>
            {
                options.SecureSocketOption = SecureSocketOptions.Auto;
            });

        }


        private void ConfigureBundles()
        {
            Configure<AbpBundlingOptions>(options =>
            {
                //options.StyleBundles.Configure(
                //    BasicThemeBundles.Styles.Global,
                //    bundle => { bundle.AddFiles("/global-styles.css"); }
                //);
            });
        }

        private void ConfigureUrls(IConfiguration configuration)
        {
            Configure<AppUrlOptions>(options =>
            {
                options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
                options.RedirectAllowedUrls.AddRange(configuration["App:RedirectAllowedUrls"].Split(','));

                options.Applications["Angular"].RootUrl = configuration["App:ClientUrl"];
                options.Applications["Angular"].Urls[AccountUrlNames.PasswordReset] = "account/reset-password";
            });
        }

        private void ConfigureVirtualFileSystem(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();

            if (hostingEnvironment.IsDevelopment())
            {
                Configure<AbpVirtualFileSystemOptions>(options =>
                {
                    options.FileSets.ReplaceEmbeddedByPhysical<ScoolDomainSharedModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $"..{Path.DirectorySeparatorChar}Scool.Domain.Shared"));
                    options.FileSets.ReplaceEmbeddedByPhysical<ScoolDomainModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $"..{Path.DirectorySeparatorChar}Scool.Domain"));
                    options.FileSets.ReplaceEmbeddedByPhysical<ScoolApplicationContractsModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $"..{Path.DirectorySeparatorChar}Scool.Application.Contracts"));
                    options.FileSets.ReplaceEmbeddedByPhysical<ScoolApplicationModule>(
                        Path.Combine(hostingEnvironment.ContentRootPath,
                            $"..{Path.DirectorySeparatorChar}Scool.Application"));
                });
            }
        }

        private void ConfigureConventionalControllers()
        {
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(ScoolApplicationModule).Assembly);
            });
        }

        private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration, IWebHostEnvironment env)
        {
            context.Services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["AuthServer:Authority"];
                    options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                    options.Audience = "Scool";
                    if (env.IsDevelopment())
                    {
                        options.TokenValidationParameters.ValidateIssuer = false;
                    } else {
                        // TODO: FIX
                        options.TokenValidationParameters.ValidateIssuer = false;
                    }
                    options.BackchannelHttpHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                });
        }

        private static void ConfigureSwaggerServices(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAbpSwaggerGenWithOAuth(
                configuration["AuthServer:Authority"],
                new Dictionary<string, string>
                {
                    {"Scool", "Scool API"}
                },
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo {Title = "Scool API", Version = "v1"});
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);
                });
        }

        private void ConfigureLocalization()
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Languages.Add(new LanguageInfo("ar", "ar", "العربية"));
                options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
                options.Languages.Add(new LanguageInfo("en", "en", "English"));
                options.Languages.Add(new LanguageInfo("en-GB", "en-GB", "English (UK)"));
                options.Languages.Add(new LanguageInfo("fr", "fr", "Français"));
                options.Languages.Add(new LanguageInfo("hu", "hu", "Magyar"));
                options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
                options.Languages.Add(new LanguageInfo("ru", "ru", "Русский"));
                options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
                options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
                options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
                options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "Deutsch", "de"));
                options.Languages.Add(new LanguageInfo("es", "es", "Español", "es"));
                options.Languages.Add(new LanguageInfo("vi", "vi-VN", "Tiếng Việt", "vn"));

            });
        }

        private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName, builder =>
                {
                    builder
                    .AllowAnyOrigin()
                    // .WithOrigins(
                    //     configuration["App:CorsOrigins"]
                    //         .Split(",")
                    //         .ToArray()
                    // )
                    .SetPreflightMaxAge(new TimeSpan(24, 0, 0))
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });
        }

        private void ConfigureAutoAntiForgery()
        {
            //TODO: Configuring Auto-AnttiForgery properly - Current: Ignore all endpoints.
            Configure<AbpAntiForgeryOptions>(options =>
            {
                options.AutoValidate = false;
                options.TokenCookie.Expiration = TimeSpan.FromDays(1);
                //options.TokenCookie.Expiration = TimeSpan.FromDays(365);
                // options.AutoValidateIgnoredHttpMethods.Remove("GET");
                //options.AutoValidateFilter =
                    //type => !type.Namespace.StartsWith("Scool");
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();
            var config = context.GetConfiguration();

            

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAbpRequestLocalization();

            if (!env.IsDevelopment())
            {
                app.UseErrorPage();
            }

            app.UseCorrelationId();

            if (env.IsDevelopment())
            {
                app.UseStaticFiles();
            }
            else
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(config["FileUploadBasePath"]),
                    RequestPath = "",
                    OnPrepareResponse = ctx =>
                    {
                        ctx.Context.Response.Headers.Append(
                            "Cache-Control", $"public, max-age=604800");
                    },
                    // ContentTypeProvider = FileExtensionContentTypeProviderBuilder.Build()
                });
            }

            app.UseRouting();
            app.UseCors(CorsPolicyName);
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/signalr-hubs-notification"))
                {
                    var bearerToken = context.Request.Query["access_token"];
                    if (!string.IsNullOrEmpty(bearerToken))
                    {
                        context.Request.Headers.Add("Authorization", $"Bearer {bearerToken}");
                    }
                }
                await next();
            });

            app.UseAuthentication();
            app.UseJwtTokenMiddleware();


            if (MultiTenancyConsts.IsEnabled)
            {
                app.UseMultiTenancy();
            }

            //  app.Use(async (ctx, next) =>
            // {
            //     var configuration = context.GetConfiguration();
            //     ctx.SetIdentityServerOrigin(configuration["App:IdentityServerOrigin"]);
            //     await next();
            // });

            app.UseUnitOfWork();
            app.UseMiddleware<AsyncInitializationMiddleware>();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseAbpSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Scool API");

                var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
                c.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
                c.OAuthClientSecret(configuration["AuthServer:SwaggerClientSecret"]);
                c.OAuthScopes("Scool");
            });

            app.UseAuditing();
            app.UseAbpSerilogEnrichers();
            //app.UseConfiguredEndpoints();
            app.UseConfiguredEndpoints(endpoints =>
            {
                endpoints.MapHub<NotificationHub>("/signalr-hubs-notification", options =>
                {
                    options.LongPolling.PollTimeout = TimeSpan.FromSeconds(30);
                });
            });
        }
    }
}
