using EditorsChoicePlugin.Configuration;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;
using MediaBrowser.Common.Configuration;
using System.Reflection;
using System.Runtime.Loader;
using Newtonsoft.Json.Linq;
using EditorsChoicePlugin.Helpers;

namespace EditorsChoicePlugin;
public class StartupService : IScheduledTask
{
    public string Name => "EditorsChoice Startup";

    public string Key => "Jellyfin.Plugin.EditorsChoice.Startup";
    
    public string Description => "Startup Service for Editors choice";
    
    public string Category => "Startup Services";
    
    private readonly ILogger<Plugin> _logger;
    private readonly IUserManager _userManager;
    private readonly IApplicationPaths _applicationPaths;
    private readonly IServerApplicationHost _applicationHost;
    private readonly IConfigurationManager _configurationManager;
    private readonly PluginConfiguration _config;

    public StartupService(ILogger<Plugin> logger, IUserManager userManager, IApplicationPaths applicationPaths, IServerApplicationHost applicationHost, IConfigurationManager configurationManager)
    {
        _logger = logger;
        _userManager = userManager;
        _applicationPaths = applicationPaths;
        _applicationHost = applicationHost;
        _configurationManager = configurationManager;
        _config = Plugin.Instance!.Configuration;
    }

    public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"EditorsChoice Startup. Registering file transformations.");

         RegisterTransformation();

        return Task.CompletedTask;
    }

    public void RegisterTransformation()
    {
        try
        {
            JObject data = new JObject
            {
                { "id", "b3d45a0e-3dac-4413-97df-32a13316571e" },
                { "fileNamePattern", "index.html" },
                { "callbackAssembly", GetType().Assembly.FullName },
                { "callbackClass", typeof(Transformations).FullName },
                { "callbackMethod", nameof(Transformations.IndexTransformation)}
            };

            Assembly? fileTransformationAssembly = AssemblyLoadContext.All.SelectMany(x => x.Assemblies).FirstOrDefault(x => x.FullName?.Contains(".FileTransformation") ?? false);

            if (fileTransformationAssembly != null)
            {
                Type? pluginInterfaceType = fileTransformationAssembly.GetType("Jellyfin.Plugin.FileTransformation.PluginInterface");
                if (pluginInterfaceType != null)
                {
                    pluginInterfaceType.GetMethod("RegisterTransformation")?.Invoke(null, new object?[] { data });
                }
            }

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to register file transformation - is FileTransformation plugin installed and enabled?");
        }
    }

    public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
    {
        yield return new TaskTriggerInfo()
        {
            Type = TaskTriggerInfoType.StartupTrigger
        };
    }
}
