using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace FrostAura.Services.Identity.Api.Tests.AuthFlow
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services
                .AddAuthentication(config =>
                {
                    config.DefaultScheme = "Cookie";
                    config.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookie")
                .AddOpenIdConnect("oidc", config => 
                {
                    config.Authority = "https://localhost:5001";
                    config.ClientId = "FrostAura.Services.Identity.Api.Tests.AuthFlow";
                    config.ClientSecret = "Password1234$";
                    config.SaveTokens = true;
                    config.ResponseType = "code";
                    config.Scope.Add("frostaura.scopes.default");
                    // This is how the app will retrieve the custom claims for the user if AlwaysIncludeUserClaimsInIdToken = false.  
                    //config.GetClaimsFromUserInfoEndpoint = true;
                    // Configure cookie claims mapping to get custom scope claims to come through. This applies when using a federated gateway too.
                    //config.ClaimActions.MapUniqueJsonKey("", "");
                });
            services
                .AddAuthorization(config => 
                {
                    config.AddPolicy("ClaimType.Name", builder => builder
                        .RequireAuthenticatedUser()
                        .RequireClaim(ClaimTypes.Name));
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
