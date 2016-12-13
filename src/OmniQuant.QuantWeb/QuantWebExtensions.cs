using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OmniQuant.QuantWeb
{
    public class QuantWebOptions
    {
    }

    public static class QuantWebExtensions
    {
        public static IApplicationBuilder UseQuantWeb(this IApplicationBuilder app, QuantWebOptions options)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
            return app;
        }
    }



    public class CustomCompilationService : DefaultRoslynCompilationService, ICompilationService
    {
        public CustomCompilationService(CSharpCompiler compiler,
            IRazorViewEngineFileProviderAccessor fileProviderAccessor, IOptions<RazorViewEngineOptions> optionsAccessor,
            ILoggerFactory loggerFactory) : base(compiler, fileProviderAccessor, optionsAccessor, loggerFactory)
        {
        }

        CompilationResult ICompilationService.Compile(RelativeFileInfo fileInfo, string compilationContent)  => Compile(fileInfo, compilationContent);
    }
}
