using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OIDC.APIClient
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
               .AddCookie(options =>
               {
                   options.Cookie.HttpOnly = false;
                   options.Cookie.SameSite = SameSiteMode.None;
                   // Configure the client application to use sliding sessions
                   options.SlidingExpiration = true;
                   // Expire the session of 15 minutes of inactivity
                   options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
               })
               .AddOpenIdConnect(options =>
               {
                   options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
                   options.Authority = "https://localhost:44367/";
                   options.RequireHttpsMetadata = true;
                   options.ClientId = "IDS4ApiClient";
                   options.ClientSecret = "IDS4ApiClientsecret";
                   options.ResponseType = "code id_token";
                   // options.SignInScheme = "Cookies";
                   options.SaveTokens = true;
                   options.CallbackPath = new PathString("/signin-oidc");
                   // options.SignedOutCallbackPath = new PathString("/signout-callback-oidc");
                   options.GetClaimsFromUserInfoEndpoint = true;
                   // options.GetClaimsFromUserInfoEndpoint = true;                   
                   options.Scope.Add("openid");
                   options.Scope.Add("profile");
                   options.Scope.Add("address");
                   // options.SignedOutRedirectUri = "https://localhost:44335/";
                   // options.Scope.Add("email");
                   options.Events = new OpenIdConnectEvents()
                   {
                       OnTokenValidated = tokenValidatedContext =>
                       {
                           var identity = tokenValidatedContext.Principal.Identity as ClaimsIdentity;
                           var subjectClaim = identity.Claims.FirstOrDefault(z => z.Type == "sub");

                           //var newClaimsIdentity = new ClaimsIdentity(tokenValidatedContext.Options.SignInScheme, "given_name", "role");
                           //newClaimsIdentity.AddClaim(subjectClaim);


                           return Task.FromResult(0);
                       },

                       OnUserInformationReceived = userInformationReceivedContext =>
                       {

                           // userInformationReceivedContext.User.Remove("address");

                           return Task.FromResult(0);
                       },

                       OnTicketReceived = context =>
                       {
                           context.Properties.ExpiresUtc = DateTime.UtcNow.AddHours(1);
                           return Task.FromResult(0);
                       }
                   };
               });
            services.AddAuthorization();
            services.AddControllersWithViews();
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

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.None,
            });
            app.UseAuthorization();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
