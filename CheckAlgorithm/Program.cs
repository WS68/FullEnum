using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RunAlgorithm.Core;
using TwoPhaseCommit;

namespace CheckAlgorithm
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new HostBuilder()
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
        }
    }
}