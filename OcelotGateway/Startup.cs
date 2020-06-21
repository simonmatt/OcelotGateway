using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Administration;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;

namespace OcelotGateway
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
            var ocelotConfig = new ConfigurationBuilder()
                .AddJsonFile("configuration.json")
                .Build();

            services.AddOcelot(ocelotConfig)
                .AddConsul() // Consul服务发现
                .AddPolly() // 熔断 https://github.com/App-vNext/Polly
                .AddCacheManager(x => x.WithDictionaryHandle())
                //.AddAdministration("/administration", Opts); // 外部IdentityServer方式
                .AddAdministration("/administration", "secret"); // 内部IdentityServer方式

            // services.AddSingleton<IOcelotCache<CachedResponse>, MyCache>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication("TestKey", Opts);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        private void Opts(IdentityServerAuthenticationOptions options)
        {
            options.Authority = "http://localhost:5003";
            options.ApiName = "api1";
            options.RequireHttpsMetadata = false;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            await app.UseOcelot();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}