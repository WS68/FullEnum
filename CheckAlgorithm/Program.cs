using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RunAlgorithm.Core;
using TwoPhaseCommit;

namespace CheckAlgorithm
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureLogging(l =>
                {
                    l.AddConsole();
                    l.SetMinimumLevel(LogLevel.Information);
                })
                .ConfigureServices(s =>
                {
                    s.AddSingleton<IActor>( new Actor("1st") );
                    s.AddSingleton<IActor>(new Actor("2nd"));

                    s.AddSingleton<IContext, Context>();
                    s.AddSingleton<IValidator, Validator>();

                    s.AddTransient<Runner>();
                });

            var app = builder.Build();
            var runner = app.Services.GetRequiredService<Runner>();
            runner.Progress += (sender, runArgs) =>
            {
                Console.Write( $"\rProcessed: {runArgs.Statistics.Total:N0}/{runArgs.Statistics.Failures:N0}");
                Console.Out.Flush();
            };

            var stat = runner.Run();
            Console.WriteLine();

            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            if (stat.Failures > 0)
            {
                logger.LogWarning($"RESULT({stat.Total:N0}): OK={stat.PositiveResults:N0}, Negative={stat.NegativeResults:N0}, Failures={stat.Failures}:N0");
            }
            else
            {
                logger.LogInformation($"RESULT({stat.Total:N0}): OK={stat.PositiveResults:N0}, Negative={stat.NegativeResults:N0}, Failures={stat.Failures:N0}");
            }
            app.Dispose();
        }
    }
}