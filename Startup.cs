using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;

using punk_tex_backend.Utils;
using static System.Net.WebRequestMethods;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace punk_tex_backend
{
    public class Startup
    {
        // Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ProjectContext>(opt => {
                opt.UseMySql("Server=localhost;Database=punktex;User=root;Password=test123;");
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(j => {
                j.Authority = Config.Authority;
                j.Audience = Config.Audience;

#if Debug
                j.MetadataAddress = null;
                j.Configuration = new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration();
                j.RequireHttpsMetadata = false;
#endif

                j.TokenValidationParameters = new TokenValidationParameters() {
                    ClockSkew = TimeSpan.FromMinutes(2),
                    IssuerSigningKey = new SymmetricSecurityKey(Config.JWTSecret),

                    ValidateAudience = false,
                    ValidateIssuer = false,
                    
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };

                j.Validate();
            });

            services.AddMvc();
        }

        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();
            app.UseStaticFiles();
        }
    }
}
