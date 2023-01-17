using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace TechLogger
{
    public static class TechLogger
    {
        public static void RegisterLogger(string environment, string projectName, IConfigurationRoot configuration)
        {
            ConfigureLogging(environment, projectName, configuration);

            try
            {
                Log.Information("TechLogger service started.");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "TechLogger service failed to start.");
            }
        }

        private static void ConfigureLogging(string environment, string projectName, IConfigurationRoot configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithMachineName()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment, projectName))
                .Enrich.WithProperty("Environment", environment)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private static ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment, string projectName)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]))
            {
                AutoRegisterTemplate = true,
                IndexFormat = $"{projectName}-{environment}-{DateTime.UtcNow:yyyy-MM}"
            };
        }
    }
}