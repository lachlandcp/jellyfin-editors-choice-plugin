using EditorsChoicePlugin.Configuration;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;
using MediaBrowser.Common.Configuration;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;
using System.Net.Mime;

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

    public StartupService(IServerApplicationHost serverApplicationHost, ILogger<Plugin> logger, IUserManager userManager, IApplicationPaths applicationPaths, IServerApplicationHost applicationHost, IConfigurationManager configurationManager)
    {
        _serverApplicationHost = serverApplicationHost;
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

        // https://github.com/nicknsy/jellyscrub/blob/main/Nick.Plugin.Jellyscrub/JellyscrubPlugin.cs
        if (_config.DoScriptInject)
        {
            if (!string.IsNullOrWhiteSpace(_applicationPaths.WebPath))
            {
                var indexFile = Path.Combine(_applicationPaths.WebPath, "index.html");
                if (File.Exists(indexFile))
                {
                    string indexContents = File.ReadAllText(indexFile);
                    string basePath = "";

                    // Get base path from network config
                    try
                    {
                        var networkConfig = _configurationManager.GetConfiguration("network");
                        var configType = networkConfig.GetType();
                        var basePathField = configType.GetProperty("BaseUrl");
                        var confBasePath = basePathField?.GetValue(networkConfig)?.ToString()?.Trim('/');

                        if (!string.IsNullOrEmpty(confBasePath)) basePath = "/" + confBasePath.ToString();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Unable to get base path from config, using '/': {e}", e);
                    }

                    // Don't run if script already exists, unless it's a variation.
                    string scriptReplace = "<script plugin=\"EditorsChoice\".*?></script>(<style plugin=\"EditorsChoice\">.*?</style>)?";
                    string scriptElement = string.Format("<script injection=\"true\" plugin=\"EditorsChoice\" defer=\"defer\" src=\"{0}/EditorsChoice/script\"></script>", basePath);

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

    public async Task RegisterTransformation()
    {
        try
        {
            string? publishedServerUrl = _config.Url;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(publishedServerUrl ?? $"http://localhost:{_applicationHost.HttpPort}");

            JsonObject data = new JsonObject
            {
                { "id", "b3d45a0e-3dac-4413-97df-32a13316571e" },
                { "fileNamePattern", "index.html" },
                { "transformationEndpoint", "/editorschoice/transform" }
            };

            HttpResponseMessage resp = await client.PostAsync("/FileTransformation/RegisterTransformation", new StringContent(data.ToString(), MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json)));
            _logger.LogInformation(resp.ToString());
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
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
