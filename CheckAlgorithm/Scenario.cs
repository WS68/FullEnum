using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RunAlgorithm.Core;
using SyncRootFolders;
using TwoPhaseCommit;

namespace CheckAlgorithm;

public static class Scenario
    {
    public static IHostBuilder CheckTwoPhaseCommit(this IHostBuilder builder)
        {
        return builder.ConfigureServices(s =>
        {
            s.AddSingleton<IActor>(new TwoPhaseCommit.Actor("1st"));
            s.AddSingleton<IActor>(new TwoPhaseCommit.Actor("2nd"));
            s.AddSingleton<IContext, TwoPhaseCommit.Context>();
            s.AddSingleton<IValidator, TwoPhaseCommit.Validator>();
            s.AddTransient<Runner>();
        });
        }

    public static IHostBuilder CheckSyncRootFolder(this IHostBuilder builder)
        {
        return builder.ConfigureServices(s =>
        {
            s.AddSingleton<IActor>(new SyncRootFolders.Actor("Remove", false, true));
            s.AddSingleton<IActor>(new SyncRootFolders.Actor("Add", true, false));
            s.AddSingleton<IContext, SyncRootFolders.Context>();
            s.AddSingleton<IValidator, SyncRootFolders.Validator>();
            s.AddTransient<Runner>();
        });
        }
    }
