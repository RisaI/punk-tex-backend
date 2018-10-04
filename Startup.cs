using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using static System.Net.WebRequestMethods;

namespace punk_tex_backend
{
    public class Startup
    {
        // Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouter(r => {
                punk_tex_backend.Routes.Login.CreateRoutes(r);
                r.MapGet("/test", _404);
            });

            app.UseStaticFiles();
        }

        public async Task _404(HttpContext c) {
            await c.Response.WriteAsync("404 :)");
        }
    }
}
