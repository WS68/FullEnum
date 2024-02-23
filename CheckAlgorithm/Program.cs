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
            runner.Run();

            app.Dispose();
        }
    }
}