using MediaBrowser.Model.Plugins;

namespace EditorsChoicePlugin.Configuration;

public class PluginConfiguration : BasePluginConfiguration {
    public PluginConfiguration(){}

    public string EditorUserId {get; set; } = "";

    public bool DoScriptInject {get; set;} = true;

}