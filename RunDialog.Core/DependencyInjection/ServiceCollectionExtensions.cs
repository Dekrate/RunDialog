using Microsoft.Extensions.DependencyInjection;
using RunDialog.Core.Interfaces;
using RunDialog.Core.Services;
using RunDialog.Core.Strategies;

namespace RunDialog.Core.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRunDialogCore(this IServiceCollection services)
    {
        services.AddSingleton<ILocalizationService, LocalizationService>();
        services.AddSingleton<IRunDialogSettings, RunDialogSettings>();
        services.AddSingleton<IHistoryRepository, InMemoryHistoryRepository>();
        services.AddTransient<ICommandParser, CommandParser>();
        services.AddTransient<ICommandExecutor, CommandExecutor>();

        // Strategy Pattern: register all execution strategies
        services.AddTransient<IExecutionStrategy, ProgramExecutionStrategy>();
        services.AddTransient<IExecutionStrategy, UriExecutionStrategy>();
        services.AddTransient<IExecutionStrategy, FolderExecutionStrategy>();

        return services;
    }
}
