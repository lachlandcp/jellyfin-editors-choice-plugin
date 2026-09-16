using EditorsChoicePlugin.Configuration;
using EditorsChoicePlugin.Helpers;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;

namespace EditorsChoicePlugin;

public class StartupService : IScheduledTask
{
    private readonly ILogger<Plugin> _logger;
    private readonly IApplicationPaths _applicationPaths;
    private readonly PluginConfiguration _config;

    public StartupService(ILogger<Plugin> logger, IApplicationPaths applicationPaths)
    {
        _logger = logger;
        _applicationPaths = applicationPaths;
        _config = Plugin.Instance!.Configuration;
    }

    public string Name => "EditorsChoice Startup";

    public string Key => "Jellyfin.Plugin.EditorsChoice.Startup";

    public string Description => "Registers the Editors Choice frontend script.";

    public string Category => "Startup Services";

    public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Editors Choice startup: registering the frontend script.");

        if (string.IsNullOrEmpty(_config.Mode))
        {
            _config.Mode = _config.ShowRandomMedia ? "RANDOM" : "FAVOURITES";
        }

        string basePath = GetBasePath();
        if (!FrontendRegistration.TryRegisterConfigured(_config, _applicationPaths, basePath, _logger))
        {
            _logger.LogWarning(
                "Editors Choice could not register its frontend script using method {InjectionMethod}.",
                FrontendInjectionMethods.Normalize(_config.FrontendInjectionMethod));
        }

        return Task.CompletedTask;
    }

    public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
    {
        yield return new TaskTriggerInfo
        {
            Type = TaskTriggerInfoType.StartupTrigger
        };
    }

    private string GetBasePath()
    {
        try
        {
            NetworkConfiguration networkConfiguration = Plugin.Instance!.ServerConfigurationManager.GetNetworkConfiguration();
            if (!string.IsNullOrWhiteSpace(networkConfiguration.BaseUrl))
            {
                string basePath = $"/{networkConfiguration.BaseUrl.Trim().Trim('/')}";
                _logger.LogInformation("Editors Choice base path is {BasePath}.", basePath);
                return basePath;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to get the Jellyfin base path; using '/'.");
        }

        return string.Empty;
    }
}
