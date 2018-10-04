using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace punk_tex_backend.Routes
{
    static class Login
    {
        public static void CreateRoutes(IRouteBuilder r)
        {
            r.MapPost("/register", _Register);
            r.MapPost("/login", _Login);
        }

        static async Task _Login(HttpRequest request, HttpResponse response, RouteData data) {
            await response.WriteAsync("Not Implemented.");
        }

        static async Task _Register(HttpRequest request, HttpResponse response, RouteData data) {
            await response.WriteAsync("Not Implemented.");
        }
    }
}