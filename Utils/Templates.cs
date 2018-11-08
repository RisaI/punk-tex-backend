using System;
using System.IO;

using punk_tex_backend.Models;

namespace punk_tex_backend.Utils
{
    public static class Templates
    {
        public static string RootPath {get;} = "templates/";

        public static string GetPath(Template template) {
            return Path.Combine(Path.Combine(RootPath, template.ID.ToString()));
        }

        public static string CreateDirectory(Template template) {
            var path = GetPath(template);
            Directory.CreateDirectory(path);
            return path;
        }

        public static string[] GetFiles(Template template) {
            var path = GetPath(template);
            if (!Directory.Exists(path))
                throw new InvalidOperationException("Template directory does not exist.");

            return Directory.GetFileSystemEntries(path);
        }

        public static void RemoveTemplate(Template template) {
            Directory.Delete(GetPath(template));
        }
    }
}