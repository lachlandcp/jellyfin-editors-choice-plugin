using MediaBrowser.Common.Plugins;
using EditorsChoicePlugin.Configuration;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Model.Serialization;
using System.Globalization;
using Microsoft.Extensions.Logging;
using MediaBrowser.Model.Configuration;
using MediaBrowser.Controller.Configuration;

namespace EditorsChoicePlugin;

public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    private readonly ILogger _logger;
    public IServiceProvider ServiceProvider { get; }

    public IServerConfigurationManager ServerConfigurationManager { get; }

    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILogger<Plugin> logger, IServiceProvider serviceProvider, IServerConfigurationManager serverConfigurationManager) : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
        _logger = logger;
        ServiceProvider = serviceProvider;
        ServerConfigurationManager = serverConfigurationManager;
    }
    public override string Name => "EditorsChoice";

    public override Guid Id => Guid.Parse("70bb2ec1-f19e-46b5-b49a-942e6b96ebae");

    public override string Description => base.Description;

    public static Plugin? Instance { get; private set; } = null!;

    public IEnumerable<PluginPageInfo> GetPages()
    {
        return new[] {
            new PluginPageInfo {
                Name = this.Name,
                EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Configuration.configPage.html", GetType().Namespace)
            }
        };
    }
}