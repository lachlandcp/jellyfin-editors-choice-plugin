using EditorsChoicePlugin.Configuration;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;
using MediaBrowser.Common.Configuration;
using System.Text.RegularExpressions;
using MediaBrowser.Common.Net;
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
    
    private readonly IServerApplicationHost _serverApplicationHost;
    private readonly ILogger<Plugin> _logger;
    private readonly IUserManager _userManager;
    private readonly IApplicationPaths _applicationPaths;
    private readonly IServerApplicationHost _applicationHost;
    private readonly IConfigurationManager _configurationManager;
    private readonly PluginConfiguration _config;
    private String _basePath;

    public StartupService(IServerApplicationHost serverApplicationHost, ILogger<Plugin> logger, IUserManager userManager, IApplicationPaths applicationPaths, IServerApplicationHost applicationHost, IConfigurationManager configurationManager)
    {
        _serverApplicationHost = serverApplicationHost;
        _logger = logger;
        _userManager = userManager;
        _applicationPaths = applicationPaths;
        _applicationHost = applicationHost;
        _configurationManager = configurationManager;
        _config = Plugin.Instance!.Configuration;
        _basePath = "";
    }

    public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"EditorsChoice Startup. Registering file transformations.");

        
        
        // Convert configuration mode boolean variable
        if (_config.Mode == "")
        {
            if (_config.ShowRandomMedia)
            {
                _config.Mode = "RANDOM";
            }
            else
            {
                _config.Mode = "FAVOURITES";
            }
        }

        // Get base path from network config
        try
        {
            NetworkConfiguration networkConfiguration = Plugin.Instance!.ServerConfigurationManager.GetNetworkConfiguration();

            string basePath = "";
            if (!string.IsNullOrWhiteSpace(networkConfiguration.BaseUrl))
            {
                basePath = $"/{networkConfiguration.BaseUrl.TrimStart('/').Trim()}";
            }

            if (!string.IsNullOrEmpty(basePath))
            {
                _basePath = basePath.ToString();
                _logger.LogInformation($"BasePath is {_basePath}");
            }
        }
        catch (Exception e)
        {
            _logger.LogError("Unable to get base path from config, using '/': {e}", e);
        }

        // https://github.com/nicknsy/jellyscrub/blob/main/Nick.Plugin.Jellyscrub/JellyscrubPlugin.cs
        if (_config.DoScriptInject)
        {
            if (!string.IsNullOrWhiteSpace(_applicationPaths.WebPath))
            {
                var indexFile = Path.Combine(_applicationPaths.WebPath, "index.html");
                if (File.Exists(indexFile))
                {
                    string indexContents = File.ReadAllText(indexFile);


                    // Don't run if script already exists, unless it's a variation.
                    string scriptReplace = "<script plugin=\"EditorsChoice\".*?></script>(<style plugin=\"EditorsChoice\">.*?</style>)?";
                    string scriptElement = string.Format("<script injection=\"true\" plugin=\"EditorsChoice\" defer=\"defer\" src=\"{0}/EditorsChoice/script\"></script>", _basePath);

                    if (!indexContents.Contains(scriptElement))
                    {
                        _logger.LogInformation("Attempting to inject editorschoice client script code in {indexFile}", indexFile);

                        // Replace old scripts
                        indexContents = Regex.Replace(indexContents, scriptReplace, "");

                        // Insert script last in body
                        int bodyClosing = indexContents.LastIndexOf("</body>");
                        if (bodyClosing != -1)
                        {
                            indexContents = indexContents.Insert(bodyClosing, scriptElement);

                            try
                            {
                                File.WriteAllText(indexFile, indexContents);
                                _logger.LogInformation("Finished injecting EditorsChoice script code in {indexFile}", indexFile);
                            }
                            catch (Exception e)
                            {
                                _logger.LogError("Encountered exception while writing to {indexFile}: {e}", indexFile, e);
                            }
                        }
                        else
                        {
                            _logger.LogInformation("Could not find closing body tag in {indexFile}", indexFile);
                        }
                    }
                    else
                    {
                        _logger.LogInformation("Found client script injected in {indexFile}", indexFile);
                    }
                }
            }
        }
        else if (_config.FileTransformation)
        {
            _ = RegisterTransformation();
        }

        return Task.CompletedTask;
    }

    public Task RegisterTransformation()
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

            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            return Task.CompletedTask;
        }
    }

    public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
    {
        yield return new TaskTriggerInfo()
        {
            Type = TaskTriggerInfo.TriggerStartup
        };
    }
}
