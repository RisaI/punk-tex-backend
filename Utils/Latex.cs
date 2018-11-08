using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using punk_tex_backend.Models;

namespace punk_tex_backend.Utils
{
    public static class Latex
    {
        public static string WORKDIR { get; } = "/tmp/punk-tex-compilation";
        public static string DEFAULT { get; } = "\\documentclass{article}\n\\usepackage[utf8]{inputenc}\n\\usepackage{amsmath}\n\\usepackage{enumerate}\n\\usepackage{enumitem}\n\\usepackage{soul}\n\\usepackage{hyperref}\n\\usepackage{listings}\n\\begin{document}\n\n\\include{content}\n\n\\end{document}";
        
        public static async Task<MemoryStream> Compile(string latex, Template template) {
            using (var job = LatexCompileJob.CreateNew()) {                
                job.PrepareWorkingDirectory();

                using (FileStream stream = new FileStream(job.ContentTex, FileMode.CreateNew, FileAccess.Write))
                using (TextWriter writer = new StreamWriter(stream)) {
                    writer.Write(latex);
                }

                if (template != null) {
                    var files = Templates.GetFiles(template);
                    var templatePath = Templates.GetPath(template);
                    for (int i = 0; i < files.Length; ++i) {
                        await CreateSymbolicLink(Path.GetFullPath(files[i]),
                            Path.Combine(job.WorkingDirectory, Path.GetRelativePath(templatePath, files[i])));
                    }
                } else {
                    using (FileStream stream = new FileStream(job.MainTex, FileMode.CreateNew, FileAccess.Write))
                    using (TextWriter writer = new StreamWriter(stream)) {
                        writer.Write(DEFAULT);
                    }
                }
                
                await job.ExecuteCompilation();

                using (FileStream stream = new FileStream(job.MainPdf, FileMode.Open, FileAccess.Read)) {
                    var buffer = new byte[stream.Length];
                    MemoryStream output = new MemoryStream(buffer);
                    stream.CopyTo(output);
                    output.Position = 0;

                    return output;
                }
            }
        }
        

        public static async Task CreateSymbolicLink(string src, string target) {
            if (!File.Exists(src) && !Directory.Exists(src))
                throw new FileNotFoundException("Source does not exist");

            if (!Path.IsPathFullyQualified(target))
                throw new ArgumentException("The target must be a fully qualified path.", "target");

            Console.WriteLine($"ln -s {src} {target}");
            using (Process p = Process.Start(new ProcessStartInfo("ln", $"-s {src} {target}"))) {
                await p.WaitForExitAsync();
                if (p.ExitCode > 0) {
                    throw new SystemException("Failed to create a symlink");
                }
            }
        }

        public static Task WaitForExitAsync(this Process process, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<object>();
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => tcs.TrySetResult(null);
            if(cancellationToken != default(CancellationToken))
                cancellationToken.Register(tcs.SetCanceled);

            return tcs.Task;
        }
    }

    public class LatexCompileJob : IDisposable
    {
        public Guid ID {get; private set;} = Guid.Empty;
        public JobState State {get; private set;} = JobState.None;

        public string WorkingDirectory => Path.Combine(Latex.WORKDIR, ID.ToString());
        public string MainTex => Path.Combine(WorkingDirectory, "main.tex");
        public string MainPdf => Path.Combine(WorkingDirectory, "main.pdf");
        public string ContentTex => Path.Combine(WorkingDirectory, "content.tex");

        private LatexCompileJob(Guid id) {
            ID = id;
        }

        ~LatexCompileJob() {
            Dispose();
        }

        public static LatexCompileJob CreateNew() {
            return new LatexCompileJob(Guid.NewGuid());
        }

        public void PrepareWorkingDirectory() {
            if (State == JobState.None) {
                try {
                    Directory.CreateDirectory(WorkingDirectory);
                } catch (Exception ex) {
                    RemoveWorkingDirectory();
                    throw ex;
                }
            }
        }

        private void RemoveWorkingDirectory() {
            Directory.Delete(WorkingDirectory, true);
        }

        public async Task ExecuteCompilation() {
            using (Process p = Process.Start(new ProcessStartInfo("pdflatex", "-halt-on-error main.tex") { WorkingDirectory = WorkingDirectory })) {
                await p.WaitForExitAsync();
                if (p.ExitCode > 0) {
                    State = JobState.Failed;
                    throw new SystemException("Failed to compile this TeX project.");
                }
            }
            State = JobState.Finished;
        }

        public void Dispose()
        {
            if (State != JobState.None && State != JobState.Disposed) {
                RemoveWorkingDirectory();
                State = JobState.Disposed;
            }
        }

        public enum JobState
        {
            None,
            Prepared,
            Finished,
            Failed,
            Disposed,
        }
    }
}