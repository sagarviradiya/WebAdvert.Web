using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebAdvert.Web.ServiceClients;
using WebAdvert.Web.Services;

namespace WebAdvert.Web
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
             string PoolId = Configuration["AWS:UserPoolId"];
             string ClientId = Configuration["AWS:UserPoolClientId"];
            //var providerConfig = new AmazonCognitoIdentityProviderConfig();
            
            //var provider = new AmazonCognitoIdentityProviderClient(RegionEndpo);
            //var cognitoUserPool = new CognitoUserPool(PoolId, ClientId, provider);

            //services.AddSingleton<IAmazonCognitoIdentityProvider>(provider);
            //services.AddSingleton(cognitoUserPool);
            services.AddCognitoIdentity();
            //Default Login Url on after logut or on loading website default page.
            services.ConfigureApplicationCookie(options => options.LoginPath = "/Accounts/Login");
            services.AddTransient<IFileUploader, S3FileUploader>();
            services.AddHttpClient<IAdvertApiClient, AdvertApiClient>();

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
            }
            app.UseStaticFiles();

            app.UseRouting();

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
