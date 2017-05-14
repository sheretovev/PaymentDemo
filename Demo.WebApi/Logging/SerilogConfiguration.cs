using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Demo.WebApi.Logging
{
    public class StandardLengthLevelName : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var newprop = $"[{logEvent.Level.ToString()}]";
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(nameof(StandardLengthLevelName), newprop));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("UtcTimestamp", logEvent.Timestamp.UtcDateTime));
        }
    }
    public static class SerilogConfigurator
    {
        public static void Configure(IHostingEnvironment env, string logsRootPath = null, string appName = null)
        {
            var template = "{UtcTimestamp:yyyy-MM-dd HH:mm:ss} {StandardLengthLevelName} {SourceContext}: {Tab}{Message}{NewLine}{Exception}";
            var logsLocation = !string.IsNullOrEmpty(logsRootPath) ? logsRootPath : (env.ContentRootPath + "\\logs");
            appName = appName ?? env.ApplicationName;
            var verboseLogger = new LoggerConfiguration()
             .Filter.ByIncludingOnly(x => x.Level == LogEventLevel.Debug || x.Level == LogEventLevel.Verbose)
             .WriteTo.RollingFile(
                 logsLocation + $"\\{appName}-verbose-{{Date}}.txt",
                 outputTemplate: template,
                 retainedFileCountLimit: 5
             )
           .CreateLogger();

            var errorLogger = new LoggerConfiguration()
             .Filter.ByIncludingOnly(x =>
                x.Level == LogEventLevel.Error
                || x.Level == LogEventLevel.Fatal
                || x.Level == LogEventLevel.Warning)
             .WriteTo.RollingFile(
                 logsLocation + $"\\{appName}-error-{{Date}}.txt",
                 outputTemplate: template,
                 retainedFileCountLimit: 5
             )
           .CreateLogger();

            var informationLogger = new LoggerConfiguration()
              .Filter.ByIncludingOnly(x => x.Level == LogEventLevel.Information)
              .WriteTo.RollingFile(
                  logsLocation + $"\\{appName}-info-{{Date}}.txt",
                  outputTemplate: template,
                  retainedFileCountLimit: 5
              )
            .CreateLogger();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Logger(errorLogger)
                .WriteTo.Logger(informationLogger)
                .WriteTo.Logger(verboseLogger)
                .Enrich.FromLogContext().Enrich.With<StandardLengthLevelName>()
                .CreateLogger();
        }
    }
}