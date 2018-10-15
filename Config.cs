using System;
using System.Text;

namespace punk_tex_backend
{
    public static class Config
    {
        public const uint PORT = 5000;

        public static byte[] JWTSecret
        {
            get;
            private set;
        } = Encoding.UTF8.GetBytes("dfngkdiusdobfsngsngpsbdgsdbgp40967394670");

        public static string Authority { get; set; } = $"http://localhost:{PORT}";
        public static string Audience  { get; set; } = Authority;
    }
}