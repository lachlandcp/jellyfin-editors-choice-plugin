using MediaBrowser.Common.Plugins;
using EditorsChoicePlugin.Configuration;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Model.Serialization;
using System.Globalization;
using MediaBrowser.Controller.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace EditorsChoicePlugin;

public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages {

    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILogger<Plugin> logger, IServerConfigurationManager configurationManager) : base(applicationPaths, xmlSerializer) {
        Instance = this;

        // https://github.com/nicknsy/jellyscrub/blob/main/Nick.Plugin.Jellyscrub/JellyscrubPlugin.cs
        if (Configuration.DoScriptInject) {
            if (!string.IsNullOrWhiteSpace(applicationPaths.WebPath))
            {
                var indexFile = Path.Combine(applicationPaths.WebPath, "index.html");
                if (File.Exists(indexFile))
                {
                    string indexContents = File.ReadAllText(indexFile);
                    string basePath = "";

                    // Get base path from network config
                    try
                    {
                        var networkConfig = configurationManager.GetConfiguration("network");
                        var configType = networkConfig.GetType();
                        var basePathField = configType.GetProperty("BaseUrl");
                        var confBasePath = basePathField?.GetValue(networkConfig)?.ToString()?.Trim('/');

                        if (!string.IsNullOrEmpty(confBasePath)) basePath = "/" + confBasePath.ToString();
                    }
                    catch (Exception e)
                    {
                        logger.LogError("Unable to get base path from config, using '/': {e}", e);
                    }

                    // Don't run if script already exists
                    string scriptReplace = "<script plugin=\"EditorsChoice\".*?></script><style plugin=\"EditorsChoice\">.*?</style>";
                    string scriptElement = string.Format("<script plugin=\"EditorsChoice\" defer=\"defer\" version=\"1.0.0.1\" src=\"{0}/EditorsChoice/script\"></script><style plugin=\"EditorsChoice\">.sections.homeSectionsContainer {1}</style>", basePath, "{padding-top:360px;}");

                    if (!indexContents.Contains(scriptElement))
                    {
                        logger.LogInformation("Attempting to inject editorschoice client script code in {indexFile}", indexFile);

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
                                logger.LogInformation("Finished injecting EditorsChoice script code in {indexFile}", indexFile);
                            }
                            catch (Exception e)
                            {
                                logger.LogError("Encountered exception while writing to {indexFile}: {e}", indexFile, e);
                            }
                        }
                        else
                        {
                            logger.LogInformation("Could not find closing body tag in {indexFile}", indexFile);
                        }
                    }
                    else
                    {
                        logger.LogInformation("Found client script injected in {indexFile}", indexFile);
                    }
                }
            }
        }
    }
    public override string Name => "EditorsChoice";

    public override Guid Id => Guid.Parse("70bb2ec1-f19e-46b5-b49a-942e6b96ebae");

    public override string Description => base.Description;

    public static Plugin? Instance { get; private set;}

    public IEnumerable<PluginPageInfo> GetPages() {
        return new [] {
            new PluginPageInfo {
                Name = this.Name,
                EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Configuration.configPage.html", GetType().Namespace)
            }
        };
    }
}